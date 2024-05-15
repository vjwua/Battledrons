using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject enemyMissilePrefab;
    [SerializeField] GameObject firePrefab;

    char[] guessGrid;
    List<int> potentialHits;
    List<int> currentHits;
    private int guess;
    GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        potentialHits = new List<int>();
        currentHits = new List<int>();
        guessGrid = Enumerable.Repeat('o', 100).ToArray();
    }

    public List<int[]>PlaceEnemyDrones()
    {
        List<int[]> enemyDrones = new List<int[]>
        {
            new int[]{-1, -1, -1, -1},
            new int[]{-1, -1, -1},
            new int[]{-1, -1},
            new int[]{-1},
            new int[]{-1}
        };
        int[] gridNumbers = Enumerable.Range(1, 100).ToArray();
        bool taken = true;
        foreach(int[] tileNumArray in enemyDrones)
        {
            taken = true;
            while (taken)
            {
                taken = false;
                int droneHead = UnityEngine.Random.Range(0, 99);
                int rotateBool = UnityEngine.Random.Range(0, 2);
                int minusAmount = rotateBool == 0 ? 10 : 1;
                for(int i = 0; i < tileNumArray.Length; i++)
                {
                    //Check that drone end will not go off board and check if tile is taken
                    if (droneHead - (minusAmount * i) < 0
                    || gridNumbers[droneHead - i * minusAmount] < 0)
                    {
                        taken = true;
                        break;
                    }
                    //Drone is horizontal, check drone doesn't go off the sides 0-10, 11-20
                    else if(minusAmount == 1
                    && droneHead / 10 != ((droneHead - i * minusAmount) -1) / 10)
                    {
                        taken = true;
                        break;
                    }
                }
                //If tile isn't taken, loop through tile numbers assign them to the array in the list
                if (!taken)
                {
                    for(int j = 0; j < tileNumArray.Length; j++)
                    {
                        tileNumArray[j] = gridNumbers[droneHead - j * minusAmount];
                        gridNumbers[droneHead - j * minusAmount] = -1;
                    }
                }
            }
        }
        /* foreach(int[] numArray in enemyDrones)
        {
            string temp = "";
            for (int i = 0; i < numArray.Length; i++)
            {
                temp += ", " + numArray[i];
            }
            Debug.Log(temp);
        } */
        /*foreach (var x in enemyDrones)
        {
            Debug.Log($"x: {x[0]}");
        }*/
        return enemyDrones;
    }

    public void NPCTurn()
    {
        List<int> hitIndex = new List<int>();
        for (int i = 0; i < guessGrid.Length; i++)
        {
            if (guessGrid[i] == 'h')
            {
                hitIndex.Add(i);
            }
        }
        if (hitIndex.Count > 1)
        {
            int diff = hitIndex[1] - hitIndex[0];
            int guessPosition = Random.Range(0, 2) * 2 - 1;
            int nextIndex = hitIndex[0] + diff;
            while(guessGrid[nextIndex] != 'o')
            {
                if(guessGrid[nextIndex] == 'm'
                || nextIndex > 100
                || nextIndex < 0)
                {
                    diff *= -1;
                }
                nextIndex += diff;
            }
            guess = nextIndex;
        }
        else if (hitIndex.Count == 1)
        {
            List<int> closeTiles = new List<int>();
            closeTiles.Add(1); //N-S-W-E
            closeTiles.Add(-1);
            closeTiles.Add(10);
            closeTiles.Add(-10);
            int index, possibleGuess;
            bool onGrid;
            TileGuess(hitIndex, closeTiles, out index, out possibleGuess, out onGrid);
            while ((!onGrid || guessGrid[possibleGuess] != 'o') && closeTiles.Count > 0)
            {
                closeTiles.RemoveAt(index);
                TileGuess(hitIndex, closeTiles, out index, out possibleGuess, out onGrid);
            }
            guess = possibleGuess;
        }
        else
        {
            int nextIndex = Random.Range(0, 100);
            while(guessGrid[nextIndex] != 'o')
            {
                nextIndex = Random.Range(0, 100);
            }
            guess = nextIndex;
            nextIndex = GuessAgainCheck(nextIndex);
            Debug.Log(" --- ");
            nextIndex = GuessAgainCheck(nextIndex);
            Debug.Log(" -########-- ");
            guess = nextIndex;
        }
        GameObject tile = GameObject.Find($"({guess / 10}, {guess % 10})");
        guessGrid[guess] = 'm';
        ParticleSystem laserParticle = enemyMissilePrefab.GetComponentInChildren<ParticleSystem>();
        Vector3 tilePosition = tile.transform.position;
        tilePosition.y += 25;
        enemyMissilePrefab.transform.position = tilePosition;
        GameObject missile = Instantiate(firePrefab, tilePosition, Quaternion.identity);
        Debug.Log(guess);
        tile.GetComponentInChildren<TileManager>().SetTarget(guess);
        laserParticle.Play();
    }

    private static void TileGuess(List<int> hitIndex, List<int> closeTiles, out int index, out int possibleGuess, out bool onGrid)
    {
        index = Random.Range(0, closeTiles.Count);
        possibleGuess = hitIndex[0] + closeTiles[index];
        onGrid = possibleGuess > -1 && possibleGuess < 100;
    }

    private int GuessAgainCheck(int nextIndex)
    {
        string str = "nx: " + nextIndex;
        int newGuess = nextIndex;
        bool edgeCase = nextIndex < 10 || nextIndex > 89 || nextIndex % 10 == 0 || nextIndex % 10 == 9;
        bool nearGuess = false;
        if (nextIndex + 1 < 100) nearGuess = guessGrid[nextIndex + 1] != 'o';
        if (!nearGuess && nextIndex - 1 > 0) nearGuess = guessGrid[nextIndex - 1] != 'o';
        if (!nearGuess && nextIndex + 10 < 100) nearGuess = guessGrid[nextIndex + 10] != 'o';
        if (!nearGuess && nextIndex - 10 > 0) nearGuess = guessGrid[nextIndex - 10] != 'o';
        if (edgeCase || nearGuess) newGuess = Random.Range(0, 100);
        while (guessGrid[newGuess] != 'o') newGuess = Random.Range(0, 100);
        Debug.Log(str + " newGuess: " + newGuess + " e:" + edgeCase + " g:" + nearGuess);
        return newGuess;
    }

    public void MissileHit(int hit)
    {
        guessGrid[guess] = 'h';
        Invoke(nameof(EndTurn), 1.0f);
    }

    public void FallenPlayer()
    {
        for (int i = 0; i < guessGrid.Length; i++)
        {
            if (guessGrid[i] == 'h')
            {
                guessGrid[i] = 'x';
            }
        }
    }

    private void EndTurn()
    {
        gameManager.GetComponent<GameManager>().EndEnemyMove();
    }

    public void PauseAndEnd(int miss)
    {
        if(currentHits.Count > 0 && currentHits[0] > miss)
        {
            foreach(int potential in potentialHits)
            {
                if(currentHits[0] > miss)
                {
                    if (potential < miss) potentialHits.Remove(potential);
                } else
                {
                    if (potential > miss) potentialHits.Remove(potential);
                }
            }
        }
        Invoke(nameof(EndTurn), 1.0f);
    }

    public void PauseAndEnd()
    {
        Invoke(nameof(EndTurn), 1.0f);
    }
}
