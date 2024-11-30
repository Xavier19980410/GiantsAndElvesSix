using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseChessman : MonoBehaviour
{
    public List<Vector3> pathPoints;
    public int rotationSpeed = 5;
    public int speed = 5;
    public int currentPointIndex = 0;

    private bool startMove;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            this.transform.position = HexagonManager.Instance[0, 0].GetPos();
            Vector2Int thisIndex = HexagonManager.Instance.GetHexagonTileByPos(this.transform.position);
            Vector2Int tarIndex = HexagonManager.Instance.GetHexagonTileByPos(HexagonManager.Instance[4, 3].GetPos());
            Astar astar = new Astar();
            pathPoints = astar.FindPath(thisIndex, tarIndex);
            HexagonManager.Instance[0, 0].hasTile = true;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            startMove = true;
        }

        if (startMove)
        {
            Vector3 targetPoint = pathPoints[currentPointIndex];

            Vector3 direction = (targetPoint - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            transform.position = Vector3.MoveTowards(transform.position, targetPoint, Time.deltaTime * speed);

            if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
            {
                CheckOccupation(currentPointIndex);
                currentPointIndex++;
                if(currentPointIndex >= pathPoints.Count)
                {
                    startMove = false;
                    currentPointIndex = 0;
                }
            }
        }
    }

    private void CheckOccupation(int currentPointIndex)
    {
        Vector2Int nextIndex = HexagonManager.Instance.GetHexagonTileByPos(pathPoints[currentPointIndex + 1]);
        if (HexagonManager.Instance[nextIndex.x, nextIndex.y].hasTile)
        {
            print($"CheckOccupation this:{currentPointIndex} nextHas:true");
        }
        else
        {
            print($"CheckOccupation this:{currentPointIndex} nextHas:false");
        }
    }

    public void StartFight()
    {

    }

    public void ChangeAnimation(string animationName)
    {

    }
}
