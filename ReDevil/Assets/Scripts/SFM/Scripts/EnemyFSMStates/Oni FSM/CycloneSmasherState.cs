using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycloneSmasherState : FSMState
{
    bool enteredState = true;
    bool cycloneCharged;
    GameObject pillar;

    public CycloneSmasherState()
    {
        stateID = FSMStateID.OniCycloneSmashing;
    }

    public override void EnterStateInit()
    {
        Debug.Log("Oni Club Smash");
        enteredState = true; 
        cycloneCharged = false;
    }

    public override void Act(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();

        if (enteredState)
        {
            oc.StartCoroutine(ChargeUpCyclone(npc));
            pillar = oc.SpawnPillar(false);
            oc.OnWallHit += OnWallHit;
            enteredState = false;
        }

        else if (cycloneCharged && pillar != null)
        {
            oc.transform.position = Vector3.MoveTowards(oc.transform.position, new Vector2(pillar.transform.position.x, pillar.transform.position.y), oc.CycloneSpeed * Time.deltaTime);

        }
    }

    private void OnWallHit()
    {
        UnityEngine.Object.Destroy(pillar);
    }

    public override void Reason(Transform player, Transform npc)
    {
        OniFSMController oc = npc.GetComponent<OniFSMController>();

        if (oc.health <= 0)
        {
            oc.StopAllCoroutines();
            oc.PerformTransition(Transition.EnemyNoHealth);
            UnityEngine.Object.Destroy(pillar);
            npc.GetComponent<SpriteRenderer>().color = Color.white;
            return;
        }

        if (!enteredState && pillar == null)
        {
            oc.DisableIFrames();
            oc.PerformTransition(Transition.OniIdle);
            npc.GetComponent<SpriteRenderer>().color = Color.white;
            return;
        }

        
    }

    IEnumerator ChargeUpCyclone(Transform npc)
    {
        yield return new WaitForSeconds(2.0f);
        npc.GetComponent<SpriteRenderer>().color = Color.red;
        cycloneCharged = true;
    }

}
