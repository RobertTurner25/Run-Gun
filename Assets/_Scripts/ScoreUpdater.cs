using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUpdater : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private GameObject gameOverCanvas;

    public void UpdateScore(int Score)
    {

        scoreText.text = Score.ToString();
        Debug.Log(scoreText.text);
     
    }
}
