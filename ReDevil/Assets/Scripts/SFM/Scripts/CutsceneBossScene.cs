using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneBossScene : MonoBehaviour
{
    public bool canContinue = false;
    public float waitTime;


    // Start is called before the first frame update
    void Start()
    {
        canContinue = false;
        StartCoroutine("Wait");

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        canContinue = true;
      
    }

}
