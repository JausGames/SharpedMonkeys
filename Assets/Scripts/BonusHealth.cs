using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusHealth : MonoBehaviour
{
    [SerializeField] private float timer = 5f;
    [SerializeField] private bool active = false;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Light light;

    private void Update()
    {
        if (active) { particles.Play(); light.enabled = true; }
        if (!active) { particles.Stop(); light.enabled = false; }

        if (timer > 0) { timer -= Time.deltaTime; return; }
        active = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("BonusHealth : " + other.gameObject.layer);
        if (!active) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.gameObject.GetComponentInParent<Player>().AddHealth(50f);
            active = false;
            timer = 5f;
        };
    }
}
