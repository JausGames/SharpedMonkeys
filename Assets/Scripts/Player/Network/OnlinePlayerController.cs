using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class OnlinePlayerController : PlayerControllerTPS
{
    NetworkObject netObj;

    private NetworkVariable<Vector3> netMove = new NetworkVariable<Vector3>(NetworkVariableReadPermission.Everyone);
    private NetworkVariable<Quaternion> netTpsLook = new NetworkVariable<Quaternion>(NetworkVariableReadPermission.Everyone);
    private NetworkVariable<Vector3> netLook = new NetworkVariable<Vector3>(NetworkVariableReadPermission.Everyone);
    private NetworkVariable<bool> netJump = new NetworkVariable<bool>(NetworkVariableReadPermission.Everyone);
    private NetworkVariable<bool> netDash = new NetworkVariable<bool>(NetworkVariableReadPermission.Everyone);

    void Awake()
    {
        SetUp();
        rotateWithMove = false;
    }
    void FixedUpdate()
    {
        UpdateDashing();
        UpdatePlayerPosition();
    }

    //Methods use to netMove player
    public void Move(Vector2 direction)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            ChangeAcceleration(direction);
        }
        else
        {
            SubmitMoveRequestServerRpc(direction);
        }
    }

    [ServerRpc]
    void SubmitMoveRequestServerRpc(Vector2 direction, ServerRpcParams rpcParams = default)
    {
        ChangeAcceleration(direction);
    }

    void ChangeAcceleration(Vector2 direction)
    {
        Debug.Log("PlayerController, ChangeAcceleration : direction " + direction);
        Debug.Log("PlayerController, ChangeAcceleration : magnitude " + body.velocity.magnitude);

        //check controller dead zone  
        if (direction.magnitude > 0.2f) netMove.Value = transform.forward * direction.y + transform.right * direction.x;
        else netMove.Value = Vector3.zero;

        move = netMove.Value;
    }

    // Methods use to rotate player
    public void Look(Vector2 direction)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            ChangeRotation(direction);
        }
        else
        {
            SubmitLookRequestServerRpc(direction);
        }
    }
    [ServerRpc]
    void SubmitLookRequestServerRpc(Vector2 direction, ServerRpcParams rpcParams = default)
    {
        ChangeRotation(direction);
    }
    void ChangeRotation(Vector2 direction)
    {
        Debug.Log("PlayerController, ChangeRotation : direction " + direction);
        Debug.Log("PlayerController, ChangeRotation : magnitude " + body.velocity.magnitude);

        //check controller dead zone  
        if (direction.magnitude > 0.05f)
        {
            netTpsLook.Value = Quaternion.Euler(0, direction.x * 50f, 0);
            netLook.Value = transform.forward * direction.x;
        }
        else
        {
            netTpsLook.Value = Quaternion.identity;
            netLook.Value = transform.forward;
        }
        look = netLook.Value;
    }

    // Methods use to rotate player
    public void Jump(bool perf, bool canc)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            ChangeJump(perf);
        }
        else
        {
            SubmitJumpRequestServerRpc(perf);
        }
    }
    [ServerRpc]
    void SubmitJumpRequestServerRpc(bool jumping, ServerRpcParams rpcParams = default)
    {
        ChangeJump(jumping);
    }
    void ChangeJump(bool jumping)
    {
        Debug.Log("PlayerController, ChangeJump : jumping " + jumping);

        netJump.Value = jumping;
        if (jumping) SetJump();
    }
    override public void RotatePlayer()
    {
        var speed = this.speed;
        var maxSpeed = this.maxSpeed;
        var lookSmoothTime = this.lookSmoothTime;
        var turnSmoothTime = this.turnSmoothTime;
        if (!onFloor) ApplyInAirModifier(out speed, out maxSpeed, out lookSmoothTime, out turnSmoothTime);

        netLook.Value = netTpsLook.Value * transform.forward;
        float targetLookAngle = Mathf.Atan2(netLook.Value.x, netLook.Value.z) * Mathf.Rad2Deg;
        float lookAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetLookAngle, ref lookSmoothVelocity, lookSmoothTime, maxLookSpeed);
        if (NetworkManager.Singleton.IsServer)
        {
            ChangeRotation(lookAngle);
        }
        else
        {
            SubmitChangeRotationRequestServerRpc(lookAngle);
        }
    }

    [ServerRpc]
    private void SubmitChangeRotationRequestServerRpc(float lookAngle, ServerRpcParams rpcParams = default)
    {
        ChangeRotation(lookAngle);
    }
    private void ChangeRotation(float lookAngle)
    {
        transform.rotation = Quaternion.Euler(0f, lookAngle, 0f);
    }
    protected override void MovePlayer(out float netMoveVectX, out float netMoveVectZ)
    {
        var speed = this.speed;
        var maxSpeed = this.maxSpeed;
        var lookSmoothTime = this.lookSmoothTime;
        var turnSmoothTime = this.turnSmoothTime;
        if (!onFloor) ApplyInAirModifier(out speed, out maxSpeed, out lookSmoothTime, out turnSmoothTime);

        Debug.Log("PlayerController, FixedUpdate : sign x = " + (Mathf.Sign(netMove.Value.x) == Mathf.Sign(body.velocity.x)) + ", sign z = " + (Mathf.Sign(netMove.Value.z) == Mathf.Sign(body.velocity.z)));
        netMoveVectX = netMove.Value.x * speed * Mathf.Clamp((maxSpeed - Mathf.Abs(body.velocity.x)) / maxSpeed, 0f, 1f);
        netMoveVectZ = netMove.Value.z * speed * Mathf.Clamp((maxSpeed - Mathf.Abs(body.velocity.z)) / maxSpeed, 0f, 1f);
        
        //make him also look around if right stick isn't touched
        if (netLook.Value.magnitude < 0.3f && rotateWithMove)
        {
            netLook.Value = netMove.Value.normalized * 0.10f;
            float targetAngle = Mathf.Atan2(netMove.Value.x, netMove.Value.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime, maxTurnSpeed);
            if (NetworkManager.Singleton.IsServer)
            {
                ChangeRotation(angle);
            }
            else
            {
                SubmitChangeRotationRequestServerRpc(angle);
            }
        }
    }
    protected override void MoveBody(Vector3 speed)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            ChangePosition(speed);
        }
        else
        {
            SubmitChangePositionRequestServerRpc(speed);
        }
    }

    [ServerRpc]
    private void SubmitChangePositionRequestServerRpc(Vector3 speed, ServerRpcParams rpcParams = default)
    {
        //GetComponent<NetworkRigidbody>().HasNetworkObject;
        if (NetworkManager.Singleton.IsServer)
        {
            body.velocity += speed;
        }
        ChangePosition(speed);
    }
    private void ChangePosition(Vector3 speed)
    {
        body.velocity += speed;
    }
    override public void Dash(bool perf, bool canc)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            SetDashing(perf);
        }
        else
        {
            SubmitDashRequestServerRpc(perf);
        }
    }

    [ServerRpc]
    void SubmitDashRequestServerRpc(bool Dashing, ServerRpcParams rpcParams = default)
    {
        SetDashing(Dashing);
    }

    public void SetDashing(bool Dashing)
    {
        this.netDash.Value = Dashing;
    }
    public bool GetDashing()
    {
        return netDash.Value;
    }
    private void UpdateDashing()
    {
        if (netDash.Value)
        {
            Debug.Log("PlayerControler, Dash");
            if (netDash.Value && dashTimer < Time.time && !jump && !wallJump)
            {
                animator.PlayDash();
                dashTimer = Time.time + dashCooldown;


                var horizVelocity = body.velocity.normalized.x * Vector3.right + body.velocity.normalized.z * Vector3.forward;
                var speedFactor = (Vector3.Dot(horizVelocity, transform.forward) * horizVelocity.magnitude) * (dashForce * 0.3f);
                var Force = Mathf.Clamp(dashForce - speedFactor, dashForce / 3f, dashForce * 1.66667f);
                var upwardModifier = Vector3.zero;
                if (body.velocity.y < 0) upwardModifier = -body.velocity.y * Vector3.up;
                Debug.Log("PlayerController, Dash ; vel = " + body.velocity.normalized.x * Vector3.right + body.velocity.normalized.z * Vector3.forward);
                Debug.Log("PlayerController, dash ; dashForce = " + Force);
                if (!onFloor) Force = Force * inAirDashRatio;
                MoveBody(transform.forward * Force + upwardModifier);
                netDash.Value = false;
            }
        }
    }
}
