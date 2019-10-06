using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSystem : MonoBehaviour
{
    public static int maxBulletsPerSecond = 150;
    public static int maxPerRound = 50;
    public const float _1d360 = 1f / 180f;


    public float camRatio = 0;

    public Missle Bullet;
    public float missleSpeed = 5f;
    public float reloadTime = 0.1f;

    public int UpShoot = 1;
    public int LeftShoot = 1;
    public int BottomShoot = 1;
    public int RightShoot = 1;

    public float reducedReload = 1;

    public float Angle = 10f;

    public float damage = 10f;

    public float bulletScale = 1;
    public float missleLife = 1;
    public float missleLifeTime = 10f;
    public float OverLoad = 0;
    public float OverloadRatio = 0.001f;
    public float OverloadDownRatio = 0.005f;

    public float deviationRange = 10;

    bool Overloaded = false;

    public Color missleColor = Color.white;

    public float range = 0.8f;

    public float bulletsPerSeoncd;

    [SerializeField]AudioClip fireSound;
    // Start is called before the first frame update
    void Start()
    {
        if(!fireSound)
            fireSound = AudioManager.instance.getRandomClip(playEffect.fire);
    }

    public void Update() {
        if (OverLoad > 0) {
            OverLoad -= OverloadDownRatio * Time.deltaTime;
            if (Overloaded) {
                //OverLoad = Mathf.Clamp(OverLoad - OverloadDownRatio * 4, 0, 2);
                if (OverLoad <= 0.05) {
                    Overloaded = false;
                }
            }
        }
    }

    public float RandomValues(float damagePerSecond) {
        //give random bullet speed!
        missleSpeed = Random.value * 9 + 1;
        float msV = missleSpeed * 0.2f;
        if (msV < 1)
            msV = 1;

        

        //get random initial damage!
        
        damage = Random.value * 0.3f * damagePerSecond + 1;
        if(damage > 10)
            bulletScale = (damage / damagePerSecond) + 1;


        float c = (25 + damage);
        if (c > 45 && c < 75)
            c += 75 - c;
        c *= _1d360;
        c -= (int)c;
        missleColor = Color.HSVToRGB(c , Random.value * 0.3f + 0.5f, 1);


        missleLife = 5;
        float av = Random.value * 4.5f + 0.5f;
        float ab = Random.value * 4.5f + 0.5f;
        if (av > ab)
            av = ab;
        missleLife *= av;
        //get maximum bullets per second and minimum attack speed (one bullets machine gun!)
        bulletsPerSeoncd = damagePerSecond / (damage * msV * av * 2f);
        if(bulletsPerSeoncd > maxBulletsPerSecond) {
            missleLife *= (1 + (bulletsPerSeoncd - maxBulletsPerSecond) / maxBulletsPerSecond);
        }
        missleLife += damagePerSecond * 0.05f;

        float bullets =  (int)(bulletsPerSeoncd * 5 * Random.value);
        if (bullets < 1)
            bullets = 1;
        else if (bullets > maxPerRound)
            bullets = maxPerRound;
        float attackSpeed = bulletsPerSeoncd / bullets;

        if(bullets > 4) {
            float upb = Random.value * 0.7f + 0.3f;
            UpShoot = Mathf.RoundToInt(upb * bullets) + 1;
            LeftShoot = Mathf.RoundToInt((1 - upb) * 0.5f * bullets);
            RightShoot = LeftShoot;
        } else {
            UpShoot = (int)bullets;
        }
        
        reloadTime = 1 / attackSpeed;

        deviationRange = Random.Range(10, 50);
        Angle = Random.Range(3, 42);

        OverloadRatio = 0.0f;
        OverloadDownRatio = 1f;

        damage += 3;
        return (bulletsPerSeoncd * damage) + bullets * av * msV;
    }

    float lastFire = 0;
    public void FireMissle(float offset, float OriginalAngle) {
        //Missle missle = Instantiate(Bullet);
        Missle missle = MissleSpawner.instance.getReusable(Bullet);

        //missle.transform.position = transform.position + offset;      
        
        //missle.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(Vector2.up, direction));
        //float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        missle.transform.rotation = Quaternion.Euler(0f, 0f, OriginalAngle);
        missle.transform.position = missle.transform.up * offset + transform.position;
        missle.velocity = missle.transform.up * missleSpeed + CameraFlow.getSpeed * Vector3.up * camRatio;
        missle.setMissleLife(missleLife);
        missle.setColor(missleColor);
        missle.damage = damage;
        missle.transform.localScale = Vector3.one * bulletScale;
        missle.maxLifeTime = missleLifeTime;
    }

    private float deviation;
    public void Fire() {
        if (Overloaded)
            return;
        if (OverLoad > 1) {
            Overloaded = true;
        }

        if (lastFire > Time.timeSinceLevelLoad)
            return;

        AudioManager.playOnce(fireSound, 1);


        lastFire = Time.timeSinceLevelLoad + (reloadTime * reducedReload);
        FireAllDir(UpShoot, Angle, deviation + transform.eulerAngles.z);
        FireAllDir(BottomShoot, Angle, deviation + 180 + transform.eulerAngles.z);
        FireAllDir(RightShoot, Angle, deviation + -90 + transform.eulerAngles.z);
        FireAllDir(LeftShoot, Angle, deviation + 90 + transform.eulerAngles.z);
        //OverLoad += OverloadRatio * UpShoot + OverloadRatio * RightShoot + OverloadRatio * LeftShoot;
        OverLoad += OverloadRatio;

        deviation = Random.Range(-deviationRange, deviationRange);
    }

    public void FireAllDir(int count ,float OffsetAngle ,float middleAngle) {
        float startAngle = middleAngle - OffsetAngle * (count * .5f);
        for (int i = 0; i < count; i++) {
            FireMissle(range, startAngle);
            startAngle += OffsetAngle;
        }

        //Vector3 addOf = Offset * 0.3f;
        //Vector3 currentOffset = -addOf * (count / 2);       
        //for (int i = 0; i < count; i++) {
        //    FireMissle(currentOffset + direction * range, direction);
        //    currentOffset += addOf;
        //}
    }
}
