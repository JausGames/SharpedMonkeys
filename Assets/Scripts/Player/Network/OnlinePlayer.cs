using Unity.Netcode;
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

    private void Update()
    {
        if (body.velocity.magnitude > 1f) Debug.Log("Player, Update : body.velocity.magnitude = " + body.velocity.magnitude);
        if (transform.position.y < -5 && isAlive)
        {
            OnlinePlayerManager.GetInstance().RegisterSuicide(this);
            Die();
        }
    }
}
