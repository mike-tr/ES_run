using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickKeyboard : MonoBehaviour
{
    public KeyCode key;

    public Button button;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key)) {
            button.onClick.Invoke();
        }
    }
}
