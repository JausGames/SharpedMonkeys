using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform visual;
    [SerializeField] protected ControllerAnimatorController animator;
    [SerializeField] private int playerIndex = 0;
    [SerializeField] protected Rigidbody body;
    [SerializeField] private List<Vector3> wallDirections = new List<Vector3>();
    
    [Header("Move & Look")]
    [SerializeField] protected bool canMove = true; 
    [SerializeField] protected float speed = 60f;
    protected Vector3 move = Vector3.zero;
    [SerializeField] protected float maxSpeed = 14f;
    protected Vector3 look = Vector3.zero;
    protected float lookSmoothTime = 0.1f;
    protected float maxLookSpeed = 1000f;
    [SerializeField] protected float lookSmoothVelocity;
    protected float turnSmoothTime = 0.2f;
    protected float maxTurnSpeed = 600f;
    [SerializeField] protected float turnSmoothVelocity;
    [SerializeField] protected bool rotateWithMove = true;


    [Header("Jump")]
    [SerializeField] private int jumpNumber = 1;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private GameObject onFloorGO;
    [SerializeField] private LayerMask floorLayer;

    private const int MAX_JUMP = 1;
    private bool onWall = false;
    [SerializeField] protected bool onFloor = false;
    [SerializeField] private bool falling = false;

    [SerializeField] protected bool jump = false;

    [SerializeField] private float jumpCooldown = 1f;
    [SerializeField] private float jumpTimer = 0f;
    [SerializeField] private float inAirTimer = 0f;
    [Header("Wall Jump")]
    [SerializeField] private bool wallJumpable = false;
    [SerializeField] private Vector3 wallDirection = Vector3.zero;
    [SerializeField] private float wallJumpCooldown = 0.15f;
    [SerializeField] private float wallJumpTimer = 0f;
    [SerializeField] private float onWallTimer = 0f;
    [SerializeField] const float MAX_WALL_RUN_TIME = 2f;
    [SerializeField] protected bool wallJump = false;
    [SerializeField] private float wallJumpForce = 8f;

    [Header("Dash")]
    protected float dashForce = 15f;
    [SerializeField] protected float dashCooldown = 0.7f;
    protected float inAirDashRatio = 0.6f;
    [SerializeField] protected float dashTimer = 0f;

    void Awake()
    {
        SetUp();
    }
    protected void SetUp()
    {
        for (float x = -Mathf.PI; x < Mathf.PI; x += Mathf.PI * 0.25f)
        {
            wallDirections.Add(new Vector3(Mathf.Cos(x), 0f, Mathf.Sin(x)) * 0.2f);
        }

        visual = transform.Find("Visual");
        body = GetComponent<Rigidbody>();
        body.constraints = RigidbodyConstraints.FreezeRotation;
        speed = (speed + body.drag * 17f) * 0.01f;
    }
    public int GetPlayerIndex()
    {
        return playerIndex;
    }
    public void SetPlayerIndex(int nb)
    {
        playerIndex = nb;
    }
   
    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePlayerPosition();
    }
    protected void UpdatePlayerPosition()
    {
        var totalForward = transform.forward;
        var wallContact = 0;
        foreach (Vector3 dir in wallDirections)
        {
            var wallCol = Physics.OverlapSphere(transform.position + dir - 0.7f * Vector3.up, 0.1f, floorLayer);
            wallContact += wallCol.Length;
            if (wallCol.Length > 0)
            {
                var currDir = -wallCol[0].ClosestPoint(transform.position).normalized;
                wallDirection = new Vector3(currDir.x, 0f, currDir.z);
            }
        }
        if (wallContact > 0)
        {
            if (Vector3.Dot(wallDirection, transform.right) > 0) animator.SetLeftWall(true);
            else animator.SetLeftWall(false);
            animator.SetOnWall(true);
            onWallTimer += Time.deltaTime;
            wallJumpable = true;
        }
        else
        {
            animator.SetOnWall(false);
            onWallTimer = 0f;
            wallJumpable = false;
        }
        //Get if player is on floor
        var cols = Physics.OverlapSphere(onFloorGO.transform.position, 0.2f, floorLayer);
        if (cols.Length == 0)
        {
            inAirTimer += Time.fixedDeltaTime;
            if (onFloor)
            {
                animator.SetInAir(true);
                onFloor = false;
            }
            if (inAirTimer > 0.1f)
            {
                falling = true;
            }
        }
        else if (!onFloor)
        {

            onWallTimer = 0f;
            inAirTimer = 0f;
            animator.SetInAir(false);
            jumpNumber = MAX_JUMP;
            onFloor = true;
            falling = false;
        }
        else
        {
            var forwardOffset = 0.3f * transform.forward;
            var upOffset = -0.8f * transform.up;
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position + upOffset, Vector3.down + forwardOffset, out hit, Mathf.Infinity, floorLayer))
            {
                var vect = (hit.normal).normalized;
                Debug.DrawRay(transform.position + upOffset, (Vector3.down + forwardOffset) * hit.distance, Color.blue);
                var vectX = transform.forward;
                var vectY = Mathf.Sqrt((1f - Mathf.Pow(vect.y, 2f))) * Vector3.up;
                vectY = vectY * Vector3.Dot(transform.right, vect.x * Vector3.right + vect.y * Vector3.forward);
                Debug.DrawRay(transform.position, vectX, Color.magenta);
                Debug.DrawRay(transform.position, vectY, Color.magenta);
                totalForward = vectY + vectX;
                var transformedForward = Vector3.Cross(totalForward, Vector3.right).magnitude * Mathf.Sign(Vector3.Dot(totalForward, Vector3.forward)) * Vector3.forward;
                var transformedRight = Vector3.Cross(totalForward, Vector3.forward).magnitude * Mathf.Sign(Vector3.Dot(totalForward, Vector3.right)) * Vector3.right;
                var transformedUp = Vector3.up * totalForward.y;
                Debug.DrawRay(transform.position + forwardOffset + upOffset, totalForward, Color.yellow);
                Debug.DrawRay(transform.position + forwardOffset + upOffset, transformedRight, Color.green);
                Debug.DrawRay(transform.position + forwardOffset + upOffset, transformedForward, Color.green);
                Debug.DrawRay(transform.position + forwardOffset + upOffset, transformedUp, Color.green);

            }
        }
        if (!canMove) return;

        //make him look around
        if (look.magnitude >= 0.3f)
        {
            RotatePlayer();
        }
        var moveVectX = 0f;
        var moveVectY = 0f;
        var moveVectZ = 0f;
        var wallDir = Vector3.zero;

        //make him move
        if (move.magnitude >= 0.2f)
        {
            MovePlayer(out moveVectX, out moveVectZ);
        }
        //make him jump
        if ((jump && Time.time > jumpTimer && jumpNumber > 0))
        {
            Debug.Log("PlayerController, FixedUpdate : Jump");
            jumpNumber--;


            var horizVelocity = body.velocity.normalized.x * Vector3.right + body.velocity.normalized.z * Vector3.forward;
            var vertVelocity = body.velocity.normalized.y * Vector3.up;

            var hspeedFactor = (Vector3.Dot(horizVelocity, transform.forward) * horizVelocity.magnitude) * (2f * 0.3f);
            var horizFactor = Mathf.Clamp(2 + hspeedFactor, 0f, 3f);

            var vspeedFactor = (Vector3.Dot(vertVelocity, transform.forward) * vertVelocity.magnitude) * (jumpForce * 0.3f);
            var vertFactor = Mathf.Clamp(jumpForce - vspeedFactor, jumpForce * 0.25f, jumpForce);

            var upwardModifier = 0f;
            if (body.velocity.y < 0) upwardModifier = -body.velocity.y;

            Debug.Log("PlayerController, Jump : horizVel = " + (body.velocity.normalized.x * Vector3.right + body.velocity.normalized.z * Vector3.forward));
            Debug.Log("PlayerController, Jump : horizForce = " + horizFactor);
            Debug.Log("PlayerController, Jump : vertVel = " + body.velocity.normalized.z);
            Debug.Log("PlayerController, Jump : vertForce = " + vertFactor);

            jump = false;
            moveVectX = horizFactor * moveVectX;
            moveVectZ = horizFactor * moveVectZ;
            moveVectY = jumpForce + upwardModifier;
            jumpTimer = Time.time + jumpCooldown;
        }
        else if (wallJump && wallJumpTimer < Time.time)
        {
            var horizVelocity = body.velocity.x * Vector3.right + body.velocity.z * Vector3.forward;

            var upwardModifier = 0f;
            if (body.velocity.y < 0) upwardModifier = -body.velocity.y;

            var calculatedWallJumpForce = Mathf.Clamp(wallJumpForce * horizVelocity.magnitude * 0.1f, 5f, wallJumpForce * 1.5f);

            Debug.Log("PlayerController, WallJump : horizVelocity = " + horizVelocity.magnitude);
            Debug.Log("PlayerController, WallJump : calculatedWallJumpForce = " + calculatedWallJumpForce);

            wallJump = false;
            wallDir = wallDirection * calculatedWallJumpForce;
            moveVectY = wallJumpForce * 0.4f + upwardModifier;
            wallJumpTimer = Time.time + wallJumpCooldown;
        }
        else if (!wallJumpable && falling)
            moveVectY = -0.4f;
        else if (onWallTimer > MAX_WALL_RUN_TIME && falling)
            moveVectY = -0.15f;
        

        if (wallJumpable && (moveVectX * Vector3.right + moveVectZ * Vector3.forward).magnitude / speed < 0.2f)
            MoveBody(-0.05f * body.velocity);

        MoveBody(moveVectX * Vector3.right + moveVectY * Vector3.up + moveVectZ * Vector3.forward + (moveVectX + moveVectZ) * totalForward.y * Vector3.down + wallDir);

        
        if ((wallJumpable && onWallTimer <= MAX_WALL_RUN_TIME) && body.velocity.y < 0f)
            MoveBody(-body.velocity.y * Vector3.up);
        


        if (move.magnitude >= 0.3f)
        {
            animator.WalkForward();
            animator.SetSpeed(body.velocity.magnitude * 0.2f);
        }
        else animator.ToIdle();
    }

    virtual protected void MovePlayer(out float moveVectX, out float moveVectZ)
    {
        var speed = this.speed;
        var maxSpeed = this.maxSpeed;
        var lookSmoothTime = this.lookSmoothTime;
        var turnSmoothTime = this.turnSmoothTime;
        if (!onFloor) ApplyInAirModifier(out speed, out maxSpeed, out lookSmoothTime, out turnSmoothTime);

        Debug.Log("PlayerController, FixedUpdate : sign x = " + (Mathf.Sign(move.x) == Mathf.Sign(body.velocity.x)) + ", sign z = " + (Mathf.Sign(move.z) == Mathf.Sign(body.velocity.z)));
        moveVectX = move.x * speed * Mathf.Clamp((maxSpeed - Mathf.Abs(body.velocity.x)) / maxSpeed, 0f, 1f);
        moveVectZ = move.z * speed * Mathf.Clamp((maxSpeed - Mathf.Abs(body.velocity.z)) / maxSpeed, 0f, 1f);

        //make him also look around if right stick isn't touched
        if (look.magnitude < 0.3f && rotateWithMove)
        {
            look = move.normalized * 0.10f;
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime, maxTurnSpeed);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }
    
    virtual protected void MoveBody(Vector3 speed)
    {
        body.velocity += speed;
    }

    virtual public void RotatePlayer()
    {
        var speed = this.speed;
        var maxSpeed = this.maxSpeed;
        var lookSmoothTime = this.lookSmoothTime;
        var turnSmoothTime = this.turnSmoothTime;
        if (!onFloor) ApplyInAirModifier(out speed, out maxSpeed, out lookSmoothTime, out turnSmoothTime);

        float targetLookAngle = Mathf.Atan2(look.x, look.z) * Mathf.Rad2Deg;
        float lookAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetLookAngle, ref lookSmoothVelocity, lookSmoothTime, maxLookSpeed);
        transform.rotation = Quaternion.Euler(0f, lookAngle, 0f);
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }
    public bool GetCanMove()
    {
        return canMove;
    }
    protected void ApplyInAirModifier(out float speed, out float maxSpeed, out float lookSmoothTime, out float turnSmoothTime)
    {
        speed = this.speed / 1.5f;
        maxSpeed = this.maxSpeed / 3f;
        lookSmoothTime = this.lookSmoothTime * 1.5f;
        turnSmoothTime = this.turnSmoothTime * 3f;
        
    }
    public virtual void SetMove(Vector2 value)
    {
        move = new Vector3(value.x, 0f, value.y);
    }
    virtual public void SetJump()
    {
        Debug.Log("PlayerControler, StartJump : Jump");
        if (onFloor && !falling && !jump)
        {
            animator.PlayJump();
            jump = true;
        }
        else if(!jump && wallJumpable && !wallJump)
        {
            animator.PlayJump();
            wallJump = true;
        }
    }
    virtual public void Dash(bool perf, bool canc)
    {
        Debug.Log("PlayerControler, Dash");
        if (perf && dashTimer < Time.time && !jump && !wallJump)
        {
            animator.PlayDash();
            dashTimer = Time.time + dashCooldown;


            var horizVelocity = body.velocity.normalized.x * Vector3.right + body.velocity.normalized.z * Vector3.forward;
            var speedFactor = (Vector3.Dot(horizVelocity, transform.forward) * horizVelocity.magnitude) * (dashForce * 0.3f);
            var Force = Mathf.Clamp(dashForce - speedFactor, dashForce / 3f, dashForce * 1.66667f);
            var upwardModifier = Vector3.zero;
            if (body.velocity.y < 0) upwardModifier = - body.velocity.y * Vector3.up;
            Debug.Log("PlayerController, Dash ; vel = " + body.velocity.normalized.x * Vector3.right + body.velocity.normalized.z * Vector3.forward);
            Debug.Log("PlayerController, dash ; dashForce = " + Force);
            if (!onFloor) Force = Force * inAirDashRatio;
            MoveBody(transform.forward * Force + upwardModifier);
        }
    }
    public void StartFalling()
    {
        falling = true;
    }
    public Vector2 GetMove()
    {
       return new Vector2 (move.x, move.z);
    }
    public bool GetOnFloor()
    {
        return onFloor;
    }
    public Vector2 GetLook()
    {
        return new Vector2(look.x, look.z);
    }
    virtual public void SetLook(Vector2 value)
    {
        look = new Vector3(value.x, 0f, value.y);
    }

    public void StopMotion()
    {
        MoveBody(-body.velocity);
    }
    public void SetFreeze(bool value)
    {
        if (value)
        {
            body.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            body.constraints = RigidbodyConstraints.None;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (onFloorGO) Gizmos.DrawWireSphere(onFloorGO.transform.position, 0.2f);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Vector3 dir in wallDirections)
        {
            Gizmos.DrawWireSphere(transform.position + dir - 0.7f * Vector3.up, 0.1f);
        }
        Gizmos.color = Color.green;
        if (wallDirection != Vector3.zero)
        {
            Gizmos.DrawLine(transform.position, transform.position + wallDirection * 2f);
        }
        
    }

}
