using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum weaponUpgrade {
    attackSpeed,
    damage,
    overloadPerFire,
    overloadReduce,
    bulletLife,
    missleSpeed,
    missleCount,
    bulletsLifeTime,
}

[System.Serializable]
public class Weapon
{
    private const int maxBullets = 30;

    public float missleSpeed = 5f;
    public float reloadTime = 0.1f;

    public int bulletsCount = 1;
    public float spread = 10f;
    public float startAngle = 0;

    public float damagePerBullet = 5f;
    public float bulletLife = 1;

    public float OverLoad = 0;
    public float OverloadRatio = 0.001f;
    public float OverloadDownRatio = 0.005f;

    public float bulletsLifetime = 4f;

    bool Overloaded = false;

    [HideInInspector]public float camRatio = 0;
    [HideInInspector]public Missle missle;
    [HideInInspector]public AudioClip audioClip;
    [HideInInspector]public AudioSource audioSource;
    [HideInInspector]public WeaponSystem parentSystem;
    [HideInInspector]public float offset = .8f;
    private Transform transform;
    [SerializeField]private int cooldown = 0;
    private float extendedOverload = 0f;

    public void setSettings(WeaponSystem parentSystem, AudioSource source, Missle missle, AudioClip clip, float camRatio) {
        this.parentSystem = parentSystem;
        this.missle =  missle;
        this.audioClip = clip;
        transform = parentSystem.transform;
        this.audioSource = source;
        this.camRatio = camRatio;
    }


    public void Update() {
        if (OverLoad > 0) {
            if (Overloaded) {
                OverLoad -= OverloadDownRatio * Time.deltaTime;
                //OverLoad = Mathf.Clamp(OverLoad - OverloadDownRatio * 4, 0, 2);
                if (OverLoad <= 0.05) {
                    Overloaded = false;
                    cooldown = 0;
                    extendedOverload = 0;
                }
            } else if(cooldown < 50){
                OverLoad -= OverloadDownRatio * Time.deltaTime;
            }
        }
        if (cooldown > 0) {
            --cooldown;
            extendedOverload *= 0.95f; 
        }
    }

    public void setLevelOneWeapon() {
        missleSpeed = 1;
        reloadTime = 1.5f;
        bulletsCount = 1;
        spread = 8f;
        startAngle = 0;
        damagePerBullet = 5;
        bulletLife = 1;
        bulletsLifetime = 5;

        OverloadRatio = 0.25f;
        OverloadDownRatio = 0.25f;
    }

    public float RandomValues(float level, int startAngle) {

        spread = Random.Range(3, 20f);
        if (Random.value > 0.5f)
            spread += Random.value * 20f;

        int maxb = 30;
        int maxBullets = (int)((Random.value * 0.8f + .2f) * level) + 2;


        int speed = (int)level - maxBullets;
        if (speed < 2)
            speed = 2;

        bulletsCount = (int)(Random.value * maxBullets - 2) + 2;
        if(bulletsCount > maxb) {
            bulletLife = bulletsCount - maxb;
            bulletsCount = (int)(maxb * (0.75f + Random.value * 0.5f));
        }

        float currentDifficulty = bulletsCount;

        float attackSpeed = Random.value * speed * .5f + 0.25f;
        if (attackSpeed > 10)
            attackSpeed = 10;

        reloadTime = 1 / attackSpeed;
        damagePerBullet = Random.Range(1, 10);

        missleSpeed = Random.Range(1.5f, speed * .5f);
        if (missleSpeed < attackSpeed) {
            missleSpeed = Random.Range(missleSpeed, attackSpeed);
        }

        float sr = Random.Range(1.5f, speed * .5f);
        missleSpeed = missleSpeed > sr ? sr : missleSpeed;
        if (missleSpeed > 10)
            missleSpeed = 10;

        currentDifficulty *= missleSpeed * attackSpeed;      


        this.startAngle = startAngle;

        OverloadRatio = Random.Range(0.1f, 0.5f);
        OverloadDownRatio = (OverloadRatio) + .1f;

        return currentDifficulty;
    }

