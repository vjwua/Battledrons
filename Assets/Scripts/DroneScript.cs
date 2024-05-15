using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneScript : MonoBehaviour
{
    [SerializeField] float xOffset = 0;
    [SerializeField] float zOffset = 0;
    float nextYRotation = 90;
    GameObject clickedTile;
    int hitCount = 0;
    public int droneSize;

    private Material[] allMaterials;

    List<GameObject> touchTiles = new List<GameObject>();
    List<Color> allColors = new List<Color>();

    private void Start()
    {
        allMaterials = GetComponentInChildren<Renderer>().materials;
        for (int i = 0; i < allMaterials.Length; i++)
        {
            allColors.Add(allMaterials[i].color);
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        //Debug.Log($"{other.gameObject.name} and {other.gameObject.transform.parent.name}");
        if(other.gameObject.CompareTag("Tile"))
        {
            touchTiles.Add(other.gameObject);
        }
    }

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
        if (clickedTile == null) { return; }
        ClearTileList();
        transform.eulerAngles += new Vector3(0, nextYRotation, 0);
        nextYRotation *= -1;
        (xOffset, zOffset) = (zOffset, xOffset); //Swap
        SetPosition(clickedTile.transform.position);
    }

    public void SetPosition(Vector3 newVector)
    {
        ClearTileList();
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

    public void FlashColor(Color tempColor)
    {
        foreach (Material material in allMaterials)
        {
            material.color = tempColor;
        }
        Invoke(nameof(ResetColor), 0.5f);
    }

    public void ResetColor()
    {
        int i = 0;
        foreach (Material material in allMaterials)
        {
            material.color = allColors[i++];
        }
    }
}
