using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeAmplitude = 1.2f;
    [SerializeField] private float shakeFrequency = 2.0f;
    private CinemachineBasicMultiChannelPerlin noise;


    private void Start()
    {
        if (virtualCamera != null)
        {
            noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    public void ShakeCamera()
    {
        if (noise != null)
        {
            noise.m_AmplitudeGain = shakeAmplitude;
            noise.m_FrequencyGain = shakeFrequency;
            Invoke("StopShaking", shakeDuration);
        }
    }

    void StopShaking()
    {
        noise.m_AmplitudeGain = 0;
    }
}
