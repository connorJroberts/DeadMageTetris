using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShapeSpawner : MonoBehaviour
{
    [SerializeField] Grid WorldGrid;
    [SerializeField] private Sprite _sprite;

    public string NextShape {  get; private set; }
    public Shape CurrentShape { get; private set; }

    List<ShapeData> shapeList = new List<ShapeData>();

    public UnityEvent<string> OnShapeSpawned;

    private void Start()
    {
        foreach (var shapeData in GameManager.Instance.CurrentLevelData.Shapes)
        {
            if (shapeData.Enabled == true)
            {
                shapeList.Add(shapeData);
            }
        }

        NextShape = ChooseShapeFromWeight(shapeList);
        SpawnShape();

        GameManager.Instance.OnLevelFinished.AddListener(DisableSelf);
        GameManager.Instance.OnLastLevelFinished.AddListener(DisableSelf);
        GameManager.Instance.OnLevelStarted.AddListener(EnableSelf);
    }

    private void DisableSelf()
    {
        enabled = false;
    }

    private void EnableSelf()
    {
        enabled = true;
        shapeList.Clear();
        foreach (var shapeData in GameManager.Instance.CurrentLevelData.Shapes)
        {
            if (shapeData.Enabled == true)
            {
                shapeList.Add(shapeData);
            }
        }
        SpawnShape();
    }

    public void SpawnShape()
    {
        if (enabled == false)
        {
            return;
        }
        if (GameManager.Instance.Board.GetMaxHeight() >= BoardController.BOARDHEIGHT)
        {
            GameManager.Instance.OnLevelLose.Invoke();
            DisableSelf();
            return;
        }
        if (CurrentShape != null)
        {
            Destroy(CurrentShape.gameObject);
        }
        GameManager.Instance.StepSpeed();
        GameObject shapeObject = new GameObject("CurrentShape");
        Shape shape = shapeObject.AddComponent<Shape>();
        CurrentShape = shape;
        shape.WorldGrid = WorldGrid;
        shape.SetSprite(_sprite);
        shape.ConfigureShape(NextShape);
        shape.OnPlaced.AddListener(SpawnShape);
        NextShape = ChooseShapeFromWeight(shapeList);
        OnShapeSpawned.Invoke(NextShape);
    }

    public string ChooseShapeFromWeight(List<ShapeData> shapeList)
    {
        float weightSum = 0;
        List<float> weightBoundaries = new List<float>();
        shapeList.ForEach(delegate (ShapeData shape)
            {
                weightSum += shape.DropWeighting;
                weightBoundaries.Add(weightSum);
            });
        
        float randomNumber = Random.Range(0, weightSum);

        for (int i = weightBoundaries.Count - 1; i >= 0; i--) {
            if (randomNumber > weightBoundaries[i])
            {
                return shapeList[i].ShapeString;
            }
        }
        return shapeList[0].ShapeString;
    }

}
