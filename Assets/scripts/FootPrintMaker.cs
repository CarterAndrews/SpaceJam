using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPrintMaker : MonoBehaviour
{
    public GameObject footPrint;
    Vector3 lastStepPosition;
    public float strideLength = 3;
    bool isRightFoot;

    private void Update()
    {
        if(Vector3.Distance(transform.position, lastStepPosition) < strideLength)
        {
            Vector3 footPrintPosition = transform.position;
            Instantiate(footPrint, footPrintPosition, Quaternion.identity);
            lastStepPosition = transform.position;
        }
    }
}
