using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerTPS : PlayerController
{
    Quaternion tpsLook;

    void Awake()
    {
        SetUp();
        rotateWithMove = false;
    }
   
    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePlayerPosition();
        
    }
    override public void SetMove(Vector2 value)
    {
        move = transform.forward * value.y + transform.right * value.x;
    }
    override public void SetLook(Vector2 value)
    {
        tpsLook = Quaternion.Euler(0, value.x * 50f, 0);
        look = transform.forward * value.x;
    }
    override public void RotatePlayer()
    {
        var speed = this.speed;
        var maxSpeed = this.maxSpeed;
        var lookSmoothTime = this.lookSmoothTime;
        var turnSmoothTime = this.turnSmoothTime;
        if (!onFloor) ApplyInAirModifier(out speed, out maxSpeed, out lookSmoothTime, out turnSmoothTime);

        look = tpsLook * transform.forward;
        float targetLookAngle = Mathf.Atan2(look.x, look.z) * Mathf.Rad2Deg;
        float lookAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetLookAngle, ref lookSmoothVelocity, lookSmoothTime, maxLookSpeed);
        transform.rotation = Quaternion.Euler(0f, lookAngle, 0f);
    }

}
