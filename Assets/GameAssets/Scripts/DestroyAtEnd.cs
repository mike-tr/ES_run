using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAtEnd : MonoBehaviour
{
    public float LifeTime = 5f;
    // Start is called before the first frame update
    void Start()
    {
        //AudioManager.instance.playAnyThatStartWith("explosion");
    }

    float time = 0;
    // Update is called once per frame
    void Update()
    {
        if (time > LifeTime)
            Destroy(gameObject);
        time += Time.deltaTime;
    }
}
