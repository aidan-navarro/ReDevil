using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSprite : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 2);
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
