using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    private SpriteRenderer[] renderers;
    public Color color = Color.white;

    public bool random = true;
    public bool randomAlpha = false;

    // Start is called before the first frame update
    void Start()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        if (random) {
            float b = Random.value;
            float h = Random.value;
            b = b > h ? b : Random.value;
            color = Color.HSVToRGB(h, b ,1);
            if (randomAlpha)
                color.a = Random.Range(0.5f, 0.1f);
        }

        ColorAll();
    }

    public void ColorAll() {
        foreach(SpriteRenderer sr in renderers) {
            sr.color = color;
        }
    }
    
}
