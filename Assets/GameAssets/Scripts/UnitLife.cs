using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitLife : MonoBehaviour {


    public delegate void OnGetHit();
    public OnGetHit onGetHitCallback;

    public float life = 100;
    public float maxDamage = -1;
    public float hitDamage = 4;

    public bool isPlayer = false;

    public bool IsDead { get => life < 0; }

    public bool AlwaysOne = false;

    public void onUnitsCollision(UnitLife colliding) {
        colliding.ApplyDamage(hitDamage);
        ApplyDamage(colliding.hitDamage);
    }

    public void ApplyDamage(float damage, bool invoke = true) {
        if (isPlayer && maxDamage < damage)
            damage = maxDamage;
        life -= damage;
        if (invoke && onGetHitCallback != null)
            onGetHitCallback.Invoke();
    }
}
