using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleReuasable : MonoBehaviour , IReusable
{
    private static int particalIDS = 10000;

    public float lifeTime = 2f;
    private bool reusable = false;

    ParticleSystem system;

    IEnumerator dumper;

    public playEffect effect = playEffect.hit;

    public int id = 0;
    private void Start() {
        system = GetComponent<ParticleSystem>();
        resetObject();
    }

    public void resetObject() {
        id = particalIDS;
        particalIDS++;
        dumper = dumpOnEnd();
        StartCoroutine(dumper);
        system.Play();
        if(effect == playEffect.hit)
            AudioManager.playMediumEffect(0.8f);
        else {
            AudioManager.playOther(effect, 0.8f);
        }
    }

    public void setReusable() {
        reusable = true;
    }

    IEnumerator dumpOnEnd() {
        yield return new WaitForSeconds(lifeTime);
        if (reusable) {
            //check if save was possible if not destroy.
            if (ParticleSpawner.instance.dumpObject(this)) {
                yield break;
            }
        }
        Destroy(gameObject);
    }

}
