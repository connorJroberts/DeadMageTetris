using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnScoreChanged.AddListener(UpdateScore);
        GameManager.Instance.OnLevelFinished.AddListener(ResetScore);
        GameManager.Instance.OnLevelLose.AddListener(ResetScore);
    }

    private void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score.ToString();
    }

    private void ResetScore()
    {
        UpdateScore(0);
    }

}
