using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulShield : SoulArmament
{
    [SerializeField]
    GameObject shieldObject;

    private void Start()
    {
        shieldObject.SetActive(false);
    }

    public override void ActivateArament()
    {
        base.ActivateArament();
        shieldObject.SetActive(true);
    }

    public override void DeActivateArament()
    {
        base.DeActivateArament();
        shieldObject.SetActive(false);
    }
}
