using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    

    [SerializeField] private List<LevelDataAsset> _levels;
    [field: SerializeField] public BoardController Board { get; private set; }
    [field: SerializeField] public ShapeSpawner ShapeSpawner { get; private set; }
    [field: SerializeField] public Grid WorldGrid { get; private set; }

    public static GameManager Instance { get; private set; }
    public int CurrentLevel { get; private set; } = 0;
    public LevelData CurrentLevelData;
    public int CurrentScore { get; private set; } = 0;
    public float CurrentDropSpeed { get; private set; }

    public UnityEvent<int> OnScoreChanged;
    public UnityEvent OnLevelFinished;

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
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        CurrentLevelData = _levels[CurrentLevel].LevelData;

    }

    // Start is called before the first frame update
    void Start()
    {
        OnScoreChanged.AddListener(CheckLevelComplete);

        CurrentDropSpeed = CurrentLevelData.SpeedMin;
    }

    private void CheckLevelComplete(int score)
    {
        if (score >= CurrentLevelData.WinningScore)
        {
            CurrentLevel++;
            CurrentLevelData = _levels[CurrentLevel].LevelData;
            Destroy(ShapeSpawner.CurrentShape.gameObject);
            Board.Clear();
            OnLevelFinished.Invoke();
            CurrentDropSpeed = CurrentLevelData.SpeedMin;
            CurrentScore = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
