using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : AnimatorController
{
    public Animator playerAnimator;
    virtual public void Die()
    {
        playerAnimator.SetTrigger("Die");
    }
    virtual public void Attack()
    {
        playerAnimator.SetTrigger("Attack");
    }
    virtual public void Revive()
    {
        playerAnimator.SetTrigger("Revive");
    }
    virtual public void Spawn()
    {
        playerAnimator.SetTrigger("Spawn");
    }
}
