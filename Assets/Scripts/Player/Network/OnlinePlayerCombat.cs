using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;



public class OnlinePlayerCombat : PlayerCombat
{
    [SerializeField] ParticleSystem bloodParticle;
    public NetworkVariable<bool> alive = new NetworkVariable<bool>(NetworkVariableReadPermission.Everyone);
    public NetworkVariable<bool> attacking = new NetworkVariable<bool>(NetworkVariableReadPermission.Everyone);

    private void Awake()
    {
        alive.Value = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateAttacking();
    }

    override public void Attack(bool perf, bool canc)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            SetAttacking(perf);
        }
        else
        {
            SubmitAttackRequestServerRpc(perf);
        }
    }

    [ServerRpc]
    void SubmitAttackRequestServerRpc(bool Attacking, ServerRpcParams rpcParams = default)
    {
        SetAttacking(Attacking);
    }

    public void SetAttacking(bool Attacking)
    {
        this.attacking.Value = Attacking;
    }
    public bool GetAttacking()
    {
        return attacking.Value;
    }
    private void UpdateAttacking()
    {
        if (attacking.Value)
        {
            if (!canMove) return;
            //particles.PlayAttackParticle();
            if (attacking.Value && canAttack)
            {
                animator.Attack();
                attackTimer = Time.time + ATTACK_COOLDOWN;
                canAttack = false;
                attacking.Value = false;
            }
        }
        if (!canAttack && attackTimer < Time.time) canAttack = true;
    }
    [ServerRpc]
    void SubmitAttackAnimRequestServerRpc()
    {
        animator.Attack();
        SubmitAttackAnimRequestClientRpc();
    }
    [ClientRpc]
    void SubmitAttackAnimRequestClientRpc()
    {
        animator.Attack();
    }
    public void GetHit(float damage)
    {
            Die();
        
    }
    [ServerRpc]
    void SubmitGetHitRequestServerRpc(float damage, ServerRpcParams rpcParams = default)
    {
        GetHit(damage);
        ClientPlayBloodParticleClientRpc();
    }
    [ClientRpc]
    void ClientPlayBloodParticleClientRpc()
    {
        bloodParticle.Play();
    }

    public void Die()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            SetIsDead(true);
        }
        else
        {
            SubmitIsDeadRequestServerRpc(true);
        }
    }

    public void SetIsDead(bool value)
    {
    }
    [ServerRpc]
    void SubmitIsDeadRequestServerRpc(bool value, ServerRpcParams rpcParams = default)
    {
    }
}
