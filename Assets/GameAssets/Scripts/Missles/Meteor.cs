using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : Missle
{
    Dictionary<Collider2D, UnitLife> expelUnits = new Dictionary<Collider2D, UnitLife>();

    public ParticleReuasable smallExplosion;

    protected override void onStart() {
        //velocity += CameraFlow.getSpeed * Vector3.up * Random.value * 0.5f;

        MeteorGenerator.instance.AddMeteor(transform, gameObject.layer);
    }

    protected override void onReset() {
        setLifeLoad = RandomEnemies.maxDifficulty * 50 + 1000;
        base.onReset();
        
    }

    public override void calculateProperties() {
        velocity = CameraFlow.position - (Vector2)transform.position;
        velocity *= (Random.value * .5f + 0.5f) * .05f;

        velocity.x *= (.75f + Random.value * 0.25f);
        velocity.y *= (.75f + Random.value * 0.25f);

        velocity.y -= 0.2f * Random.value;

        if (velocity.sqrMagnitude > 4)
            velocity = velocity.normalized * 2;

        velocity.z = 0;
    }

    protected override void checkLifeTime() {
        lifeTime += Time.fixedDeltaTime;
        if (lifeTime > 25 && !CameraFlow.inScreenView(transform.position, 0.1f)) {
            dumpObject();
        }
        //null
    }

    protected override void onUpdate() {
        foreach (UnitLife unit in expelUnits.Values) {
            Transform es = unit.transform;
            es.position += (es.position - transform.position) * 2f * Time.deltaTime;
        }
    }

    protected override void onHitShip(UnitLife unit, Collider2D collision) {
        expelUnits.Add(collision, unit);
        if (collidedUnits.Contains(unit))
            return;
        collidedUnits.Add(unit);
        damageTarget(unit);
        StartCoroutine(referesh(unit, collision));
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(expelUnits.ContainsKey(collision))
            expelUnits.Remove(collision);
    }

    protected override bool dumpCondition() {
        return true;
    }

    public void damageTarget(UnitLife unit) {
        Transform exp = ParticleSpawner.instance.getReusable(smallExplosion).transform;
        exp.position = (unit.transform.position + transform.position) * .5f;
        unit.ApplyDamage(3);    
    }

    protected override void OnCollision() {
        // Do nothing!
    }
    IEnumerator referesh(UnitLife unit, Collider2D collider) {
        yield return new WaitForSeconds(0.35f);
        if (!expelUnits.ContainsKey(collider)) {
            collidedUnits.Remove(unit);
            yield break;
        } else {
            damageTarget(unit);
            yield return referesh(unit, collider);
        }
    }
}
