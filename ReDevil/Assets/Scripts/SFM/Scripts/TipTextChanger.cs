using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipTextChanger : MonoBehaviour
{

    [SerializeField] int maxIndex;
    public string[] tipsText;
    public Text text;
    public int index;
    public float timer;
    public float maxTime;

    // Start is called before the first frame update
    void Start()
    {

        index = 0;
        timer = 0;
        text.text = tipsText[index];
      
    }

    // Update is called once per frame
    void Update()
    {
       if (timer < maxTime)
       {
            timer += Time.deltaTime;
       }

        else
        {
            LoopIndex();
            timer = 0;
        }
    }

    public void LoopIndex()
    {

        index++;

        if (index == maxIndex)
        {
            index = 0;
        }

        TipTextChange();

    }

    public void TipTextChange()
    {

          for (int i = 0; i < tipsText.Length; i++)
          {
             if (i == index)
             {
                 text.text = tipsText[i];
             }
          }

    }
}
