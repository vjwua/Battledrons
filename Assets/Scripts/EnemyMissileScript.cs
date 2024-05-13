using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissileScript : MonoBehaviour
{
    GameManager gameManager;
    EnemyScript enemyScript;
    public Vector3 targetTileLocation;
    private int targetTile = -1;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        enemyScript = FindObjectOfType<EnemyScript>();
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Drone"))
        {
            gameManager.EnemyHitPlayer(targetTileLocation, targetTile, other.gameObject);
        }
        else
        {
            enemyScript.PauseAndEnd(targetTile);
        }
    }

    public void SetTarget(int target)
    {
        targetTile = target;
    }
}
