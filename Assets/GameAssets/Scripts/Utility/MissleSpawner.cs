using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleSpawner : Spawner<Missle>
{
    public static MissleSpawner instance;

    private void Start() {
        instance = this;
    }

    public void Update() {
        if(log)
            UISystem.Log(logStats());
    }
}
