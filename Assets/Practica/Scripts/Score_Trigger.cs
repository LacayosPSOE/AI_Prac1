using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreTrigger : MonoBehaviour
{
    public TextMeshProUGUI scoreText; 
    private int score = 0;
    public float scoreIncreaseRate = 1f; 
    private bool playerInTrigger = false;

    private void Start()
    {
        UpdateScoreText();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

            playerInTrigger = true;
            StartCoroutine(IncreaseScoreOverTime());
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {

            playerInTrigger = false;
        
    }

    private IEnumerator IncreaseScoreOverTime()
    {
        while (playerInTrigger)
        {
            score++;
            UpdateScoreText();
            yield return new WaitForSeconds(scoreIncreaseRate);
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}