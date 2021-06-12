using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// --------- DEV NOTES ------------
// script follows process of how the player attack script works

public class BasanFlamethrower : MonoBehaviour
{
    // Flamethrower information
    [SerializeField] private Collider2D flame;
    private Flamethrower flameData; // access the information tied to the flame collider
    [SerializeField] private SpriteRenderer flameSprite;

    [SerializeField] private float attackActiveTime;
    public float getAttackTime() { return attackActiveTime; }
    public bool attacking;
    

    // reference to the basan this script will be attached to
    private BasanFSMController basan;

    // Start is called before the first frame update
    void Start()
    {
        basan = gameObject.GetComponent<BasanFSMController>();
        attacking = false;
        flame.enabled = false;
        flameSprite.enabled = false;
        flameData = flame.GetComponent<Flamethrower>();
    }

    // if it's enabled does this mean that
    public void ActivateFlamethrower()
    {
        attacking = true;
        flame.enabled = true;
        flameSprite.enabled = true;
    }
    public void DeactivateFlamethrower()
    {
        attacking = false;
        flame.enabled = false;
        flameSprite.enabled = false;
    }

    public void UndoHitbox()
    {
        flame.enabled = false;
    }

    public void TurnOffHitbox()
    {
        flameSprite.enabled = false;
    }

  
}
