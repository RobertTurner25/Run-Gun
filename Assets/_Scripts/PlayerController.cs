using System.Collections;
using TempleRun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput),typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float initialPlayerSpeed = 8f;
    [SerializeField]
    private float maximumPlayerSpeed = 30f;
    [SerializeField]
    private float playerSpeedIncreaseRate = .1f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float initialGravityValue = -9.81f;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask obstacleLayer;
    [SerializeField]
    private AnimationClip slideAnimationClip;
    [SerializeField]
    private GameObject gameOverCanvas;
    [SerializeField]
    private ScoreUpdater scoreUpdater;
    [SerializeField]
    private TileSpawner tileSpawner;

    private bool gameRunning;


    [SerializeField]
    private float playerSpeed;
    [SerializeField]
    private float scoreMultiplier = 1.5f;
    private float gravity;
    private Vector3 movementDirection = Vector3.forward;
    private Vector3 playerVelocity;
    
    private float position=1;
    private PlayerInput playerInput;
    private InputAction leftAction;
    private InputAction rightAction;
    private InputAction jumpAction;
    private InputAction slideAction;



    private CharacterController controller;
    private Animator animator;

    private int slidingAnimationId;

    private bool sliding = false;
    private float score = 0;
   

    [SerializeField]
    private UnityEvent<int> gameOverEvent;
    [SerializeField]
    private UnityEvent<int> scoreUpdateEvent;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        slidingAnimationId = Animator.StringToHash("Sliding");

        leftAction = playerInput.actions["Left"];
        rightAction = playerInput.actions["Right"];
        jumpAction = playerInput.actions["Jump"];
        slideAction = playerInput.actions["Slide"];
        gravity = initialGravityValue;
    }

    private void OnEnable()
    {
        leftAction.performed += PlayerLeft;
        rightAction.performed += PlayerRight;
        slideAction.performed += PlayerSlide;
        jumpAction.performed += PlayerJump;
    }
    private void OnDisable()
    {
        leftAction.performed -= PlayerLeft;
        rightAction.performed -= PlayerRight;
        slideAction.performed -= PlayerSlide;
        jumpAction.performed -= PlayerJump;
    }

    private void Start()
    {
        playerSpeed = initialPlayerSpeed;
        gravity = initialGravityValue;
        gameOverCanvas.SetActive(false);
        gameRunning = true;
    }

 //Should probably find a way to slow down this left and right movement a little bit...
    private void PlayerLeft(InputAction.CallbackContext context)
    {

        if (position > 0)
        {
            float left = leftAction.ReadValue<float>();
            Vector3 moveLeft = new Vector3(-(left), 0, 0);
            controller.Move(moveLeft);
            position--;
        }
    }

    private void PlayerRight(InputAction.CallbackContext context)
    {
        if (position < 2)
        {
            float right = rightAction.ReadValue<float>();
             Vector3 moveRight = new Vector3(right, 0, 0);
            controller.Move(moveRight);
            position++;
        }
    }

    private void PlayerSlide(InputAction.CallbackContext context)
    {
        if(!sliding && isGrounded())
        {
            StartCoroutine(Slide());
        }
    }

    private IEnumerator Slide()
    {
        sliding = true;
        //Shrink the collider
        Vector3 originalControllerCenter = controller.center;
        Vector3 newControllerCenter = originalControllerCenter;
        controller.height /= 2;
        newControllerCenter.y -= controller.height / 2;
        controller.center = newControllerCenter;

        

        // Play the sliding animation
        animator.Play(slidingAnimationId);
        yield return new WaitForSeconds(slideAnimationClip.length);
       // Set back to normal after sliding
        controller.height *= 2;
        controller.center = originalControllerCenter;
        sliding = false;
    }
    private void PlayerJump(InputAction.CallbackContext context)
    {
        if (isGrounded())
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * gravity * -3f);
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }

    private void Update()
    {

        //Score functionality
        score += scoreMultiplier * Time.deltaTime;
        

        if (scoreUpdater != null)
        {
            if (gameRunning)
            {
                scoreUpdater.UpdateScore((int)score);
            }
        }

        if (!isGrounded(20f))
        {
            GameOver();
            return;
        }

        controller.Move(playerSpeed * Time.deltaTime * transform.forward);

        if(isGrounded() && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        

    }

    private bool isGrounded(float length = .2f)
    {
        Vector3 raycastOriginFirst = transform.position;
        raycastOriginFirst.y -= controller.height / 2f;
        raycastOriginFirst.y += .1f;

        Vector3 raycastOriginSecond = raycastOriginFirst;
        raycastOriginFirst -= transform.forward * .2f;
        raycastOriginSecond += transform.forward * .2f;

        Debug.DrawLine(raycastOriginFirst, Vector3.down, Color.green, 2f);
        Debug.DrawLine(raycastOriginSecond, Vector3.down, Color.red, 2f);
        if (Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, groundLayer) || Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, groundLayer))
        {
            return true;
        }
        return false;
    }

    private void GameOver()
    {

        Debug.Log("Game Over");
        gameOverEvent.Invoke((int)score);
        //gameObject.SetActive(false);
        playerSpeed = 0;
        gameRunning = false;


          if (tileSpawner != null)
        {
            tileSpawner.StopTileSpawning();
        }

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (((1 << hit.collider.gameObject.layer) & obstacleLayer)!= 0)
        {
            GameOver();
        }
    }
}


