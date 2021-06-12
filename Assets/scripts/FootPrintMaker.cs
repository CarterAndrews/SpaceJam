using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPrintMaker : MonoBehaviour
{
    Vector3 lastStepPosition;
    public float strideLength = 3;

    private void Update()
    {
        if(Vector3.Distance(transform.position, lastStepPosition) < strideLength)
        {
            Step();
        }
    }

    void Step()
    {

    }
}
