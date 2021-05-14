using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTimeWait : MonoBehaviour
{
    public int index; //what number i am on
    [SerializeField] bool keyDown;
    [SerializeField] int maxIndex; //max number of buttons

    public GameObject[] csButtons; //buttons
  

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine("testWait");

        index = 0;
        keyDown = false;

       //for (int i = 0; i < underlines.Length; i++)
       //{
       //    underlines[i].enabled = false;
       //}
    }

    // Update is called once per frame
    void Update()
    {
       // ChangeOptions();

        if (Input.GetButtonDown("Submit"))
        {
            ChangeMenus();
        }
        


        if (!keyDown)
        {
            if (Input.GetAxis("Horizontal") > 0.5)
            {
                keyDown = true;
                StartCoroutine("IncreaseIndex");

            }

            else if (Input.GetAxis("Horizontal") < -0.5)
            {
                keyDown = true;
                StartCoroutine("DecreaseIndex");
            }

        }
        else if ((Input.GetAxis("Horizontal") < 0.5 && Input.GetAxis("Horizontal") > -0.5) && keyDown)
        {
            keyDown = false;
            StopAllCoroutines();
        }
    }

    //public void ChangeOptions()
    //{
    //    for (int i = 0; i < underlines.Length; i++)
    //    {
    //        if (i == index)
    //        {
    //            underlines[i].enabled = true;
    //        }
    //
    //        else
    //        {
    //            underlines[i].enabled = false;
    //        }
    //    }
    //
    //}
    public void ChangeMenus() //switching menus,  similar to how change options is built
    {
        if (index == 0)
        {
           //start from checkpoint
        }
        else if (index == 1) //changing screens. first one is current, second one is next screen. menuController is to access anything in the menuController script that is public
        {
            //restart
            SceneManager.LoadScene(0);
        }
        else if (index == 2) //changing screens. first one is current, second one is next screen. menuController is to access anything in the menuController script that is public
        {
           //main menu
        }

    }

    public IEnumerator testWait()
    {
        yield return new WaitForSeconds(3.0f);
        
    }

    public IEnumerator IncreaseIndex()
    {

        index++;

        if (index == maxIndex)
        {
            index = 0;
        }

        yield return new WaitForSeconds(0.2f);

        keyDown = false;
    }

    public IEnumerator DecreaseIndex()
    {

        index--;

        if (index < 0)
        {
            index = maxIndex - 1;
        }

        yield return new WaitForSeconds(0.2f);

        keyDown = false;
    }
}
