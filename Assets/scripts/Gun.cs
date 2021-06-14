using Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private Transform m_attachPoint;
    private GameObject m_chargeBall;
    private GameObject m_blastPrefab;
    private GameObject m_guidePrefab;
    CameraFx m_camFx;
    GameObject m_guideObj = null;
    LayerMask outerfence;
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
        outerfence = ~LayerMask.GetMask("OuterFence");
        m_state = GunState.Idle;
        m_chargeBall = transform.Find("art").Find("charge-ball").gameObject;
        m_blastPrefab = Resources.Load<GameObject>("blast");
        m_guidePrefab = Resources.Load<GameObject>("laser-guide");
        m_camFx = GameObject.FindObjectOfType<CameraFx>();
        m_guideObj = Instantiate(m_guidePrefab, m_chargeBall.transform.position, Quaternion.LookRotation(transform.forward, Vector3.up), transform);
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

        UpdateGuideLaser();
    }
    private float GetClosestHitDist()
    {
        GameObject obj = null;
        return GetClosestHitDist(ref obj);

    }
    private float GetClosestHitDist(ref GameObject obj)
    {
        float rayDist = 100;
        Ray ray = new Ray(transform.position - Vector3.up * .8f - transform.forward * .5f, transform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(ray, .4f, rayDist, outerfence);
        RaycastHit closestHit = new RaycastHit();
        float closestdist = rayDist + 5;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject == m_attachPoint.root.gameObject)
            {
                continue;
            }
            else
            {
                if (hit.distance < closestdist)
                {
                    closestdist = hit.distance;
                    closestHit = hit;
                }
            }
        }

        if (closestHit.collider != null)
        {
            rayDist = closestHit.distance;
            obj = closestHit.collider.gameObject;
        }
        return rayDist;
    }

    public void UpdateGuideLaser()
    {
        m_guideObj.transform.localScale = new Vector3(1, 1, GetClosestHitDist());
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

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TriggerSound(AudioManager.TriggerSoundType.GUNSHOT, transform.position);
        }
    }

    private void UpdateMovement()
    {
        if (m_attachPoint == null)
            return;

        transform.position = m_attachPoint.position;
        transform.rotation = Quaternion.LookRotation(m_attachPoint.root.forward, Vector3.up);
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
    float m_toggleTimer;
    
    private void DoCharge()
    {
        if (m_enteringState)
        {
            m_enteringState = false;
            m_chargeProg = 0;
            m_attachPoint.SendMessageUpwards("SetCanMove", false);
           
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

        float rayDist = 99;
        GameObject hitObject = null;
        if ((rayDist = GetClosestHitDist(ref hitObject)) < 80)
        {
            Player otherPlayer = hitObject.GetComponent<Player>();
            if(null != otherPlayer && otherPlayer.isEvil)
            {
                otherPlayer.Die();
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.TriggerSound(AudioManager.TriggerSoundType.VICTORY_SONG, transform.position);
                    foreach(KeyValuePair<GameObject,FMODUnity.StudioEventEmitter> runner in AudioManager.Instance.Runners)
                        AudioManager.Instance.TriggerSoundAttached(AudioManager.TriggerSoundType.VICTORY_CHEER, runner.Value.gameObject);
                }
            }
        }
        if(hitObject)
        {
            Destructable dest = hitObject.GetComponent<Destructable>();
            if (dest)
                dest.Destruct();
        }


        var blastObj = Instantiate(m_blastPrefab, m_chargeBall.transform.position, Quaternion.LookRotation(transform.forward,Vector3.up));
        blastObj.transform.localScale = new Vector3(1,1,rayDist);
        if (m_camFx)
            m_camFx.ApplyShake(15.0f);

        if (AudioManager.Instance != null)
            AudioManager.Instance.ResetInputTimer();

        GoToState(GunState.Recharge);
       
    }

    private float m_rechargeTimer = 0;
    private const float kRechargeTime = 5;
    private const float kRenableMovement = .7f;
    private const float kPreChargeTime = 1f;
    private void DoRecharge()
    {
        if (m_enteringState)
        {
            m_enteringState = false;
            m_rechargeTimer = 0;
            m_guideObj.SetActive(false);
        }
        m_rechargeTimer += Time.deltaTime;

        if(m_rechargeTimer > kRenableMovement)
            m_attachPoint.SendMessageUpwards("SetCanMove", true);

        if (m_rechargeTimer + kPreChargeTime > kRechargeTime && 
            m_rechargeTimer + kPreChargeTime - Time.deltaTime <= kRechargeTime) // Start sfx early
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.TriggerSoundAttached(AudioManager.TriggerSoundType.GUN_FILLED, gameObject);
        }

        if (m_rechargeTimer > kRechargeTime)
        {
            m_guideObj.SetActive(true);
            GoToState(GunState.Idle);
        }
    }
}
