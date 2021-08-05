using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OniEasterEgg : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private GameObject secretMessage;

    private GameplayControls easterEggControls;

    [SerializeField]
    private bool canPress;
    private int counter;
    // Start is called before the first frame update
    void Awake()
    {
        easterEggControls = new GameplayControls();
        easterEggControls.EasterEgg.HitOni.performed += ctx => HitOni();
        counter = 0;
        secretMessage.SetActive(false);
        canPress = true;
    }

    private void HitOni()
    {
        if(canPress)
        {
            canPress = false;
            Debug.Log("Smack Oni");
            counter++;
            if (counter < 150)
            {
                //Oni loses life points
                animator.SetBool("Hurt", true);
            }
            else
            {
                //Oni's life points have reached 0.  He lost the duel
                animator.SetBool("Dead", true);
            }
        }
        else
        {
            //do nothing.
        }
        
        

    }

    private void ExitHurtAnimation()
    {
        animator.SetBool("Hurt", false);   
    }

    private void SetCanPress()
    {
        canPress = true;
    }

    private void OniDead()
    {
        secretMessage.SetActive(true);
    }

    private void OnEnable()
    {
        easterEggControls.Enable();
    }

    private void OnDisable()
    {
        easterEggControls.Disable();
    }
}
