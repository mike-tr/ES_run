using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missle : MonoBehaviour, IReusable
{
    private static int LASTID = 0;

    public int id { get; private set; }

    public float exploshion_time = 0.25f;
    public bool dead = false;

    public float SlowFactor = 1;
    public float damage = 15f;

    public float maxLifeTime = 4f;
    public Vector3 velocity;

    public float followSpeed = 5;
    public ParticleReuasable exploshion;

    [SerializeField]protected float setLifeLoad = 1f;
    protected float clife = 1f;

    public bool RandomScale = false;
    public Vector2 scaleMinMAx;

    public Transform target;
    private bool reusable = false;

    public IEnumerator rangeCheacker;

    public Vector2 cameraSpeedRanges = new Vector2(0.25f, 0.75f);

    public SpriteRenderer renderer;

    public TrailRenderer trail;

    private bool destroyed = false;
    protected float lifeTime = 0;
    private void Start() {
        dead = false;
        id = LASTID++;
        clife = setLifeLoad;

        target = null;

        rangeCheacker = checkDumpCondition();
        StartCoroutine(rangeCheacker);

        onStart();
        if(!renderer)
            renderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected virtual void onReset() {
        lifeTime = 0;
    }

    protected virtual void onStart() {

    }

    public void setColor(Color color) {
        renderer.color = color;
        if (trail)
            trail.startColor = color;
    }

    private float time;
    private void FixedUpdate() {
        transform.position += velocity * Time.deltaTime;
        velocity *= SlowFactor;

        onUpdate();
        if (dead) {
            onDeath();
            onDestroy();
            transform.localScale *= 0.95f;
            velocity *= 0.9f;
            if(transform.localScale.x < 0.5) {
                dumpObject();
            }

        }
        checkLifeTime();
    }

    protected virtual void checkLifeTime() {
        lifeTime += Time.fixedDeltaTime;

        if (lifeTime > maxLifeTime) {
            if (transform.localScale.x < 0.6f) {
                dumpObject();
                return;
            }
            transform.localScale *= 0.995f;
        }
    }

    protected virtual void onUpdate() {
        if (target != null)
            transform.position = Vector3.Lerp(transform.position, target.position, Time.fixedDeltaTime * followSpeed);
    }

    protected virtual void onDestroy() {
        if (destroyed)
            return;

        foreach (UnitLife unit in collidedUnits)
            unit.ApplyDamage(damage);
        if (!CameraFlow.inScreenView(transform.position, 0.01f))
            return;
        Transform exp = ParticleSpawner.instance.getReusable(exploshion).transform;
        exp.position = transform.position + Vector3.one * Random.Range(0, 0.1f);
        destroyed = true;
        //dumpObject();
    }

    protected virtual void onDeath() {

    }

    IEnumerator checkDumpCondition() {
        yield return new WaitForSeconds(1f);
        while (dumpCondition()) {
            yield return new WaitForSeconds(1f);
            
        }

        dumpObject();
    }

    protected virtual void dumpObject() {
        if (reusable) {
            if (MissleSpawner.instance.dumpObject(this)) {
                transform.localScale = Vector2.one;
                transform.position = Vector3.right * 100;

                if (trail)
                    trail.Clear();
                return;
            }
        }
        StopCoroutine(rangeCheacker);
        Destroy(gameObject);
    }

    protected virtual bool dumpCondition() {
        return CameraFlow.inScreenView(transform.position, 0.20f);
    }

    public List<UnitLife> collidedUnits = new List<UnitLife>();
    public void OnTriggerEnter2D(Collider2D collision) {
        if (dead)
            return;

        UnitLife unit = collision.GetComponentInParent<UnitLife>();
        if (unit) {
            onHitShip(unit, collision);
        } else {
            Missle missle = collision.GetComponentInParent<Missle>();
            if (missle) {
                if (missle.GetType() == typeof(Meteor))
                    clife = 0;
                clife -= missle.damage;
            }else {
                clife -= 1f;
            }

            if(clife < 0) {
                dead = true;
            } else {
                OnCollision();
            }
        }
    }

    protected virtual void OnCollision() {
        Transform exp = ParticleSpawner.instance.getReusable(exploshion).transform;
        exp.position = transform.position + Vector3.one * Random.Range(0, 0.1f);
    }

    protected virtual void onHitShip(UnitLife unit, Collider2D collision) {
        dead = true;
        if (target == null)
            target = collision.transform;
        collidedUnits.Add(unit);
    }

    public void setReusable() {
        reusable = true;
    }

    public void resetObject() {
        dead = false;
        id = LASTID++;
        clife = setLifeLoad;

        target = null;
        destroyed = false;
        rangeCheacker = checkDumpCondition();
        StartCoroutine(rangeCheacker);
        collidedUnits = new List<UnitLife>();

        onReset();
    }

    public virtual void calculateProperties() {
        //empty
    }

    public void setMissleLife(float life) {
        clife = life;
    }
}
