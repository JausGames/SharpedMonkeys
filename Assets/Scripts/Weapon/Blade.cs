using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Blade : Weapon
{
    [SerializeField] protected Collider bladeCollider;
    protected bool isSlashing = false;
    [SerializeField] protected ParticleSystem slashParticles;
    [SerializeField] protected ParticleSystem bloodParticles;

    private void Start()
    {
        var main = slashParticles.main;
        main.simulationSpace = ParticleSystemSimulationSpace.Custom;
        main.customSimulationSpace = owner.transform.parent;
    }
    public void SetIsSlashing(bool value)
    {
        isSlashing = value;
        if (value) slashParticles.Play();
    }
    public bool GetIsSlashing()
    {
        return isSlashing;
    }
    public void StartSlashing()
    {
        isSlashing = true;
    }
    public void StopSlashing()
    {
        isSlashing = false;
    }
    public void Reject(Player player)
    {
        isSlashing = false;
        player.GetReject(owner);
    }
    public void HitPlayer(Player player)
    {
        bloodParticles.Play();
        player.GetHit(damage, owner);
    }
}
