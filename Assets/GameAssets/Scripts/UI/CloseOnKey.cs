using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseOnKey : MonoBehaviour
{
    public KeyCode key;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key)) {
            gameObject.SetActive(false);
            UISystem.instance.saveAudio();
        }
    }
}
