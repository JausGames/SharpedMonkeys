using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    Animator animator = null;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Rigidbody>().velocity.magnitude > 0.1f || other.GetComponentInParent<Rigidbody>().velocity.magnitude > 0.1f) animator.SetBool("Touched", true);
        else animator.SetBool("Touched", false);
    }
    private void OnTriggerExit(Collider other)
    {
        animator.SetBool("Touched", false);
    }

}
