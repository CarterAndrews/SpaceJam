using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private Transform m_attachPoint;
    private GameObject m_chargeBall;

    enum GunState
    {
        Idle,
        Charge,
        Blast,
        Recharge
    }; GunState m_state;

    bool m_enteringState = true;

    private void Start()
    {
        m_state = GunState.Idle;
    }

    private void Update()
    {
        switch (m_state)
        {
            case GunState.Idle:
                DoIdle(); break;
            case GunState.Charge:
                DoCharge(); break;
            case GunState.Blast:
                DoBlast(); break;
            case GunState.Recharge:
                DoRecharge(); break;
        }

        UpdateMovement();
    }

    public void SetAttachPoint(Transform transform)
    {
        m_attachPoint = transform;
    }

    public void TryShoot()
    {
        if (GunState.Idle != m_state)
            return;

        GoToState(GunState.Charge);
    }

    private void UpdateMovement()
    {
        if (m_attachPoint == null)
            return;

        transform.position = Vector3.Lerp(transform.position, m_attachPoint.position, Time.deltaTime * 15.0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_attachPoint.forward, Vector3.up), Time.deltaTime * 15.0f);
    }

    private void GoToState(GunState newState)
    {
        m_state = newState;
        m_enteringState = true;
    }

    private void DoIdle()
    {
        if(m_enteringState)
        {
            m_enteringState = false;
        }
    }

    private void DoCharge()
    {
        if (m_enteringState)
        {
            m_enteringState = false;
        }
    }

    private void DoBlast()
    {
        if (m_enteringState)
        {
            m_enteringState = false;
        }
    }

    private void DoRecharge()
    {
        if (m_enteringState)
        {
            m_enteringState = false;
        }
    }
}
