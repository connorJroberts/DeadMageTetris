using System.Collections.Generic;
using UnityEngine;

public class ShapeSpawner : MonoBehaviour
{
    [SerializeField] Grid WorldGrid;
    [SerializeField] private Sprite _sprite;

    public string NextShape {  get; private set; }
    public Shape CurrentShape { get; private set; }

    private LevelData _levelData => GameManager.Instance.CurrentLevelData;
    List<ShapeData> shapeList = new List<ShapeData>();

    private void Start()
    {
        foreach (var shapeData in _levelData.Shapes)
        {
            if (shapeData.Enabled)
            {
                shapeList.Add(shapeData);
            }
        }

        NextShape = ChooseShapeFromWeight(shapeList);
        SpawnShape();

        GameManager.Instance.OnLevelFinished.AddListener(DisableSelf);
    }

    private void DisableSelf()
    {
        enabled = false;
    }

    private void EnableSelf()
    {
        enabled = true;
    }

    public void SpawnShape()
    {
        if (enabled == false) return;
        GameManager.Instance.StepSpeed();
        if (GameManager.Instance.Board.GetMaxHeight() >= BoardController.BOARDHEIGHT) return;
        GameObject shapeObject = new GameObject("CurrentShape");
        Shape shape = shapeObject.AddComponent<Shape>();
        if (CurrentShape != null ) Destroy(CurrentShape.gameObject);
        CurrentShape = shape;
        shape.WorldGrid = WorldGrid;
        shape.SetSprite(_sprite);
        shape.ConfigureShape(NextShape);
        shape.OnPlaced.AddListener(SpawnShape);
        NextShape = ChooseShapeFromWeight(shapeList);
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
            if (randomNumber > weightBoundaries[i]) return shapeList[i].ShapeString;
        }
        return shapeList[0].ShapeString;
    }

}
