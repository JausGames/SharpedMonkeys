using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    //[SerializeField] private PlayerParticleManager particles;
    [SerializeField] protected bool canMove = false;
    [SerializeField] protected bool canAttack = true;
    [SerializeField] protected float attackTimer = 0f;
    protected const float ATTACK_COOLDOWN = 0.3f;
    [SerializeField] protected CombatController animator;
    [SerializeField] private Blade weapon;
    private Rigidbody body = null;
    private int playerIndex = 0;

    [Header("Attack stats")]
    private float attackDashForce = 6f;
    private float attackDashSpeedLimit = 5f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<CombatController>();
        body = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (!canAttack && attackTimer < Time.time) canAttack = true;
    }
    public void SetSlashing(bool value)
    {
        weapon.SetIsSlashing(value);
        if (value)
        {
            var horizVelocity = body.velocity.normalized.x * Vector3.right + body.velocity.normalized.z * Vector3.forward;
            var speedFactor = (Vector3.Dot(horizVelocity, transform.forward) * horizVelocity.magnitude) * (attackDashForce * 0.06f);
            var dashForce = Mathf.Clamp(attackDashForce - speedFactor, 1f, 10f);
            Debug.Log("PlayerCombat, SetSlashing ; vel = " + body.velocity.normalized.x * Vector3.right + body.velocity.normalized.z * Vector3.forward);
            Debug.Log("PlayerCombat, SetSlashing ; dashForce = " + dashForce);
            if (GetComponent<PlayerController>().GetOnFloor()) dashForce = dashForce * 1.7f;
            body.velocity += transform.forward * dashForce;
        }
    }
    public Blade GetBlade()
    {
        return weapon;
    }



    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;
        
    }
    public void SetCanMove(bool value)
    {
        Debug.Log("PlayerCombat, SetCanMove : value = " + value);
        canMove = value;
    }
    public void SetCanAttackTrue()
    {
        //canAttack = true;
    }
    public bool GetCanMove()
    {
        return canMove;
    }

    public int GetPlayerIndex()
    {
        return playerIndex;
    }
    public void SetPlayerIndex(int nb)
    {
        playerIndex = nb;
    }
    virtual public void Attack(bool perf, bool canc)
    {
        if (!canMove) return;
        //particles.PlayAttackParticle();
        if (perf && canAttack)
        {
            animator.Attack();
            attackTimer = Time.time + ATTACK_COOLDOWN;
            canAttack = false;
        }

    }
}
