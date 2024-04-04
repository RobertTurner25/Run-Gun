
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TempleRun
{
    public class TileSpawner : MonoBehaviour
    {
        [SerializeField]
        private int tileStartCount = 10;
        [SerializeField]
        private int minimumStraightTiles = 3;
        [SerializeField]
        private int maximumStraightTiles = 15;
        [SerializeField]
        private int minimumCurrentTiles = 20;
        [SerializeField]
        private GameObject startingTile;
        [SerializeField]
        private List<GameObject> turnTiles;
        [SerializeField]
        private List<GameObject> obstacles;
        [SerializeField]
        private bool spawnObstacle = false;
        private float spawnObstacleDelay = 2f;

        private Vector3 currentTileLocation = Vector3.zero;
        private Vector3 currentTileDirection = Vector3.forward;
        private GameObject prevTile;

        private List<GameObject> currentTiles;
        private List<GameObject> currentObstacles;

        
        private float _timer =0f;
        private float spawnTileInterval = 11f;
        private float deleteTileInterval = 14f;
        private float _timerSpawn=0f;
        

        private bool gameRunning;
        private float elaspedTime;

        private void Start()
        {
            currentTiles = new List<GameObject>();
            currentObstacles = new List<GameObject>();
            

            gameRunning = true;
            
            Random.InitState(System.DateTime.Now.Millisecond);

            SpawnTiles(1);
            spawnObstacle = true;
            SpawnTiles(29);
           
            



        }
        private void Update()
        {
            elaspedTime += Time.deltaTime;
            Debug.Log("Timer: " + elaspedTime);

           

            if (gameRunning)
            {

                _timerSpawn += Time.deltaTime;
                _timer += Time.deltaTime;

                
                


                if (_timerSpawn >= spawnTileInterval)
                {


                    SpawnTiles(10);

                    _timerSpawn = 0f;

                }
                if (_timer > deleteTileInterval)
                {

                    DeletePreviousTiles();
                    _timer = 0f;

                }
            }
           


        }
        private void SpawnTiles(int number)
        {
            for (int i =0; i < number; ++i)
            {
                SpawnTile(startingTile.GetComponent<Tile>());
            }
          
            
        }

        private void SpawnTile(Tile tile)
            {
            
            Quaternion newTileRotation = Quaternion.LookRotation(currentTileDirection, Vector3.up);
                prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation);
                currentTiles.Add(prevTile);

            if (spawnObstacle) { SpawnObstacle(); }
                   
            if (tile.type == TileType.STRAIGHT)
            {
                currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
            }
            }

        private void DeletePreviousTiles()
        {

            if (currentTiles.Count > 0)
            {
                for (int i = 0; i < Mathf.Min(10, currentTiles.Count); i++)
                {
                    GameObject tile = currentTiles[0];
                    currentTiles.RemoveAt(0);
                    Destroy(tile);
                }
            }

            // Check if there are obstacles to delete
            if (currentObstacles.Count > 0)
            {
                GameObject obstacle = currentObstacles[0];
                currentObstacles.RemoveAt(0);
                Destroy(obstacle);
            }
        }

         public void StopTileSpawning()
        {
            gameRunning = false;
        }
      
        private void SpawnObstacle()
        {
            if (Random.value > .4f) return;

            
                GameObject obstaclePrefab = SelectRandomGameObjectFromList(obstacles);
                Quaternion newObjectRotation = Quaternion.LookRotation(currentTileDirection, Vector3.up);
                GameObject obstacle = Instantiate(obstaclePrefab, currentTileLocation, newObjectRotation);
                Debug.Log(newObjectRotation);
                currentObstacles.Add(obstacle);
            
        }

        private GameObject SelectRandomGameObjectFromList(List<GameObject> list) {
            if (list.Count == 0) return null;

            return list[Random.Range(0, list.Count)];
        }
     
    }
}