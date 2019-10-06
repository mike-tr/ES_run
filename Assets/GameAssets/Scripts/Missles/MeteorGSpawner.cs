using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorGSpawner : MonoBehaviour
{
    public float spawnRate = 1f;

    public int avgMeteors = 5;
    public float chance = 0.1f;
    public Meteor prefab;
    Camera cam;
    SpawnObjectHolder<Missle> meteorHolder;


    public int liveM = 0;
    public void Start() {      
        cam = Camera.main;
        meteorHolder = MissleSpawner.instance.getHolder(prefab);
        StartCoroutine(spawnLoop());
    }

    public static int ID = 0;
    IEnumerator spawnLoop() {
        while (true) {
            yield return new WaitForSeconds(spawnRate);
            float c = ((avgMeteors - meteorHolder.live) * chance);
            if (c < chance)
                c = chance;
            liveM = meteorHolder.live;
            if(Random.value < c) {
                Missle meteor = MissleSpawner.instance.getReusable(prefab);
                Vector2 spawnPoint = CameraFlow.outSideScreen(MathHelper.DegreeToVector2(Random.value * 360));
                meteor.transform.position = spawnPoint;
                meteor.calculateProperties();
            }
        }
    }
}
