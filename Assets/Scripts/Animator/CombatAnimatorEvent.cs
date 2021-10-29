using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAnimatorEvent : MonoBehaviour
{
    [SerializeField] PlayerCombat combat;

    private void Awake()
    {
        combat = GetComponentInParent<PlayerCombat>();
    }
    public void SetCanAttackTrue()
    {
        combat.SetCanAttackTrue();
    }
    public void StartSlashingEvent()
    {
        combat.SetSlashing(true);
    }
    public void StopSlashingEvent()
    {
        combat.SetSlashing(false);
    }
}
