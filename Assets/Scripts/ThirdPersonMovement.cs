using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonMovement : MonoBehaviour

{


    [SerializeField] float horizontalInput;
    [SerializeField] float verticalInput;

    [Header("Wing trail effects")]
    [Range(0.01f, 1f)]
    [SerializeField] float trailThickness;
    [SerializeField] TrailRenderer[] wingTrailEffects;


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

    [SerializeField] float heightUpwardCurrentSpeed;

    [SerializeField] float heightDownwardCurrentSpeed;

    [Header("Engine Light Settings")]
    [Range(0.1f, 20f)]
    [SerializeField] float engineLightDefault = 1f;

    [Range(0.1f, 20f)]
    [SerializeField] float engineLightBoost = 5f;
    [SerializeField] float currentEngineLightIntensity;


    [SerializeField] Light[] engineLights;
    [SerializeField] Light[] headLights;
    [SerializeField] MeshRenderer[] headRenderers;

    [Header("Thruster Particles Setting")]
    [SerializeField] ParticleSystem mainThrusterParticle;
    [SerializeField] ParticleSystem leftThrusterParticle;
    [SerializeField] ParticleSystem rightThrusterParticle;
    [SerializeField] ParticleSystem normalThrusterParticle;
    [SerializeField] ParticleSystem downwardThrusterParticle;
    [SerializeField] ParticleSystem upwardThrusterParticle;


    [Header("Main Thruster Sound Setting")]
    [SerializeField] AudioSource mainThrusterSoundSource;

    [SerializeField] float defaultSoundPitch = 0.3f;
    [SerializeField] float moveSoundPitch = 1f;
    [SerializeField] float BoostSoundPitch = 1.5f;
   
    [SerializeField] float currentSoundPitch;


    [Header("Thruster Sound Setting")]

    [SerializeField] AudioClip rotationThrusterSoundSource;


    AudioSource rotationThrusterAudio;

 
 

   float turnSmoothVeloctiy;

    // Update is called once per frame
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        defaultSpeed = maxSpeed;
        SetupHeadLightMatEmission();
        
        mainThrusterParticle.GetComponent<ParticleSystem>();
        leftThrusterParticle.GetComponent<ParticleSystem>();   
        rightThrusterParticle.GetComponent <ParticleSystem>();
        normalThrusterParticle.GetComponent<ParticleSystem>();  
        downwardThrusterParticle.GetComponent<ParticleSystem>();
        upwardThrusterParticle.GetComponent<ParticleSystem>();  

        rotationThrusterAudio = GetComponent<AudioSource>();
     

        currentSoundPitch = defaultSoundPitch;
        currentEngineLightIntensity = engineLightDefault;

    }

    

    void Update()
    {

        //play engine sound
        AudioSystem();


        // process input event
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

      

        Vector3 direction = new Vector3 (horizontalInput, 0f, verticalInput).normalized;

        if (direction.magnitude > 0.1)

        {
            ProcessTurnRotation(direction);

            ProcessMovement();

            ProcessBoostMovement();
        }

        else if(direction.magnitude < 0.1)

        {
            ProcessMovementStop();
        }




        ProcessHeightMovement();

        //Control engine lights if any

        if (engineLights.Length >0)
        {
            ProcessEngineLights(engineLights, currentEngineLightIntensity);
        }

        //Control head lights if any

        if (headLights.Length > 0)
        {
            
            ToggleHeadLights(headLights, headRenderers);
        }

        //Toggle lights todo 


    }


    

    void ProcessMovementStop()
    {
        if (currentSpeed > 0)
        {
            currentSpeed -= deceleration * Time.deltaTime;
        }
        else
        {
            currentSpeed = 0;
        }
        controller.Move(transform.forward * currentSpeed * Time.deltaTime);

        // normal thruster Particle

        normalThrusterParticle.Stop();

        // rotate thruster Particle
        leftThrusterParticle.Stop();
        rightThrusterParticle.Stop();


        //Audio
        rotationThrusterAudio.Stop();
        currentSoundPitch = defaultSoundPitch;

     
    }

    void ProcessMovement()
    {
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else
        {
            currentSpeed = maxSpeed;
        }

        controller.Move(transform.forward * currentSpeed * Time.deltaTime);


        //normal thruster particle

        if (!normalThrusterParticle.isPlaying)
        {
            normalThrusterParticle.Play();
        }


        //Audio
        currentSoundPitch = moveSoundPitch;
        
       
        




    }

    void ProcessTurnRotation(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

        float angleY = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVeloctiy, turnSmoothTime);

        //float angleX = Mathf.SmoothDampAngle(transform.eulerAngles.x, cam.eulerAngles.x, ref turnSmoothVeloctiy, turnSmoothTime);

        //transform.rotation = Quaternion.Euler(0f, angleY, 0f);

        transform.rotation = Quaternion.Euler(0f, angleY, 0f);


        if (horizontalInput > 0)
        {
            if (!leftThrusterParticle.isPlaying)
            {
                leftThrusterParticle.Play();
          
            }

            //Audio

            if (!rotationThrusterAudio.isPlaying)
            {
                rotationThrusterAudio.PlayOneShot(rotationThrusterSoundSource);
            }



        }
        else if (horizontalInput < 0)
        {
            if (!rightThrusterParticle.isPlaying)
            {
                rightThrusterParticle.Play();
        
            }

            //Audio

            if (!rotationThrusterAudio.isPlaying)
            {
                rotationThrusterAudio.PlayOneShot(rotationThrusterSoundSource);
            }

        }
        else if(horizontalInput == 0)
        {
            leftThrusterParticle.Stop();
            rightThrusterParticle.Stop();

            //Audio
            rotationThrusterAudio.Stop();

       
        }
    }

    // boost mode
    void ProcessBoostMovement()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {   
            //Set speed to boostspeed and rotation to boost values
            maxSpeed = boostSpeed;

            //Engline lights turn on
            currentEngineLightIntensity = engineLightBoost;

            //Trail Effect turn on
            ChangeWingTrailEffectThickness(trailThickness);

            //Audio

            currentSoundPitch = BoostSoundPitch;


            //Play Main Thruster Particle
            if (!mainThrusterParticle.isPlaying)
            {
                mainThrusterParticle.Play();
            }

        }
        else
        {   //Speed and rotation normal
            maxSpeed = defaultSpeed;

            //Engine lights turn down
            currentEngineLightIntensity = engineLightDefault;

            //Trail Effects turn off
            ChangeWingTrailEffectThickness(0f);

            //Aduio
            currentSoundPitch = moveSoundPitch;

            //Stop Partilce

            mainThrusterParticle.Stop();    
        }
        
    }
    //Flying car thruster sound
    void AudioSystem()
    {
        mainThrusterSoundSource.pitch = Mathf.Lerp(mainThrusterSoundSource.pitch, currentSoundPitch, 10f * Time.deltaTime);

    }


    // move the car upward or downward
    void ProcessHeightMovement()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            //car fly up
            controller.Move(transform.up * heightUpwardCurrentSpeed * Time.deltaTime);
            ProcessHeightFlyUpVelocity();

            //Audio

            currentSoundPitch = moveSoundPitch;

            //Particles
            if (!upwardThrusterParticle.isPlaying)
            {
                upwardThrusterParticle.Play();
            }




        }
        else if (Input.GetKey(KeyCode.E))
        {
            //car fly down
            controller.Move(transform.up * heightDownwardCurrentSpeed * Time.deltaTime);
            ProcessHeightFlyDownVelocity();

            //Audio
            currentSoundPitch = moveSoundPitch;


            //Particle

            if (!downwardThrusterParticle.isPlaying)
            {
                downwardThrusterParticle.Play();
            }
            
        }
       
        else
        {
            StopHeightMovement();

            //Particle
            downwardThrusterParticle.Stop();
            upwardThrusterParticle.Stop();
        }
    }

    void StopHeightMovement()
    {

        if (heightUpwardCurrentSpeed > 0)
        {
            heightUpwardCurrentSpeed -= heightMovementDeceleration * Time.deltaTime;
        }
        else
        {
            heightUpwardCurrentSpeed = 0;
        }

        
        controller.Move(transform.up * heightUpwardCurrentSpeed * Time.deltaTime);



        if (heightDownwardCurrentSpeed < 0)
        {
            heightDownwardCurrentSpeed += heightMovementDeceleration * Time.deltaTime;
        }
        else
        {
            heightDownwardCurrentSpeed = 0;
        }

        controller.Move(transform.up * heightDownwardCurrentSpeed * Time.deltaTime);

        

    }

    void ProcessHeightFlyUpVelocity()
    {
        if (heightUpwardCurrentSpeed < heightMovementMaxSpeed)
        {
            heightUpwardCurrentSpeed += heightMovementAcceleration * Time.deltaTime;
        }
        else
        {
            heightUpwardCurrentSpeed = heightMovementMaxSpeed;
        }

    }

    void ProcessHeightFlyDownVelocity()
    {
        if (heightDownwardCurrentSpeed > -heightMovementMaxSpeed)
        {
            heightDownwardCurrentSpeed -= heightMovementAcceleration * Time.deltaTime;
        }
        else
        {
            heightDownwardCurrentSpeed = -heightMovementMaxSpeed;
        }

    }



    void ProcessEngineLights(Light[] _lights, float _intensity)
    {
        for (int i = 0; i < _lights.Length; i++)
        {
            _lights[i].intensity = Mathf.Lerp(_lights[i].intensity, _intensity, Time.deltaTime * 10f);
        }
    
    }

    void ToggleHeadLights(Light[] _lights, MeshRenderer[] _headLightsMaterial)
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            for (int i = 0; i < _lights.Length; i++)
            {
                _lights[i].enabled = !_lights[i].isActiveAndEnabled;


                if (_lights[i].enabled == false)
                {
                    _headLightsMaterial[i].material.SetColor("_EmissionColor", Color.black);
                }
                else
                {
                    _headLightsMaterial[i].material.SetColor("_EmissionColor", Color.yellow);
                }  
            }
        }
    }

    void ChangeWingTrailEffectThickness(float _thickness)
    {

        for (int i = 0; i < wingTrailEffects.Length; i++)
        {
            wingTrailEffects[i].startWidth = Mathf.Lerp(wingTrailEffects[i].startWidth, _thickness, Time.deltaTime * 10f);

        }
    }


    void SetupHeadLightMatEmission()
    {
        for (int i = 0; i < headRenderers.Length; i++)
        {
            headRenderers[i].material.EnableKeyword("_EMISSION");
            headRenderers[i].material.SetColor("_EmissionColor", Color.yellow);
        }
    }





}
