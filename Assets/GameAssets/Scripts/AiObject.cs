using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiObject : MonoBehaviour
{
    public static int MaxLife = 500;
    public static int Enemies = 0;
    public Transform target;
    FireSystem fireSystem;
    public float speed = 5f;
    public float Range = 5f;
    public float fleeRange = 5f;

    public float rotationSpeed = 5f;

    public ParticleReuasable exploshion;

    UnitLife life;

    float goldDrop = 0;
    bool suicider;

    float camRatio;

    private float difficulty;
    private bool inRange = false;
    private float shootRange = 0;
    private float AdRange = 0;

    public int behaviorType = 1;

    float repositionChance = 0;

    private bool isBoss = false;
    // Start is called before the first frame update

    float maxLife = 1;
    void Start()
    {
        Enemies++;
        life = GetComponent<UnitLife>();
        target = PlayerController.player;
        fireSystem = GetComponent<FireSystem>();

        float maxPowerLevel = RandomEnemies.maxDifficulty;
        float powerLevel = maxPowerLevel;
        if (Random.value < RandomEnemies.BossChance) {
            powerLevel *= 10f;
            RandomEnemies.BossChance = 0;
            isBoss = true;
            //AudioManager.playOnce(AudioManager.instance.bossIsCommingClip);
            AudioManager.playRepeatedEffect(AudioManager.instance.bossIsCommingClip, 6, 2, 1);
        }
        else {
            powerLevel = maxPowerLevel * (Random.value + 2) * 0.05f;
            float reducePower = maxPowerLevel * (Random.value + 2) * 0.05f;
            if (powerLevel > reducePower)
                powerLevel = reducePower;
        }

        repositionChance = Random.value * 0.1f;

        camRatio = Random.value * 0.5f;
        fireSystem.camRatio = camRatio;

        difficulty = fireSystem.RandomValues(powerLevel);

        float vv = (difficulty / RandomEnemies.maxDifficulty);
        float Rv = 0.75f + .15f * vv;
        
        life.life = (difficulty * 2) + 35;
        life.life = Mathf.Clamp(life.life, RandomEnemies.maxDifficulty + 35 , RandomEnemies.maxDifficulty * 20);
        maxLife = life.life;

        float scale = Rv < 2.5f ? Rv : 2.5f;
        transform.localScale = scale * Vector3.one;

        goldDrop = difficulty * 3 + 5;
        if(goldDrop > RandomEnemies.maxDifficulty * 10) {
            goldDrop *= 0.25f + Random.value * 0.75f;
        }

        rotationSpeed = 2;
        behaviorType = 1;
        Range = 0;
        fleeRange = 0;



        speed = Random.value * 4f + 1;

        if (vv < 1 || Random.value > 0.85) {
            if (Random.value > 0.7) {
                behaviorType = 0;
                Range = Random.Range(6f, 12f);
                AdRange = Range * .05f + 1f;
                shootRange = Random.Range(Range * 1.5f, Range * 2.5f);
                fleeRange = Range * 0.5f * Random.value;

            } else {
                suicider = Random.value > vv * 4f ? true : false;
                if (suicider) {
                    if (Random.value > .5f) {

                        difficulty *= .5f;

                        Range = 0;
                        speed = 3.25f / Rv;
                        speed = Mathf.Clamp(speed, 2.5f, 5f);

                        StartCoroutine(DestroySelf(7.5f));

                        rotationSpeed = 6f;
                        life.life *= 0.75f;
                        camRatio = 0;
                    }
                }
            }
        }

        StartCoroutine(damageColliding());

        stayPos = CameraFlow.randomScreenPos();
        RandomEnemies.liveDifficulty += difficulty;

        StartCoroutine(CheckDestroy());

        life.onGetHitCallback += onGethit;
    }
    float healthPercent = 1;
    float _1dhealthp;
    public void onGethit() {
        healthPercent = life.life / maxLife;
        _1dhealthp = 1 / healthPercent;
        if(healthPercent < 0.8f) {
            float r = healthPercent;
            if (r < 0.27)
                r = 0.27f;
            fireSystem.reducedReload = r * 1.25f;
        }
    }

    Vector3 velocity;
    // Update is called once per frame
    bool onDestroying = false;
    float timeE = 0;
    void FixedUpdate()
    {
        if(life.IsDead) {
            if (onDestroying) {
                difficulty -= RandomEnemies.maxDifficulty * 5 *  Time.deltaTime;
                timeE += Time.fixedDeltaTime;
                if (timeE > 0.25f) {
                    Transform expl = ParticleSpawner.instance.getReusable(exploshion).transform;
                    expl.position = transform.position + transform.localScale * Random.Range(-0.33f, 0.33f);
                    timeE = 0;
                }

                transform.position += velocity;
                velocity *= 0.95f;
                if (difficulty < 0)
                    destroySelf();
                return;
            }
            RandomEnemies.liveDifficulty -= difficulty;
            Transform exp = ParticleSpawner.instance.getReusable(exploshion).transform;
            exp.position = transform.position + transform.localScale * Random.Range(-0.33f, 0.33f);
            PlayerController.instance.AddGold(goldDrop);
            GameData.score += (int)(difficulty * 8);
            if (isBoss)
                RandomEnemies.maxDifficulty += difficulty * 0.05f;
            else
                RandomEnemies.maxDifficulty += difficulty * 0.1f;
            RandomEnemies.BossChance += 0.025f;
            onDestroying = true;
            return;
        }

        //transform.position += Vector3.up * CameraFlow.getSpeed * camRatio * Time.fixedDeltaTime;
        behavior();
        transform.position += velocity;
        velocity *= 0.95f;

    }

    Vector2 stayPos;
    public void behavior() {
        if (!target)
            return;
        switch (behaviorType) {
            case 0:
                chaseTarget(target.position);
                break;
            case 1:
                if(Vector2.Distance(stayPos, transform.position) > 1.5f) {
                    chaseTarget(stayPos);
                    fireSystem.Fire();
                } else {
                    Vector2 diff = (Vector2)target.position - (Vector2)transform.position;                     
                    float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Lerp(transform.rotation,
                        Quaternion.Euler(0f, 0f, rot_z - 90), Time.fixedDeltaTime * rotationSpeed);
                    fireSystem.Fire();
                }          
                break;
        }
    }

    void chaseTarget(Vector2 pos) {
        if (target) {
            Vector2 diff = pos - (Vector2)transform.position;

            float distance = diff.magnitude;
            if (suicider) {
                velocity += transform.up * speed * .1f * Time.fixedDeltaTime;
                if (distance > 4)
                    fireSystem.Fire();
            } else if (distance < Range) {

                if (!inRange) {
                    inRange = true;
                    Range += AdRange;
                }
                if (distance < fleeRange) {
                    velocity -= transform.up * speed * Time.fixedDeltaTime * .1f;
                }
            } else {
                if (inRange) {
                    inRange = false;
                    Range -= AdRange;
                }
                velocity += transform.up * speed * Time.fixedDeltaTime * .1f;
            }

            if (distance < shootRange)
                fireSystem.Fire();

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            if (distance > 1)
                distance = 1;
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.Euler(0f, 0f, rot_z - 90), Time.fixedDeltaTime * rotationSpeed);
            return;
        }
        return;
    }

    int cycles = 0;
    IEnumerator CheckDestroy() {
        int sc = 0;
        cycles = (int)(Random.value * 20f) + 10;
        if (isBoss)
            cycles += 20;
        while (Vector2.Distance(transform.position, CameraFlow.position) < 100) {
            if (transform.position.y < CameraFlow.getScreenMinBorderY()) {
                break;
            }
            sc++;
            if(sc > 7) {
                speed *= 0.75f;
                sc = 0;
            }

            if(Random.value < repositionChance) {
                if(Random.value > 0.92) {
                    behaviorType = 0;
                    Range = Random.Range(6f, 12f);
                    AdRange = Range * .05f + 1f;
                    shootRange = Random.Range(Range * 1.5f, Range * 2.5f);
                    fleeRange = Range * 0.5f * Random.value;
                }
                stayPos = CameraFlow.randomScreenPos();
            }

            if(healthPercent < 0.9f) {
                cycles--;
                if (cycles < 0 && Random.value > 0.6) {
                    speed += 3;
                    stayPos = Vector2.down * 100 + Vector2.right * (Random.value - 0.5f) * 200;
                    behaviorType = 1;
                    Range = 1;
                }
            }

            yield return new WaitForSeconds(1f);
        }
        destroySelf();
    }

    IEnumerator DestroySelf(float time) {
        yield return new WaitForSeconds(time);
        Transform exp = ParticleSpawner.instance.getReusable(exploshion).transform;
        exp.position = transform.position + Vector3.one * Random.Range(0, 0.1f);
        destroySelf();
    }

    Dictionary<int, UnitLife> unitsHit = new Dictionary<int, UnitLife>();
    List<UnitLife> collidedObjects = new List<UnitLife>();
    private void OnTriggerEnter2D(Collider2D collision) {
        UnitLife life = collision.GetComponentInParent<UnitLife>();
        if (life) {
            if (!unitsHit.ContainsKey(collision.GetInstanceID())) {
                unitsHit.Add(collision.GetInstanceID(), life);

            }

            if (!collidedObjects.Contains(life)) {
                collidedObjects.Add(life);
                life.onUnitsCollision(this.life);
                Transform exp = ParticleSpawner.instance.getReusable(exploshion).transform;
                exp.position = transform.position + Vector3.one * Random.Range(0, 0.1f);
            }
                    
            //PlayerController.gold += goldDrop;          
            //life.ApplyDamage(fireSystem.damage);
            if (suicider) {
                life.ApplyDamage(fireSystem.damage * 2);
                destroySelf();
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision) {
        int id = collision.GetInstanceID();
        if (unitsHit.ContainsKey(id)) {
            unitsHit.Remove(id);
        }
    }

    void destroySelf() {
        Enemies--;
        RandomEnemies.liveDifficulty -= difficulty;
        Destroy(gameObject);
    }

    IEnumerator damageColliding() {
        while (true) {
            foreach(UnitLife life in unitsHit.Values) {
                life.onUnitsCollision(this.life);         
                Transform exp = ParticleSpawner.instance.getReusable(exploshion).transform;
                exp.position = transform.position + Vector3.one * Random.Range(0, 0.1f);
                collidedObjects.Add(life);
            }       
            yield return new WaitForSeconds(0.2f);
            collidedObjects = new List<UnitLife>();
        }
    }
}
