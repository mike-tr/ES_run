using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : Spawner<ParticleReuasable>
{
    public static ParticleSpawner instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
}
