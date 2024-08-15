using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("Levels")]
    [SerializeField] private List<LevelDataAsset> _levels;

    [Header("Game Components")]
    [field: SerializeField] public BoardController Board { get; private set; }
    [field: SerializeField] public ShapeSpawner ShapeSpawner { get; private set; }
    [field: SerializeField] public Grid WorldGrid { get; private set; }

    [Header("Buttons")]
    [SerializeField] private Button _restart;
    [SerializeField] private Button _nextLevel;

    public static GameManager Instance { get; private set; }
    public LevelData CurrentLevelData;
    public int CurrentLevel { get; private set; } = 0;
    public int CurrentScore { get; private set; } = 0;
    public float CurrentDropSpeed { get; private set; }

    public UnityEvent<int> OnScoreChanged;
    public UnityEvent OnLevelFinished;
    public UnityEvent OnLastLevelFinished;
    public UnityEvent OnLevelStarted;
    public UnityEvent OnLevelLose;

    public void AddScore(int scoreGained)
    {
        CurrentScore += scoreGained;
        OnScoreChanged.Invoke(CurrentScore);
    }

    public void StepSpeed()
    {
        CurrentDropSpeed += CurrentLevelData.SpeedStep;
        CurrentDropSpeed = Mathf.Clamp(CurrentDropSpeed, CurrentLevelData.SpeedMin, CurrentLevelData.SpeedMax);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        CurrentLevelData = _levels[CurrentLevel].LevelData;

    }

    private void EnableRestartButton()
    {
        _restart.gameObject.SetActive(true);
    }
    private void EnableNextLevelButton()
    {
        _nextLevel.gameObject.SetActive(true);
    }
    private void DisableRestartButton()
    {
        _restart.gameObject.SetActive(false);
    }
    private void DisableNextLevelButton()
    {
        _nextLevel.gameObject.SetActive(false);
    }
    private void DisableButtons()
    {
        DisableNextLevelButton();
        DisableRestartButton();
    }

    void Start()
    {
        OnScoreChanged.AddListener(CheckLevelComplete);
        _restart.onClick.AddListener(RestartGame);
        _nextLevel.onClick.AddListener(NextLevel);

        OnLevelStarted.AddListener(DisableButtons);
        OnLevelLose.AddListener(EnableRestartButton);
        OnLevelFinished.AddListener(EnableNextLevelButton);
        OnLevelFinished.AddListener(EnableRestartButton);
        OnLastLevelFinished.AddListener(EnableRestartButton);

        CurrentDropSpeed = CurrentLevelData.SpeedMin;
    }

    private void RestartGame()
    {
        CurrentLevel = 0;
        CurrentLevelData = _levels[CurrentLevel].LevelData;
        CurrentDropSpeed = CurrentLevelData.SpeedMin;
        Board.Clear();
        CurrentScore = 0;
        OnLevelStarted.Invoke();
    }

    private void NextLevel()
    {
        Board.Clear();
        OnLevelStarted.Invoke();
    }

    private void CheckLevelComplete(int score)
    {
        if (score >= CurrentLevelData.WinningScore)
        {
            if (CurrentLevel < _levels.Count - 1)
            {
                CurrentLevel++;
                OnLevelFinished.Invoke();
            }
            else
            {
                OnLastLevelFinished.Invoke();
            }
            CurrentLevelData = _levels[CurrentLevel].LevelData;
            Destroy(ShapeSpawner.CurrentShape.gameObject);
            Board.Clear();
            CurrentDropSpeed = CurrentLevelData.SpeedMin;
            CurrentScore = 0;
        }
    }
}
