using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] drones;
    [SerializeField] Button nextButton;
    [SerializeField] Button rotateButton;
    [SerializeField] Button resetButton;

    private bool setupComplete = false;
    private bool playerTurn = true;
    private int droneIndex = 0;
    private DroneScript drone;

    // Start is called before the first frame update
    void Start()
    {
        drone = drones[droneIndex].GetComponent<DroneScript>();
        nextButton.onClick.AddListener(() => NextDroneClicked());
        rotateButton.onClick.AddListener(() => RotateClicked());
        resetButton.onClick.AddListener(() => ResetClicked());
    }

    void NextDroneClicked()
    {
        if (droneIndex <= drones.Length - 2)
        {
            droneIndex++;
            drone = drones[droneIndex].GetComponent<DroneScript>();
            // drone.flashColor(Color.yellow);
        }
    }

    void RotateClicked()
    {
        drone.RotateClicked();
    }

    void ResetClicked()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; 
        SceneManager.LoadScene(currentSceneIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TileChecked(GameObject tile)
    {
        if (setupComplete && playerTurn)
        {
            //TODO: Missile attack
        }
        else if (!setupComplete)
        {
            PlaceDrone(tile);
            drone.SetClickedTile(tile);
        }
    }

    private void PlaceDrone(GameObject tile)
    {
        drone = drones[droneIndex].GetComponent<DroneScript>();
        drone.ClearTileList();
        Vector3 droneVector = drone.GetOffsetVector(tile.transform.position);
        drones[droneIndex].transform.localPosition = droneVector;
    }
}
