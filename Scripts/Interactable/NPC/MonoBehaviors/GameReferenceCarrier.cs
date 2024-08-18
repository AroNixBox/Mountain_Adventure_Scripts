using Cinemachine;
using UnityEngine;

public class GameReferenceCarrier : MonoBehaviour
{
    public static GameReferenceCarrier Instance { get; private set; }
    
    [field: SerializeField] public Transform VcamTarget { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera Vcam { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera EventVcam { get; private set; }
    [field: SerializeField] public Transform EventCameraTarget { get; private set; }
    
    [field: SerializeField] public Transform PlayerHead { get; private set; }
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
