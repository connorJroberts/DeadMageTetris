using UnityEngine;

public class BoardController : MonoBehaviour
{
    public const int BOARDWIDTH = 10;
    public const int BOARDHEIGHT = 20;

    public Vector2Int BoardDimensions => new Vector2Int(BOARDWIDTH, BOARDHEIGHT);

    public int[,] BoardState = new int[BOARDWIDTH, BOARDHEIGHT + 3];
    public SpriteRenderer[,] BoardSprites = new SpriteRenderer[BOARDWIDTH, BOARDHEIGHT + 3]; // + 3 for spawning room at top of screen

    //Max height of column
    public int GetColumnHeight(int x)
    {
        int height = 0;
        for (int y = 0; y < BOARDHEIGHT; y++)
        {
            if (BoardState[x, y] == 0)
            {
                continue;
            }
            if (BoardState[x,y] == 1)
            {
                height = y + 1;
            }
        }
        return height;
    }

    //Number of blocks within row
    public int GetRowFill(int y)
    {
        int fill = 0;
        for (int x = 0; x < BOARDWIDTH; x++)
        {
            if (BoardState[x, y] == 0)
            {
                continue;
            }
            if (BoardState[x, y] == 1)
            {
                fill++;
            }
        }
        return fill;
    }

    public int GetMaxHeight()
    {
        int maxHeight = 0;
        for (int x = 0; x < BOARDWIDTH; x++)
        {
            int currentHeight = GetColumnHeight(x);
            if (currentHeight > maxHeight)
            {
                maxHeight = currentHeight;
            }
        }
        return maxHeight;
    }

    public void RemoveRowAbsolute(int y)
    {
        BoardState = RemoveRow(y, BoardState);
        BoardSprites = RemoveRow(y, BoardSprites);
        GameManager.Instance.AddScore(1);
    }

    public T[,] RemoveRow<T>(int y, T[,] board)
    {
        T[,] tempBoard = (T[,])board.Clone();

        for (int h = y; h < BOARDHEIGHT; h++)
        {
            for (int x = 0; x < BOARDWIDTH; x++)
            {
                if (h == y)
                {
                    var block = (tempBoard[x, h] as Component);
                    if (block != null) Destroy(block.gameObject);
                }
                if (h >= y && h + 1 < BOARDHEIGHT)
                {
                    try
                    {
                        tempBoard[x, h] = board[x, h + 1];
                        Component block = tempBoard[x, h] as Component;
                        block.transform.position = new Vector3(block.transform.position.x, block.transform.position.y - GameManager.Instance.WorldGrid.cellSize.y, 0);
                    }
                    catch
                    {
                        tempBoard[x, h] = board[x, h + 1];
                    }
                }
                else if (h >= y && h + 1 >= BOARDHEIGHT)
                {
                    try
                    {
                        Destroy((tempBoard[x, h] as Component).gameObject);
                        tempBoard[x, h] = default;
                    }
                    catch
                    {
                        tempBoard[x, h] = default;
                    }
                }

            }
        }

        return tempBoard;
    }

    public void Clear()
    {
        BoardState = new int[BOARDWIDTH, BOARDHEIGHT + 3];
        foreach (SpriteRenderer sprite in BoardSprites)
        {
            if (sprite == null)
            {
                continue;
            }
            Destroy(sprite.gameObject);
        }
        BoardSprites = new SpriteRenderer[BOARDWIDTH, BOARDHEIGHT + 3];
    }
    public void PrintBoard()
    {
        string boardStateString = "\n";

        for (int y = 0; y < BOARDHEIGHT; y++)
        {
            for (int x = 0; x < BOARDWIDTH; x++)
            {
                boardStateString += BoardState[x, y].ToString();
            }
            boardStateString += "\n";
        }
        Debug.Log(boardStateString);
    }

    public void CheckAllRowsForRemoval()
    {
        for (int y = 0; y < BOARDHEIGHT; y++)
        {
            if (CheckRemoveRow(y) == true)
            {
                CheckAllRowsForRemoval();
            }
        }
    }
    public bool CheckRemoveRow(int y)
    {
        if (GetRowFill(y) == BOARDWIDTH)
        {
            RemoveRowAbsolute(y);
            return true;
        }
        return false;
    }

}
