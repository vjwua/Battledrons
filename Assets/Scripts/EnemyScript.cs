using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public List<int[]> PlaceEnemyDrones()
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
        return enemyDrones;
    }
}
