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
    [Header("Drones")]
    public GameObject[] drones;
    public EnemyScript enemyScript;
    private DroneScript drone;
    private List<int[]> enemyDrones;
    private int droneIndex = 0;
    private List<TileManager> allTileManager;

    [Header("HUD")]
    [SerializeField] Button nextButton;
    [SerializeField] Button rotateButton;
    [SerializeField] Button resetButton;
    [SerializeField] TextMeshProUGUI topDroneText;
    [SerializeField] TextMeshProUGUI playerDroneText;
    [SerializeField] TextMeshProUGUI enemyDroneText;

    [Header("Objects")]
    [SerializeField] GameObject missilePrefab;
    [SerializeField] GameObject enemyMissilePrefab;
    [SerializeField] GameObject firePrefab;

    private bool setupComplete = false;
    private bool playerTurn = true;
    private List<GameObject> playerFires = new List<GameObject>();
    private List<GameObject> enemyFires = new List<GameObject>();

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
        if (!drone.OnGameBoard())
        {
            drone.FlashColor(Color.red);
        }
        else
        {
            if (droneIndex <= drones.Length - 2)
            {
                droneIndex++;
                drone = drones[droneIndex].GetComponent<DroneScript>();
                drone.FlashColor(Color.yellow);
            }
            else
            {
                nextButton.gameObject.SetActive(false);
                rotateButton.gameObject.SetActive(false);
                resetButton.gameObject.SetActive(false);
                topDroneText.text = "Choose a tile to strike";
                setupComplete = true;
                for (int i = 0; i < drones.Length; i++)
                {
                    drones[i].SetActive(false);
                }
            }
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
            ParticleSystem laserParticle = missilePrefab.GetComponentInChildren<ParticleSystem>();
            Vector3 tilePosition = tile.transform.position;
            tilePosition.y += 25;
            playerTurn = false;
            missilePrefab.transform.position = tilePosition;
            laserParticle.Play();
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
                    enemyFires.Add(Instantiate(firePrefab, tile.transform.position, Quaternion.identity));
                    tile.GetComponent<TileManager>().SetTileColor(1, new Color32(68, 0, 0, 255));
                    tile.GetComponent<TileManager>().SwitchColors(1);
                }
                else
                {
                    topDroneText.text = "Highlighted";
                    tile.GetComponent<TileManager>().SetTileColor(1, new Color32(255, 0, 0, 255));
                    tile.GetComponent<TileManager>().SwitchColors(1);
                }
            }
        }
        Debug.Log("tileNum");
        if(hitCount == 0)
        {
            tile.GetComponent<TileManager>().SetTileColor(1, new Color32(38, 57, 76, 255));
            tile.GetComponent<TileManager>().SwitchColors(1);
            topDroneText.text = "Missed";
        }
        Invoke(nameof(EndPlayerMove), 2.0f);
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
        Invoke(nameof(EndEnemyMove), 2.0f);
    }

    private void EndPlayerMove()
    {
        for (int i = 0; i < drones.Length; i++)
        {
            drones[i].SetActive(true);
        }
        foreach (GameObject fire in playerFires)
        {
            fire.SetActive(true);
        }
        foreach (GameObject fire in enemyFires)
        {
            fire.SetActive(false);
        }
        enemyDroneText.text = enemyDroneCount.ToString();
        enemyScript.NPCTurn();
        ColorAllTiles(0);
        if (playerDroneCount < 1) GameOver("You win!");
    }

    public void EndEnemyMove()
    {
        for (int i = 0; i < drones.Length; i++)
        {
            drones[i].SetActive(false);
        }
        foreach (GameObject fire in playerFires)
        {
            fire.SetActive(false);
        }
        foreach (GameObject fire in enemyFires)
        {
            fire.SetActive(true);
        }
        playerDroneText.text = playerDroneCount.ToString();
        enemyScript.NPCTurn();
        ColorAllTiles(0);
        if (enemyDroneCount < 1) GameOver("The enemy wins!");
    }

    private void ColorAllTiles(int colorIndex)
    {
        foreach (TileManager tileManager in allTileManager)
        {
            tileManager.SwitchColors(colorIndex);
        }
    }

    void GameOver(string winner)
    {
        topDroneText.text = "Game over" + winner;
        resetButton.gameObject.SetActive(true);
    }
}
