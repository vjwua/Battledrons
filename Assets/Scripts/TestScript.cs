using UnityEngine;

 public class TestScript : MonoBehaviour 
 {
     public void CollisionDetected(DroneScript droneScript)
     {
         Debug.Log("child collided");
     } 
 }