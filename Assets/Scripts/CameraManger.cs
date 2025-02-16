using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraManger : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineFreeLook;
    public void Init(Transform follow, Transform Lookat)
    {
        cinemachineFreeLook.Follow = follow;
        cinemachineFreeLook.LookAt = Lookat;
    }
}
