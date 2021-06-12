using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPrintMaker : MonoBehaviour
{
    public GameObject footPrint;
    Vector3 lastStepPosition;
    public float strideLength;
    bool isRightFoot;

    private void Update()
    {
        if(Vector3.Distance(transform.position, lastStepPosition) > strideLength)
        {
            Vector3 footPrintPosition = transform.position;
            Transform t = Instantiate(footPrint, footPrintPosition, Quaternion.identity).transform;
            t.transform.forward = transform.forward;
            if(isRightFoot)
                t.transform.position += t.transform.right;
            else
                t.transform.position += -t.transform.right;

            lastStepPosition = transform.position;

            isRightFoot = !isRightFoot;
        }
    }
}
