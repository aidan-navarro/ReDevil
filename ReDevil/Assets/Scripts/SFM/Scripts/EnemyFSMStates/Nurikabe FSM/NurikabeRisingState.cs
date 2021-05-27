using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurikabeRisingState : FSMState
{
    // Start is called before the first frame update
    private Vector2 startPos;
    private Vector2 endPos;

    public NurikabeRisingState()
    {
        stateID = FSMStateID.NurikabeRising;
    }
    public override void Act(Transform player, Transform npc)
    {
        NurikabeFSMController nc = npc.GetComponent<NurikabeFSMController>();
        Debug.Log("In Rising State");
        startPos = nc.GetIdlePoint();
        endPos = nc.GetActivePoint();

        Debug.Log("Start Pos: " + startPos);
        Debug.Log("End Pos: " + endPos);
        nc.timer += Time.deltaTime;

        Vector2 test = Vector2.Lerp(startPos, endPos, nc.timer);
        Debug.Log("Test -> " + test);

        nc.ActivateNurikabe(startPos, endPos, nc.timer);
        nc.SetCurrentPos(nc.transform.position);

    }

    public override void Reason(Transform player, Transform npc)
    {
        NurikabeFSMController nc = npc.GetComponent<NurikabeFSMController>();

        if (nc.currentPos.y >= endPos.y)
        {
            nc.PerformTransition(Transition.NurikabeActive);
        }

        if (nc.health <= 0)
        {
            nc.PerformTransition(Transition.EnemyNoHealth);
        }
    }
}
