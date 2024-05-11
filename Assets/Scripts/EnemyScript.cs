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
                int droneAhead = UnityEngine.Random.Range(0, 99);
                int rotateBool = UnityEngine.Random.Range(0, 2);
                int minusAmount = rotateBool == 0 ? 10 : 1;
                for(int i = 0; i < tileNumArray.Length; i++)
                {
                    if (droneAhead - (minusAmount * 1) < 0
                    || gridNumbers[droneAhead - i * minusAmount] < 0)
                    {
                        taken = true;
                        break;
                    }
                    else if(minusAmount == 1
                    && droneAhead / 10 != (droneAhead - i * minusAmount) / 10)
                    {
                        taken = true;
                        break;
                    }
                }
                if (!taken)
                {
                    for(int j = 0; j < tileNumArray.Length; j++)
                    {
                        tileNumArray[j] = gridNumbers[droneAhead - j * minusAmount];
                        gridNumbers[droneAhead - j * minusAmount] = -1;
                    }
                }
            }
        }
        return enemyDrones;
    }
}
