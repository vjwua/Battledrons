using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] drones;

    [Header("HUD")]
    [SerializeField] Button nextButton;
    [SerializeField] Button rotateButton;
    [SerializeField] Button resetButton;
    [SerializeField] TextMeshProUGUI topDroneText;
    [SerializeField] TextMeshProUGUI playerDroneText;
    [SerializeField] TextMeshProUGUI enemyText;

    [Header("Objects")]
    [SerializeField] GameObject missilePrefab;
    [SerializeField] GameObject enemyMissilePrefab;
    [SerializeField] GameObject firePrefab;
    //[SerializeField] GameObject woodDeck;

    private bool setupComplete = false;
    private bool playerTurn = true;
    private int droneIndex = 0;
    private DroneScript drone;
    public EnemyScript enemyScript;
    private List<int[]> enemyDrones;
    private List<GameObject> playerFires;

    private int enemyDroneCount = 5;
    private int playerDroneCount = 5;

    // Start is called before the first frame update
    void Start()
    {
        drone = drones[droneIndex].GetComponent<DroneScript>();
        enemyDrones = enemyScript.PlaceEnemyDrones();
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
        else
        {
            enemyScript.PlaceEnemyDrones();
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

    public void CheckHit(GameObject tile)
    {
        int tileNum = Int32.Parse(Regex.Match(tile.name, @"\d+").Value);
        int hitCount = 0;
        foreach(int [] tileNumArray in enemyDrones)
        {
            if(tileNumArray.Contains(tileNum))
            {
                for (int i = 0; i > tileNumArray.Length; i++)
                {
                    if(tileNumArray[i] == tileNum)
                    {
                        tileNumArray[i] = -5;
                        hitCount++;
                    }
                    else if(tileNumArray[i] == -5)
                    {
                        hitCount++;
                    }
                }
                if (hitCount == tileNumArray.Length)
                {
                    enemyDroneCount--;
                    topDroneText.text = "Fallen";
                }
                else
                {
                    topDroneText.text = "Highlighted";
                }
            }
        }
        Debug.Log("tileNum");
        if(hitCount == 0)
        {
            topDroneText.text = "Missed";
            //Invoke("EndPlayerMove", 1.0f)
            //or
            //StartCoroutine(EndPlayerMove());
        }
    }

    public void EnemyHitPlayer(Vector3 tile, int tileNum, GameObject gameObject)
    {
        enemyScript.MissileHit(tileNum);
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        playerFires.Add(Instantiate(firePrefab, tile, Quaternion.identity));
        if (gameObject.GetComponent<DroneScript>().HitCheckFallen())
        {
            playerDroneCount--;
            playerDroneText.text = playerDroneCount.ToString();
            enemyScript.FallenPlayer();
        }
        //Invoke("EndEnemyMove", 2.0f)
        //or
        //StartCoroutine(EndEnemyMove());
    }
}
