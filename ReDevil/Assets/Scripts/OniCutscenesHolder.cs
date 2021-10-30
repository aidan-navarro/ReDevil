using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class OniCutscenesHolder : MonoBehaviour
{
    [SerializeField]
    private TimelineAsset OniDeathCutscene;
    [SerializeField]
    private TimelineAsset OniCycloneSmashCutscene;
    [SerializeField]
    private TimelineAsset OniIntroCutscene;

    private OniFSMController oniBoss;
}