    public int randomWeaponUpgrades(float maxLevel, int startAngle) {
        this.startAngle = startAngle;

        weaponUpgrade[] upgrades = System.Enum.GetValues(typeof(weaponUpgrade)) as weaponUpgrade[];

        int weaponLevel = 0;
        int averageLevel = (int)(maxLevel / upgrades.Length);
        for (int i = 0; i < upgrades.Length; i++) {
            int level = 1 + (int)(Random.value * averageLevel);
            for (int k = 0; k < level; k++) {
                upgradeWeapon(upgrades[i]);
            }
            weaponLevel += level;
        }
        Debug.Log(weaponLevel);

        Debug.Log(bulletsLifetime);
        return weaponLevel;
    }

    float lastFire = 0;
    public void FireMissle(float offset, float OriginalAngle, Vector2 parentVelocity) {
        //Missle missle = Instantiate(Bullet);
        Missle spawnMissle = MissleSpawner.instance.getReusable(missle);

        //missle.transform.position = transform.position + offset;      

        //missle.transform.eulerAngles = new Vector3(0, 0, Vector2.Angle(Vector2.up, direction));
        //float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        spawnMissle.transform.rotation = Quaternion.Euler(0f, 0f, OriginalAngle);
        spawnMissle.transform.position = spawnMissle.transform.up * offset + transform.position;
        float sp = missleSpeed > 10 ? 10 : missleSpeed;
        spawnMissle.velocity = (Vector2)spawnMissle.transform.up * sp + parentVelocity;
        spawnMissle.setMissleLife((int)bulletLife);
        spawnMissle.damage = damagePerBullet;
        spawnMissle.maxLifeTime = bulletsLifetime;
    }

    public void fire() {
        if (Overloaded)
            return;
        if (OverLoad > 1) {
            Overloaded = true;
        }

        if (lastFire > Time.timeSinceLevelLoad)
            return;
        cooldown += 20;
        if (cooldown > 65)
            cooldown = 65;
        extendedOverload = (extendedOverload + .02f) * 1.05f;
        if (audioSource) {
            audioSource.PlayOneShot(audioClip);
        }

        lastFire = Time.timeSinceLevelLoad + reloadTime;
        FireAllDir(bulletsCount, spread, startAngle + transform.eulerAngles.z);
        //OverLoad += OverloadRatio * UpShoot + OverloadRatio * RightShoot + OverloadRatio * LeftShoot;
        OverLoad += OverloadRatio * (1 + extendedOverload);
    }

    public void FireAllDir(int count, float OffsetAngle, float middleAngle) {
        float startAngle = middleAngle - OffsetAngle * (count * .5f);
        for (int i = 0; i < count; i++) {
            FireMissle(offset, startAngle, Vector2.zero);
            startAngle += OffsetAngle;
        }
    }

    public bool upgradeWeapon(weaponUpgrade upgrade, bool downgrade = false) {
        switch (upgrade) {
            case weaponUpgrade.attackSpeed:
                reloadTime *= .965f;
                break;
            case weaponUpgrade.bulletLife:
                bulletLife = (bulletLife * 1.03f) + 1;
                //missleSpeed *= 0.96f;
                break;
            case weaponUpgrade.damage:
                damagePerBullet *= 1.06f;
                break;
            case weaponUpgrade.missleSpeed:
                missleSpeed *= 1.05f;
                break;
            case weaponUpgrade.overloadPerFire:
                OverloadRatio *= 0.98f;
                OverloadDownRatio *= 0.99f;
                break;
            case weaponUpgrade.overloadReduce:
                if (OverloadDownRatio > 1f)
                    return false;
                OverloadDownRatio *= 1.02f;
                OverloadRatio *= 1.01f;
                break;
            case weaponUpgrade.missleCount:
                if (bulletsCount > maxBullets)
                    return false;
                bulletsCount += 1;                
                break;
            case weaponUpgrade.bulletsLifeTime:
                bulletsLifetime *= 1.05f;
                break;
            default:
                return false;
        }
        return true;
    }
}
