using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Weapon : MonoBehaviour
{
    [SerializeField] protected Player owner;
    protected float damage;

    public Player GetOwner()
    {
        return owner;
    }
}
