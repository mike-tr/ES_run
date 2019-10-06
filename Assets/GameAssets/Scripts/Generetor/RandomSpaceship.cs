using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpaceship : MonoBehaviour
{
    public static RandomSpaceship instance;

    public Sprite[] parts;

    private void Start() {
        instance = this;
    }

    public void AddRandomParts(Transform target, int layer) {
        List<int> added = new List<int>();
        float hue = Random.value;
        float wt = (Random.value * 0.5f - 0.25f) + 0.5f;
        int maxParts = (int)((parts.Length - 1) * Random.value) + 1;
        for (int i = 0; i < maxParts; i++) {
            int npartId = randomIndex(added);
            added.Add(npartId);
            hue += Random.value * 0.15f;
            hue = Mathf.Clamp(hue, 0, 1);
            wt += Random.value * 0.25f;
            wt = Mathf.Clamp(wt, 0, 1);
            Transform newP = getNewPart(npartId, Color.HSVToRGB(hue, wt, 1f));
            newP.parent = target;
            newP.localPosition = Vector2.zero;
            newP.localEulerAngles = Vector2.zero;
            newP.localScale = Vector2.one;

            newP.gameObject.layer = layer;
        }
    }

    public int randomIndex(List<int> excludeIndex) {
        int target = 0;
        target = increaseIfExist(target, excludeIndex);
        float cv = Random.value;
        for (int i = 1; i < parts.Length;) {
            float tv = Random.value;
            if(tv > cv) {
                cv = tv;
                target = i;
            }
            i++;
            i = increaseIfExist(i, excludeIndex);
        }
        return target;
    }

    public int increaseIfExist(int start, List<int> exclude) {
        foreach(int i in exclude) {
            if (i == start) {
                start++;
                return increaseIfExist(start, exclude);
            }             
        }
        return start;
    }

    public Transform getNewPart(int id, Color color) {
        GameObject part = new GameObject("spaceship p:" + id);
        SpriteRenderer renderer = part.AddComponent<SpriteRenderer>();
        renderer.sprite = parts[id];
        renderer.color = color;
        renderer.sortingOrder = id;
        part.AddComponent<PolygonCollider2D>().isTrigger = true;
        return part.transform;
    }


}
