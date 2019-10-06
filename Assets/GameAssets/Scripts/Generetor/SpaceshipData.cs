using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipData : MonoBehaviour
{
    private Dictionary<int, Transform> parts = new Dictionary<int, Transform>();
    public List<int> getPartsids() {
        return new List<int>(parts.Keys);
    }

    public void Start() {
        RandomSpaceship.instance.AddRandomParts(transform, gameObject.layer);
    }
}
