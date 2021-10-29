using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeTriggerEvent : MonoBehaviour
{
    [SerializeField] GameObject bladeContactParticles;
    public Blade blade;
    private void OnTriggerEnter(Collider other)
    {
        if (!blade.GetIsSlashing()) return;
        Debug.Log("BladeTriggerEvent, OnTriggerEnter : other = " + other.name);

        if (other.GetComponentInParent<Blade>() != null && other.GetComponentInParent<Blade>().GetIsSlashing())
        {
            var prefab = Instantiate(bladeContactParticles, other.ClosestPoint(transform.position), Quaternion.FromToRotation(Vector3.up, transform.position - other.ClosestPoint(transform.position)), null);
            blade.Reject(other.GetComponentInParent<Player>());
        }
        else if (other.GetComponent<Player>() != null && other.GetComponent<Player>() != blade.GetOwner())
        {
            blade.HitPlayer(other.GetComponent<Player>());
        }
        else if (other.GetComponentInParent<Player>() != null && other.GetComponentInParent<Player>() != blade.GetOwner())
        {
            blade.HitPlayer(other.GetComponentInParent<Player>());
        }
    }
}
