using System;
using System.Collections;
using UnityEngine;

public class ShapeMove : MonoBehaviour
{
    private LevelData _levelData => GameManager.Instance.CurrentLevelData;

    private Shape _shape;

    private IEnumerator _currentHorizontalBufferRoutine;
    private IEnumerator _currentDownBufferRoutine;
    private IEnumerator _currentRotateBufferRoutine;

    private void Start()
    {
        TryGetComponent(out _shape);
        StartCoroutine(ShapeDropRoutine());
    }

    private IEnumerator ShapeDropRoutine()
    {
        do
        {
            if (_currentDownBufferRoutine == null)
            {
                transform.position -= new Vector3(0, GameManager.Instance.WorldGrid.cellSize.y, 0);
            }
            yield return new WaitForSecondsRealtime(1 / GameManager.Instance.CurrentDropSpeed);

        } while (_shape.CheckForShapePlace() == false);
    }

    private void Update()
    {
        // This section handles tapped button input
        if (Input.GetButtonDown("Drop") == true)
        {
            OnDrop();
        }
        else if (Input.GetButtonDown("Horizontal") == true)
        {
            OnHorizontal(Input.GetAxisRaw("Horizontal"));
            if (_currentHorizontalBufferRoutine != null)
            {
                StopCoroutine(_currentHorizontalBufferRoutine);
            }
            _currentHorizontalBufferRoutine = InputHoldBufferRoutine(delegate
            {
                _currentHorizontalBufferRoutine = null;
            }, 0.5f);
            StartCoroutine(_currentHorizontalBufferRoutine);
        }
        else if (Input.GetButtonDown("Down") == true)
        {
            OnDown();
            if (_currentDownBufferRoutine != null)
            {
                StopCoroutine(_currentDownBufferRoutine);
            }
            _currentDownBufferRoutine = InputHoldBufferRoutine(delegate
            {
                _currentDownBufferRoutine = null;
            }, 0.5f);
            StartCoroutine(_currentDownBufferRoutine);
        }
        else if (Input.GetButtonDown("Rotate") == true)
        {
            OnRotate();
            if (_currentRotateBufferRoutine != null)
            {
                StopCoroutine(_currentRotateBufferRoutine);
            }
            _currentRotateBufferRoutine = InputHoldBufferRoutine(delegate
            {
                _currentRotateBufferRoutine = null;
            }, 0.5f);
            StartCoroutine(_currentRotateBufferRoutine);
        }
        // This section handles holding button input
        else if (Input.GetButton("Horizontal") == true && _currentHorizontalBufferRoutine == null)
        {
            _currentHorizontalBufferRoutine = InputHoldBufferRoutine( delegate 
            { 
                OnHorizontal(Input.GetAxisRaw("Horizontal"));
                _currentHorizontalBufferRoutine = null;
            });
            StartCoroutine(_currentHorizontalBufferRoutine);
        }
        else if (Input.GetButton("Down") == true && _currentDownBufferRoutine == null)
        {
            _currentDownBufferRoutine = InputHoldBufferRoutine(delegate
            {
                OnDown();
                _currentDownBufferRoutine = null;
            });
            StartCoroutine(_currentDownBufferRoutine);
        }
        else if (Input.GetButton("Rotate") == true && _currentRotateBufferRoutine == null)
        {
            _currentRotateBufferRoutine = InputHoldBufferRoutine(delegate
            {
                OnRotate();
                _currentRotateBufferRoutine = null;
            });
            StartCoroutine(_currentRotateBufferRoutine);
        }
    }

    public void OnHorizontal(float direction)
    {

        foreach (SpriteRenderer block in _shape.Blocks)
        {
            Vector3Int blockPosition = _shape.WorldGrid.WorldToCell(block.transform.position);

            if (blockPosition.x + direction >= BoardController.BOARDWIDTH)
            {
                return;
            }
            if (blockPosition.x + direction < 0)
            {
                return;
            }
            if (GameManager.Instance.Board.BoardState[blockPosition.x + (int)direction, blockPosition.y] == 1)
            {
                return;
            }

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
                transform.position += new Vector3(_shape.WorldGrid.cellSize.x, 0, 0);
            }
            else if (blockPosition.x >= BoardController.BOARDWIDTH)
            {
                transform.position -= new Vector3(_shape.WorldGrid.cellSize.x, 0, 0);
            }
        }
    }

    private IEnumerator InputHoldBufferRoutine(Action action = null, float time = 0.1f)
    {
        yield return new WaitForSecondsRealtime(time);
        action();
    }

}
