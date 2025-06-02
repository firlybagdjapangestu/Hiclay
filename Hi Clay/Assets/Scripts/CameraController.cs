using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;

    private void OnEnable()
    {
        GameEvents.OnHit += ShakeCamera;
    }

    private void OnDisable()
    {
        GameEvents.OnHit -= ShakeCamera;
    }

    private void ShakeCamera()
    {
        impulseSource.GenerateImpulse();
    }
}
