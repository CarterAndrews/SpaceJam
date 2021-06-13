using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private Transform m_attachPoint;
    private GameObject m_chargeBall;
    private GameObject m_blastPrefab;
    CameraFx m_camFx;
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
        m_chargeBall = transform.Find("art").Find("charge-ball").gameObject;
        m_blastPrefab = Resources.Load<GameObject>("blast");
        m_camFx = GameObject.FindObjectOfType<CameraFx>();
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

    const float kChargeDuration = .15f;
    const float kMaxChargeBallRad = 10.0f;
    float m_chargeProg = 0;
    private void DoCharge()
    {
        if (m_enteringState)
        {
            m_enteringState = false;
            m_chargeProg = 0;
        }

        m_chargeProg += Time.deltaTime;
        if(m_chargeProg > kChargeDuration)
        {
            m_chargeBall.transform.localScale = Vector3.zero;
            GoToState(GunState.Blast);
        }
        else
        {
            float scale = Mathf.Sin( m_chargeProg * Mathf.PI / kChargeDuration ) * kMaxChargeBallRad;
            m_chargeBall.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    private void DoBlast()
    {
        if (m_enteringState)
        {
            m_enteringState = false;
        }
        Ray ray = new Ray(m_chargeBall.transform.position, transform.forward);
        float rayDist = 100;
        RaycastHit hitInfo;
        if (Physics.SphereCast(ray, .2f, out hitInfo, rayDist))
        {
            var hitObject = hitInfo.collider.gameObject;
            Player otherPlayer = hitObject.GetComponent<Player>();
            if(null != otherPlayer && otherPlayer.isEvil)
            {
                otherPlayer.Die();
            }

            rayDist = hitInfo.distance;
        }
        var blastObj = Instantiate(m_blastPrefab, m_chargeBall.transform.position, Quaternion.LookRotation(transform.forward,Vector3.up));
        blastObj.transform.localScale = new Vector3(1,1,rayDist);
        if (m_camFx)
            m_camFx.ApplyShake(15.0f);
        GoToState(GunState.Recharge);
    }

    private void DoRecharge()
    {
        if (m_enteringState)
        {
            m_enteringState = false;
        }
        GoToState(GunState.Idle);
    }
}
