using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    GameManager gameManager;
    EnemyScript enemyScript;
    Ray ray;
    RaycastHit hit;
    private bool missileHit = false;
    private int targetTile = -1;
    Color[] hitColor = new Color[2];

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        enemyScript = FindObjectOfType<EnemyScript>();
        hitColor[0] = gameObject.GetComponent<MeshRenderer>().material.color;
        hitColor[1] = gameObject.GetComponent<MeshRenderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            if(Input.GetMouseButtonDown(0) && hit.collider.gameObject.name == gameObject.name)
            {
                if (!missileHit)
                {
                    gameManager.TileChecked(hit.collider.gameObject);
                }
            }
        }
    }

    /* private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Missile"))
        {
            missileHit = true;
        }
        else if(other.gameObject.CompareTag("EnemyMissile"))
        {
            hitColor[0] = new Color32(38, 57, 76, 255);
            GetComponent<Renderer>().material.color = hitColor[0];
        }
    } */

    private void OnParticleCollision(GameObject other) {
        Debug.Log("Particle hit!"); //player
        if(gameManager.PlayerTurn)
        {
            Debug.Log("Test-1");
            gameManager.CheckHit(other.gameObject);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Test-2");
            
            // if (other.gameObject.CompareTag("Drone")) {
            //     gameManager.EnemyHitPlayer(other.gameObject.transform.position, targetTile, other.gameObject);
            // } 
            // else {
                enemyScript.PauseAndEnd(targetTile);
                Destroy(gameObject);
            // }
        }
    }

    public void SetTarget(int target)
    {
        targetTile = target;
    }

    public void SetTileColor(int index, Color32 color)
    {
        hitColor[index] = color;
    }

    public void SwitchColors(int colorIndex)
    {
        GetComponent<Renderer>().material.color = hitColor[colorIndex];
    }
}
