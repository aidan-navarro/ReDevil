using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molotov : Bullet
{

    protected override void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        rig.velocity = direction * speed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        //don't update every frame like regular bullet since we want physics to do the work
    }
}
