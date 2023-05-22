using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pikachu
{
    public class PikachuManager : MonoBehaviour
    {
        public static PikachuManager instance;

        [SerializeField] private Vector3 offset;
        [SerializeField] private bool autoOffset;
        [Tooltip("Product of x and y must be divisible by 2 and number allType after that divide by 2")]
        [SerializeField] private Vector2Int gameSize;
        [SerializeField] private Color[] allType;
        [SerializeField] private PikachuCell cellPrefab;
        [SerializeField] private Transform cellContainer;
        [SerializeField] private LineRenderer lineRenderer;

        private int cellsCount;
        private int[] allTypeCount;
        private Vector3 cellSize;
        private PikachuCell[,] cellArray;
        private PikachuCell cellChoosed1;
        private PikachuCell cellChoosed2;

        private int step = 0;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            cellsCount=gameSize.x*gameSize.y;
            List<int> avalableType = new List<int>();
            allTypeCount = new int[allType.Length];
            for (int i = 0; i < allTypeCount.Length; i++)
            {
                allTypeCount[i] = gameSize.x * gameSize.y / allType.Length;
                avalableType.Add(i);
            }

            if (autoOffset)
            {
                offset = new Vector3(-gameSize.x / 20f * 10.25f*cellPrefab.transform.localScale.x, -gameSize.y / 10f * 5.3f* cellPrefab.transform.localScale.y);
            }

            Vector2 cellSpriteSize= cellPrefab.GetComponent<SpriteRenderer>().size;
            cellSize = new Vector3(cellSpriteSize.x * cellPrefab.transform.localScale.x, cellSpriteSize.y * cellPrefab.transform.localScale.y);

            cellArray = new PikachuCell[gameSize.x + 2, gameSize.y + 2];
            for (int i = 1; i < gameSize.x + 1; i++)
                for (int j = 1; j < gameSize.y + 1; j++)
                {
                    int rdType = avalableType[(Random.Range(0, avalableType.Count))];
                    allTypeCount[rdType]--;

                    var tempCell = Instantiate(cellPrefab);
                    tempCell.Init(new Vector3(i*cellSize.x,j*cellSize.y) + offset, allType[rdType], cellContainer, new Vector2Int(i, j));
                    cellArray[i, j] = tempCell;

                    if (allTypeCount[rdType] == 0)
                        avalableType.Remove(rdType);
                }
        }

        public void ChooseOneCell(PikachuCell cell)
        {
            if (cellChoosed1 == null)
            {
                cellChoosed1 = cell;
            }
            else if (cellChoosed2 == null)
            {
                cellChoosed2 = cell;

                if (cellChoosed1 == cellChoosed2)
                    ClearChoose();
                else
                    Check();
            }
        }

        public void ClearChoose()
        {
            cellChoosed1.ResetCell();
            cellChoosed2.ResetCell();
            cellChoosed1 = null;
            cellChoosed2 = null;
        }

        private void Check()
        {

            if (cellChoosed1.type == cellChoosed2.type)
            {
                step = 0;
                if (cellChoosed1.index.x == cellChoosed2.index.x)
                {
                    if (CheckLineX(cellChoosed1.index.y, cellChoosed2.index.y, cellChoosed2.index.x))
                    {
                        Vector3[] points = new Vector3[2];
                        points[0] = new Vector3(cellChoosed1.index.x, cellChoosed1.index.y);
                        points[1] = new Vector3(cellChoosed2.index.x, cellChoosed2.index.y);
                        DrawLine(points);

                        Success();
                        return;
                    }
                }
                if (cellChoosed1.index.y == cellChoosed2.index.y)
                {
                    if (CheckLineY(cellChoosed1.index.x, cellChoosed2.index.x, cellChoosed2.index.y))
                    {
                        Vector3[] points = new Vector3[2];
                        points[0] = new Vector3(cellChoosed1.index.x, cellChoosed1.index.y);
                        points[1] = new Vector3(cellChoosed2.index.x, cellChoosed2.index.y);
                        DrawLine(points);

                        Success();
                        return;
                    }
                }
                // check in rectangle with x
                step = 1;
                if ((CheckRectX(cellChoosed1.index, cellChoosed2.index)) != -1)
                {
                    //Debug.Log("Step = "+step);
                    Success();
                    return;
                }
                step = 2;
                // check in rectangle with y
                if ((CheckRectY(cellChoosed1.index, cellChoosed2.index)) != -1)
                {
                    //Debug.Log("Step = " + step);
                    Success();
                    return;
                }
                step = 3;
                // check more right
                if ((CheckMoreLineX(cellChoosed1.index, cellChoosed2.index, 1)) != -1)
                {
                    //Debug.Log("Step = " + step);
                    Success();
                    return;
                }
                step = 4;
                // check more left
                if ((CheckMoreLineX(cellChoosed1.index, cellChoosed2.index, -1)) != -1)
                {
                    //Debug.Log("Step = " + step);
                    Success();
                    return;
                }
                step = 5;
                // check more down
                if ((CheckMoreLineY(cellChoosed1.index, cellChoosed2.index, 1)) != -1)
                {
                    //Debug.Log("Step = " + step);
                    Success();
                    return;
                }
                step = 6;
                // check more up
                if ((CheckMoreLineY(cellChoosed1.index, cellChoosed2.index, -1)) != -1)
                {
                    //Debug.Log("Step = " + step);
                    Success();
                    return;
                }

            }

            ClearChoose();
        }

        private void Success()
        {
            Destroy(cellChoosed1.gameObject);
            Destroy(cellChoosed2.gameObject);

            cellsCount -= 2;
            if (cellsCount <= 0)
                WinGame();
        }

        private void WinGame()
        {
            Debug.Log("Win game");
        }    

        private void DrawLine(Vector3[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = offset + new Vector3(points[i].x*cellSize.x, points[i].y * cellSize.y);
            }
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
            Invoke("DestroyLine", 0.5f);
        }

        private void DestroyLine()
        {
            lineRenderer.positionCount = 0;
        }

        private bool CheckLineX(int y1, int y2, int x)
        {
            int min = Mathf.Min(y1, y2);
            int max = Mathf.Max(y1, y2);
            for (int y = min; y <= max; y++)
            {
                if (cellArray[x, y] != null)
                {
                    if (cellArray[x, y] != cellChoosed1 && cellArray[x, y] != cellChoosed2)
                        return false;
                }
            }

            return true;
        }
        private bool CheckLineY(int x1, int x2, int y)
        {
            int min = Mathf.Min(x1, x2);
            int max = Mathf.Max(x1, x2);
            for (int x = min; x <= max; x++)
            {
                if (cellArray[x, y] != null)
                {
                    if (cellArray[x, y] != cellChoosed1 && cellArray[x, y] != cellChoosed2)
                        return false;
                }
            }

            return true;
        }

        private int CheckRectX(Vector2Int p1, Vector2Int p2)
        {
            // find point have y min and max
            Vector2Int pMinY = p1, pMaxY = p2;
            if (p1.y > p2.y)
            {
                pMinY = p2;
                pMaxY = p1;
            }
            for (int y = pMinY.y + 1; y < pMaxY.y; y++)
            {
                // check three line
                if (CheckLineX(pMinY.y, y, pMinY.x) && CheckLineY(pMinY.x, pMaxY.x, y) && CheckLineX(y, pMaxY.y, pMaxY.x))
                {

                    List<Vector3> tList = new List<Vector3>();
                    tList.Add(new Vector3(pMinY.x, pMinY.y));
                    tList.Add(new Vector3(pMinY.x, y));
                    tList.Add(new Vector3(pMaxY.x, y));
                    tList.Add(new Vector3(pMaxY.x, pMaxY.y));
                    Vector3[] points = new Vector3[tList.Count];
                    tList.CopyTo(points);
                    DrawLine(points);


                    return y;
                }
            }
            // have a line in three line not true then return -1
            return -1;
        }
        private int CheckRectY(Vector2Int p1, Vector2Int p2)
        {
            // find point have y min
            Vector2Int pMinX = p1, pMaxX = p2;
            if (p1.x > p2.x)
            {
                pMinX = p2;
                pMaxX = p1;
            }
            // find line and y begin
            for (int x = pMinX.x + 1; x < pMaxX.x; x++)
            {
                if (CheckLineY(pMinX.x, x, pMinX.y)
                        && CheckLineX(pMinX.y, pMaxX.y, x)
                        && CheckLineY(x, pMaxX.x, pMaxX.y))
                {

                    List<Vector3> tList = new List<Vector3>();
                    tList.Add(new Vector3(pMinX.x, pMinX.y));
                    tList.Add(new Vector3(x, pMinX.y));
                    tList.Add(new Vector3(x, pMaxX.y));
                    tList.Add(new Vector3(pMaxX.x, pMaxX.y));
                    Vector3[] points = new Vector3[tList.Count];
                    tList.CopyTo(points);
                    DrawLine(points);

                    return x;
                }
            }
            return -1;
        }

        private int CheckMoreLineX(Vector2Int p1, Vector2Int p2, int type)
        {
            // find point have y min
            Vector2Int pMinY = p1, pMaxY = p2;
            if (p1.y > p2.y)
            {
                pMinY = p2;
                pMaxY = p1;
            }
            // find line and y begin
            int y = pMaxY.y;
            int row = pMinY.x;
            if (type == -1)
            {
                y = pMinY.y;
                row = pMaxY.x;
            }
            // check more
            if (CheckLineX(pMinY.y, pMaxY.y, row))
            {
                while ((cellArray[pMinY.x, y] == null || cellArray[pMinY.x, y] == cellChoosed1 || cellArray[pMinY.x, y] == cellChoosed2)
                        && (cellArray[pMaxY.x, y] == null || cellArray[pMaxY.x, y] == cellChoosed1 || cellArray[pMaxY.x, y] == cellChoosed2))
                {
                    if (CheckLineY(pMinY.x, pMaxY.x, y))
                    {

                        List<Vector3> tList = new List<Vector3>();
                        tList.Add(new Vector3(pMinY.x, pMinY.y));
                        tList.Add(new Vector3(pMinY.x, y));
                        tList.Add(new Vector3(pMaxY.x, y));
                        tList.Add(new Vector3(pMaxY.x, pMaxY.y));
                        Vector3[] points = new Vector3[tList.Count];
                        tList.CopyTo(points);
                        DrawLine(points);

                        return y;
                    }
                    y += type;
                }
            }
            return -1;
        }

        private int CheckMoreLineY(Vector2Int p1, Vector2Int p2, int type)
        {
            Vector2Int pMinX = p1, pMaxX = p2;
            if (p1.x > p2.x)
            {
                pMinX = p2;
                pMaxX = p1;
            }
            int x = pMaxX.x;
            int col = pMinX.y;
            if (type == -1)
            {
                x = pMinX.x;
                col = pMaxX.y;
            }
            if (CheckLineY(pMinX.x, pMaxX.x, col))
            {
                while ((cellArray[x, pMinX.y] == null || cellArray[x, pMinX.y] == cellChoosed1 || cellArray[x, pMinX.y] == cellChoosed2)
                        && (cellArray[x, pMaxX.y] == null || cellArray[x, pMaxX.y] == cellChoosed1 || cellArray[x, pMaxX.y] == cellChoosed2))
                {
                    if (CheckLineX(pMinX.y, pMaxX.y, x))
                    {

                        List<Vector3> tList = new List<Vector3>();
                        tList.Add(new Vector3(pMinX.x, pMinX.y));
                        tList.Add(new Vector3(x, pMinX.y));
                        tList.Add(new Vector3(x, pMaxX.y));
                        tList.Add(new Vector3(pMaxX.x, pMaxX.y));
                        Vector3[] points = new Vector3[tList.Count];
                        tList.CopyTo(points);
                        DrawLine(points);

                        return x;
                    }
                    x += type;
                }
            }
            return -1;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(new Vector3(offset.x+cellSize.x/2,offset.y+cellSize.y/2), 0.2f);
            if (cellArray == null)
                return;
            Gizmos.color = Color.black;
            for (int i = 0; i < cellArray.GetLength(0); i++)
                for (int j = 0; j < cellArray.GetLength(1); j++)
                {
                    Gizmos.DrawWireSphere(new Vector3(i*cellSize.x, j*cellSize.y) + offset, 0.2f);
                }
        }

    }

}
