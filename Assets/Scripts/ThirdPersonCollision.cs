using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCollision : MonoBehaviour
{
    [SerializeField] float collideForce = 1;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log(this.name + " collide with " + hit.gameObject.name);
        //hit.rigidbody.AddForce(Vector3.forward * collideForce, ForceMode.Impulse);

        

        if (hit.gameObject.CompareTag("Obstacle"))
        {
            hit.rigidbody.AddRelativeForce(Vector3.forward, ForceMode.Impulse);
        }

        




    }



    
}
