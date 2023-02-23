using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Made by Marcel

public class CameraReference : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    public static CameraReference Instance;
    
    void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    public void SetFollow(Transform transform)
    {
        if (virtualCamera == null)
        {
            return;
        }

        virtualCamera.Follow = transform;
        virtualCamera.LookAt = transform;
    }
}
