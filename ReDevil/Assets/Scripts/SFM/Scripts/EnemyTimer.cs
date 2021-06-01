using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTimer : MonoBehaviour
{
    // State Variables
    public float knockbackTimer;
    private EnemyFSMController ec;

    // Start is called before the first frame update
    void Start()
    {
        ec = GetComponent<EnemyFSMController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // use this to delay enemy reactions
    public IEnumerator EnemyKnockbackTimer()
    {
        yield return new WaitForSeconds(knockbackTimer);
    }
}
