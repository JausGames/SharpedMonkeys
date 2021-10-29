using MLAPI;
using UnityEngine;
using System.Collections.Generic;

public class OnlinePlayer : Player
{
    [SerializeField] Camera personalCamera;
    private void Start()
    {
        if (GetComponent<NetworkObject>().IsLocalPlayer)
        {
            personalCamera.enabled = true;
            Camera.main.enabled = false;
            Camera.main.GetComponent<AudioListener>().enabled = false;
            personalCamera.GetComponent<AudioListener>().enabled = true;
        }
    }
}
