using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonMovement : MonoBehaviour
{
   public CharacterController controller;
   [SerializeField] Transform cam;

   [Header("Flying Car Speed")]
   float defaultSpeed;
   [SerializeField] float currentSpeed = 0f;
   [SerializeField] float maxSpeed = 30f;

   [Range(10f,200f)]
   [SerializeField] float boostSpeed = 30f;

   [Range(1f,50f)]
   [SerializeField] float acceleration = 4f;

   [Range(1f,50f)]
   [SerializeField] float deceleration = 2f;

   [Range(0.05f,1f)]
   [SerializeField] float turnSmoothTime = 0.1f;

   [Range(1f,50f)]
   [SerializeField] float heightMovementMaxSpeed = 5f;

   [Range(1f,50f)]
   [SerializeField] float heightMovementAcceleration = 2f;

   [Range(1f,50f)]
   [SerializeField] float heightMovementDeceleration = 2f;

   [SerializeField] float heightMovementCurrentSpeed;






   float turnSmoothVeloctiy;

    // Update is called once per frame
    void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked;   
        defaultSpeed = maxSpeed; 
    }
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3 (horizontalInput, 0f, verticalInput).normalized;

        if (direction.magnitude > 0.1)

        {   
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            float angleY = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVeloctiy, turnSmoothTime);

            //float angleX = Mathf.SmoothDampAngle(transform.eulerAngles.x, cam.eulerAngles.x, ref turnSmoothVeloctiy, turnSmoothTime);

            //transform.rotation = Quaternion.Euler(0f, angleY, 0f);
            transform.rotation = Quaternion.Euler(0f, angleY, 0f);


            if (currentSpeed < maxSpeed)
            {
                currentSpeed += acceleration * Time.deltaTime;
            } else
            {
                currentSpeed = maxSpeed;
            }
            

           
            controller.Move(transform.forward * currentSpeed * Time.deltaTime);

            ProcessBoostMovement();

            

        } 
        
        else if(direction.magnitude < 0.1)

        {
            
            if (currentSpeed >0)
            {
                currentSpeed -= deceleration * Time.deltaTime;
            } 
            else
            {
                currentSpeed = 0;
            }

            

            controller.Move(transform.forward * currentSpeed * Time.deltaTime);

        }

        
        ProcessHeightMovement();

        

        
    }

    // boost mode
     void ProcessBoostMovement()
    {

        if (Input.GetKey(KeyCode.LeftShift))
        {

            maxSpeed = boostSpeed;
            
        }
        else
        {
            maxSpeed = defaultSpeed;

        }


    }
    // move the car upward or downward
    void ProcessHeightMovement()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            //car fly up
            controller.Move(transform.up * heightMovementCurrentSpeed * Time.deltaTime);
            ProceesHeightVelocity();
        }
        else if (Input.GetKey(KeyCode.E))
        {
            //car fly down
            controller.Move(transform.up * -heightMovementCurrentSpeed * Time.deltaTime);
            ProceesHeightVelocity();
        }
        else
        {
            if (heightMovementCurrentSpeed >0)
            {
                heightMovementCurrentSpeed -= heightMovementDeceleration * Time.deltaTime;
            } 
            else
            {
                heightMovementCurrentSpeed = 0;
            }

            if ( )
            {
                
            }

            controller.Move(transform.up * heightMovementCurrentSpeed * Time.deltaTime);

        }


        void ProceesHeightVelocity()
        {
            if (heightMovementCurrentSpeed < heightMovementMaxSpeed)
            {
                heightMovementCurrentSpeed += heightMovementAcceleration * Time.deltaTime;
            
            }
            else
            {
                heightMovementCurrentSpeed = heightMovementMaxSpeed;

            }

        }
        
        

    }


}
