using System.Collections;
using UnityEngine;

public class ShapeMove : MonoBehaviour
{
    private LevelData _levelData => GameManager.Instance.CurrentLevelData;

    private Shape _shape;

    private void Start()
    {
        TryGetComponent(out _shape);
        StartCoroutine(ShapeDropRoutine());
    }

    private IEnumerator ShapeDropRoutine()
    {
        do
        {
            transform.position -= new Vector3(0, GameManager.Instance.WorldGrid.cellSize.y, 0);
            yield return new WaitForSecondsRealtime(1 / GameManager.Instance.CurrentDropSpeed);

        } while (_shape.CheckForShapePlace() == false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Horizontal")) OnHorizontal(Input.GetAxisRaw("Horizontal"));
        else if (Input.GetButtonDown("Down")) OnDown();
        else if (Input.GetButtonDown("Drop")) OnDrop();
        else if (Input.GetButtonDown("Rotate")) OnRotate();
    }

    public void OnHorizontal(float direction)
    {

        foreach (SpriteRenderer block in _shape.Blocks)
        {
            Vector3Int blockPosition = _shape.WorldGrid.WorldToCell(block.transform.position);

            if (blockPosition.x + direction >= BoardController.BOARDWIDTH) return;
            if (blockPosition.x + direction < 0 ) return;
            if (GameManager.Instance.Board.BoardState[blockPosition.x + (int)direction, blockPosition.y] == 1) return;

        }
        transform.position += new Vector3(direction * _shape.WorldGrid.cellSize.x, 0, 0);
    }

    public void OnDown()
    {
        transform.position -= new Vector3(0, _shape.WorldGrid.cellSize.y, 0);
        _shape.CheckForShapePlace();
    }

    public void OnDrop()
    {
        while (_shape.CheckForShapePlace() == false)
        {
            transform.position -= new Vector3(0, _shape.WorldGrid.cellSize.y, 0);
        } 
    }

    public void OnRotate()
    {
        transform.Rotate(new Vector3(0, 0, 90));
        foreach (SpriteRenderer block in _shape.Blocks)
        {
            Vector3Int blockPosition = GameManager.Instance.WorldGrid.WorldToCell(block.transform.position);
            if (blockPosition.x < 0)
            {
                transform.position -= new Vector3(_shape.WorldGrid.cellSize.x, 0, 0);
                break;
            }
            else if (blockPosition.x > BoardController.BOARDWIDTH)
            {
                transform.position -= new Vector3(_shape.WorldGrid.cellSize.x, 0, 0);
                break;
            }
        }
    }

}
