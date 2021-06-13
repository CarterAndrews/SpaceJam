using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPrint : MonoBehaviour
{
    Material mat;
    float time;
    public AnimationCurve animCurve;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        time += Time.deltaTime;
        mat.SetColor("_TintColor", new Color(1, 1, 1, Mathf.Clamp01( animCurve.Evaluate(time))));
        if (mat.GetColor("_TintColor").a <= 0)
            Destroy(transform.parent.gameObject);
    }
}
