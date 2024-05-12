using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(TextMeshPro))]
public class CoordinateLabeler : MonoBehaviour
{
    TextMeshPro coordinateLabel;
    Vector2Int coordinates = new Vector2Int();
    
    void Awake()
    {
        coordinateLabel = GetComponent<TextMeshPro>();
        coordinateLabel.enabled = false;
        DisplayCoordinates();
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            DisplayCoordinates();
            UpdateObjectsName();
            coordinateLabel.enabled = true;
        
        }
        ToggleLabels();
    }

    void ToggleLabels()
    {
        if (Input.GetKey(KeyCode.C))
        {
            coordinateLabel.enabled = !coordinateLabel.IsActive();
        }
    }

    private void DisplayCoordinates()
    {
        coordinates.x = Mathf.RoundToInt(transform.parent.position.x / UnityEditor.EditorSnapSettings.move.x);
        coordinates.y = Mathf.RoundToInt(transform.parent.position.z / UnityEditor.EditorSnapSettings.move.z);
        coordinateLabel.text = $"{coordinates.x}, {coordinates.y}";
    }

    void UpdateObjectsName()
    {
        transform.parent.name = coordinates.ToString();
    }
}
