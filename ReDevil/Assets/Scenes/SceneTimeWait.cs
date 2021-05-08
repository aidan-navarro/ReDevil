using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTimeWait : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine("testWait");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator testWait()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(0);
    }
}
