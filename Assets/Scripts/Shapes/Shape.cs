using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ShapeMove))]
public class Shape : MonoBehaviour
{
    [SerializeField] private Sprite _sprite;

    public List<SpriteRenderer> Blocks { get; private set; } = new List<SpriteRenderer>();
    public Grid WorldGrid;

    public UnityEvent OnPlaced = new UnityEvent();

    public void SetSprite(Sprite sprite)
    {
        _sprite = sprite;
    }

    public void ConfigureShape(string shapeString)
    {
        ParseShapeString(shapeString, out Vector2 pivotLocation, out int[,] shapeDefinition);
        Vector3 pivotOffset = Vector3.zero;
        for (int i = 0; i < shapeDefinition.GetLength(0); i++)
        {
            for (int j = 0; j < shapeDefinition.GetLength(1); j++)
            {
                if (shapeDefinition[i, j] == 0) continue;
                pivotOffset = new Vector3(pivotLocation.x - Mathf.Round(pivotLocation.x), pivotLocation.y - Mathf.Round(pivotLocation.y), 0);
                Vector3 spriteSpawnPosition = WorldGrid.CellToLocal(new Vector3Int(i - (int)(pivotLocation.x), j - (int)(pivotLocation.y), 0)) - pivotOffset;
                Transform spriteTransform = new GameObject("SpriteContainer").transform;
                spriteTransform.parent = transform;
                SpriteRenderer spriteRenderer = spriteTransform.AddComponent<SpriteRenderer>();
                Blocks.Add(spriteRenderer);
                spriteRenderer.sprite = _sprite;
                spriteTransform.position = new Vector2(spriteSpawnPosition.x, spriteSpawnPosition.y);
            }
        }

        transform.position = new Vector3(  -0.5f , -0.5f, 0) + pivotOffset; //Aligns with grid while ensuring pivot in centre of pivot block

    }

    public void ParseShapeString(string shapeString, out Vector2 pivotLocation, out int[,] shapeDefinition)
    {
        shapeDefinition = null;
        pivotLocation = Vector2Int.zero;
        int pivotCount = 0;

        var identifier = new StringReader(shapeString).ReadLine();
        shapeString = shapeString.Remove(0, identifier.Length + 2); //Remove name identifier for easier parse, +2 removes \n


        var blockSize = new StringReader(shapeString).ReadLine().Length;
        Vector2Int currentShapeIndex = Vector2Int.zero;

        shapeDefinition = new int[blockSize, blockSize];

        for (int i = 0; i < shapeString.Length; i++)
        {
            switch (shapeString[i]){
                case '_':
                    currentShapeIndex.x++;
                    break;

                case '\n':
                    currentShapeIndex.y++;
                    currentShapeIndex.x = 0;
                    break;

                case 'X':
                    shapeDefinition[currentShapeIndex.x, currentShapeIndex.y] = 1;
                    currentShapeIndex.x++;
                    break;

                case 'O':
                    shapeDefinition[currentShapeIndex.x, currentShapeIndex.y] = 1;
                    pivotLocation += currentShapeIndex;
                    pivotCount++;
                    currentShapeIndex.x++;
                    break;
            }
        }

        pivotLocation /= pivotCount; //This allows the averaging of pivot locations for squares

    }

    public bool CheckForShapePlace()
    {
        foreach (SpriteRenderer block in Blocks)
        {
            Vector3Int blockPosition3D = WorldGrid.WorldToCell(block.transform.position);
            Vector2Int blockPosition = new Vector2Int(blockPosition3D.x, blockPosition3D.y);
            if (blockPosition.y == 0 || (blockPosition.y - 1 < BoardController.BOARDHEIGHT && GameManager.Instance.Board.BoardState[blockPosition.x, blockPosition.y - 1] == 1))
            {
                ShapePlace();
                return true;
            }
        }
        return false;
    }

    public void ShapePlace()
    {
        foreach (SpriteRenderer block in Blocks)
        {
            Vector3Int blockPosition = WorldGrid.WorldToCell(block.transform.position);
            block.transform.parent = GameManager.Instance.Board.transform;
            GameManager.Instance.Board.BoardSprites[blockPosition.x, blockPosition.y] = block;
            GameManager.Instance.Board.BoardState[blockPosition.x, blockPosition.y] = 1;
        }
        GameManager.Instance.Board.CheckAllRowsForRemoval();
        GameManager.Instance.Board.PrintBoard();
        OnPlaced.Invoke();
        Destroy(gameObject);
    }

}
