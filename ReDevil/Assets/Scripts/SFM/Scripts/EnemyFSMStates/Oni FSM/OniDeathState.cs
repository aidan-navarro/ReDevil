using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OniDeathState : FSMState
{
    bool enteredState = true;

    public OniDeathState()
    {
        stateID = FSMStateID.OniDeath;
    }

    public override void EnterStateInit()
    {
        base.EnterStateInit();
        enteredState = true;
    }

    public override void Act(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();
        if (enteredState)
        {
            enteredState = false;
            oc.StartCoroutine(PreformOniDeath(oc));
        }
    }
    
    private IEnumerator PreformOniDeath(OniFSMController oc)
    {
        oc.OnOniBeginDeath?.Invoke();
        oc.oniSprite.gameObject.SetActive(true);
        oc.cutsceneHolder.GetComponent<Animator>().Play("OniSprite");
        yield return new WaitForSeconds(oc.cutsceneHolder.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        oc.OnOniEndDeath.Invoke();
        oc.Killed();
    }

    public override void Reason(Transform player, Transform npc)
    {
        
    }
}
