using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocsillator : MonoBehaviour
{
    [SerializeField]  Vector3 movementVector;
    [SerializeField]  float period;
    [SerializeField][Range(0, 1)]  float movementFactor;

    private Vector3 startPos;


    void Start()
    {
        startPos = transform.position;
    }

    
    void Update()
    {
        OcsillateObject();
    }


    void OcsillateObject()
    {
        if (period <= Mathf.Epsilon) { return; }

        float tau = Mathf.PI * 2;

        float cycles = Time.time / period;

        movementFactor = (Mathf.Sin(cycles * tau) + 1) / 2;

        Vector3 Offset = movementVector * movementFactor;

        transform.position = startPos + Offset;

    }
}
