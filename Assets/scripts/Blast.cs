using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blast : MonoBehaviour
{
    public LineRenderer m_lineRenderer;
    void Start()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
        m_rampingUp = true;
        m_lineRenderer.endWidth = 0;
        m_lineRenderer.startWidth = 0;
    }
    float rampUpTime = .05f;
    float holdTime = .2f;
    float rampDownTime = .1f;
    bool m_rampingUp;
    bool m_rampingDown;
    bool m_holding;
    float t = 0;
    float kMaxBlastWidth = 2.0f;
    void Update()
    {
        float blastWidth = 0;
        if (m_rampingUp)
        {
            if (t >= rampUpTime)
            {
                m_rampingUp = false;
                m_holding = true;
                t = 0;
            }
            else
            {
                blastWidth = t * kMaxBlastWidth / rampUpTime;
            }
        }
        else if (m_holding)
        {
            if (t >= holdTime)
            {
                m_holding = false;
                m_rampingDown = true;
                t = 0;
            }
            else
            {
                blastWidth = kMaxBlastWidth;
            }
        }
        else
        {
            if (t >= rampDownTime)
            {
                m_rampingDown = false;
                t = 0;
                Destroy(gameObject);
            }
            else
            {
                blastWidth = Mathf.Lerp(kMaxBlastWidth, 0, t / rampUpTime);
            }
        }
        t += Time.deltaTime;
        m_lineRenderer.endWidth = blastWidth;
        m_lineRenderer.startWidth = blastWidth;

    }
}
