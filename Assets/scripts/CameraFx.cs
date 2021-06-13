using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFx : MonoBehaviour
{
    private static readonly float MaxScreenShakeIntensity = 20;
    private float m_screenShakeIntensity;
    private Transform m_physicalCamera;
    private void Start()
    {
        m_screenShakeIntensity = 0;
        m_physicalCamera = transform.GetChild(0);
    }

    public void ApplyShake(float intensity)
    {
        m_screenShakeIntensity += intensity;
        m_screenShakeIntensity = Mathf.Min(MaxScreenShakeIntensity, m_screenShakeIntensity);
    }

    private void Update()
    {
        ApplyScreenShake_Interal();
    }

    private void ApplyScreenShake_Interal()
    {
        float rotationalNoiseX = Mathf.PerlinNoise(Time.time * 15.0f, 0) * 2.0f - 1.0f;
        float rotationalNoiseY = Mathf.PerlinNoise(Time.time * 15.0f, 5000) * 2.0f - 1.0f;
        float rotationalNoiseZ = Mathf.PerlinNoise(Time.time * 15.0f, 10000) * 2.0f - 1.0f;

        float maxRotation = 0.5f;
        float rotationAmount = maxRotation * (m_screenShakeIntensity / MaxScreenShakeIntensity);
        m_physicalCamera.transform.localPosition = new Vector3(rotationalNoiseX * rotationAmount, rotationalNoiseY * rotationAmount, rotationalNoiseZ * rotationAmount);

        m_screenShakeIntensity -= 10.0f * Time.deltaTime;
        m_screenShakeIntensity = Mathf.Max(0, m_screenShakeIntensity);

        if(m_screenShakeIntensity <= .01)
            m_physicalCamera.transform.localPosition = Vector3.Lerp(m_physicalCamera.transform.localPosition, Vector3.zero, Time.deltaTime );
    }
}
