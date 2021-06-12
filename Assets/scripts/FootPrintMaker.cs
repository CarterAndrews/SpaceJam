using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPrintMaker : MonoBehaviour
{
    public GameObject footPrint;
    Vector3 lastStepPosition;
    float strideLength = 1f;
    float strideWidth = 0.5f;
    bool isRightFoot;

    private void Start()
    {
        lastStepPosition = transform.position;
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, lastStepPosition) > strideLength)
        {
            Vector3 footPrintPosition = transform.position;
            Transform t = Instantiate(footPrint, footPrintPosition, Quaternion.identity).transform;
            t.transform.forward = transform.forward;
            if(isRightFoot)
                t.transform.position += t.transform.right * strideWidth/2;
            else
                t.transform.position += -t.transform.right * strideWidth/2;

            lastStepPosition = transform.position;

            isRightFoot = !isRightFoot;
        }
    }
}
