using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingMissle : Missle
{
    float scale;
    protected override void onDeath() {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * scale, (1 / exploshion_time) * Time.fixedDeltaTime);
    }
}
