using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorGenerator : MonoBehaviour
{
    public static MeteorGenerator instance;
    public void Start() {
        instance = this;
    }

    public Sprite[] meteors;
    public Sprite[] rocks;

    public Color Dark;
    public Color Bright;

    public void AddMeteor(Transform parent, int layer) {
        GameObject addM = new GameObject("meteor");
        SpriteRenderer renderer = addM.AddComponent<SpriteRenderer>();
        renderer.sprite = meteors[(int)(Random.value * meteors.Length)];
        renderer.color = Color.Lerp(Bright, Dark, Random.value);
        renderer.sortingOrder = 0;
        renderer.sortingLayerName = "Top";
        addM.AddComponent<PolygonCollider2D>().isTrigger = true;
        addM.layer = layer;
        addM.transform.parent = parent;
        addM.transform.localPosition = Vector2.zero;
        addM.transform.localEulerAngles = Vector3.forward * Random.value * 360;
        addM.transform.localScale = Vector2.one * Random.Range(0.5f, 1.5f);

        GameObject addR = new GameObject("rocks");
        renderer = addR.AddComponent<SpriteRenderer>();
        renderer.sprite = rocks[(int)(Random.value * rocks.Length)];
        renderer.color = Color.Lerp(Bright, Dark, Random.value);
        renderer.sortingOrder = 1;
        renderer.sortingLayerName = "Top";
        addR.transform.parent = parent;
        addR.transform.localPosition = Vector2.zero;
        addR.transform.localEulerAngles = Vector3.forward * Random.value * 360;
        addR.transform.localScale = Vector2.one * Random.Range(0.8f, 1.2f);
    }
}
