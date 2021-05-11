using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulArmament : MonoBehaviour
{
    [SerializeField]
    protected float SoulCostPerSecond;
    public float SoulCost => SoulCostPerSecond;

    protected PlayerFSMController player;
    protected bool active = false;
    public bool IsActive => active;

    public virtual void ActivateArament()
    {
        active = true;
    }

    public virtual void DeActivateArament()
    {
        active = false;
    }
}