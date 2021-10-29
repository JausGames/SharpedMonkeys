using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private Camera cam;


    private void Start()
    {
        cam = FindObjectOfType<Camera>();
    }
    private void FixedUpdate()
    {
        if (cam) transform.rotation = Quaternion.Euler(-45f, 180f, 0f) ;
        else cam = FindObjectOfType<Camera>();
    }
}
