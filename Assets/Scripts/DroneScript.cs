using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneScript : MonoBehaviour
{
    List<GameObject> touchTiles = new List<GameObject>();
    [SerializeField] float xOffset = 0;
    [SerializeField] float zOffset = 0;
    float nextYRotation = 90;
    GameObject clickedTile;
    int hitCount = 0;
    int droneSize;

    public void ClearTileList()
    {
        touchTiles.Clear();
    }

    public Vector3 GetOffsetVector(Vector3 tilePosition)
    {
        return new Vector3(tilePosition.x + xOffset, 2, tilePosition.z + zOffset);
    }

    public void RotateClicked()
    {
        ClearTileList();
        transform.eulerAngles += new Vector3(0, nextYRotation, 0);
        nextYRotation *= -1;
        (xOffset, zOffset) = (zOffset, xOffset); //Swap
        SetPosition(clickedTile.transform.position);
    }

    public void SetPosition(Vector3 newVector)
    {
        transform.localPosition = new Vector3(newVector.x + xOffset, 2, newVector.z + zOffset);
    }

    public void SetClickedTile(GameObject tile)
    {
        clickedTile = tile;
    }

    public bool OnGameBoard()
    {
        return touchTiles.Count == droneSize;
    }

    public bool HitCheckFallen()
    {
        hitCount++;
        return droneSize <= hitCount;
    }
}
