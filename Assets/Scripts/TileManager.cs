using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    GameManager gameManager;
    Ray ray;
    RaycastHit hit;
    private bool missileHit = false;
    Color[] hitColor = new Color[2];

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
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
}
