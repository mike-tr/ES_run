using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    List<Weapon> allWeapons = new List<Weapon>();
    public Weapon[] startingWeapons;
    public float camRatio = 0;

    public int weaponsCount;

    public int addRandomWeaponsCount = 0;
    private AudioSource audioSource;

    public Missle defaultMissle;
    public AudioClip defaultFireSound;
    // Start is called before the first frame update

    private bool reset = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (reset) {
            allWeapons = new List<Weapon>();
            reset = true;
            weaponsCount = 0;
        }

        if (startingWeapons.Length > 0) {
            for (int i = 0; i < startingWeapons.Length; i++) {
                Weapon current = startingWeapons[i];
                current.setSettings(this, audioSource, defaultMissle, defaultFireSound, camRatio);
                allWeapons.Add(current);
                weaponsCount++;
            }
        }

        for (int i = 0; i < addRandomWeaponsCount; i++) {
            addRandomWeapon(Random.value * 1000, (int)(Random.value * 360));
        }
    }

    public int addRandomWeapon(float maxLevel, int angle) {
        if (reset) {
            allWeapons = new List<Weapon>();
            reset = true;
        }

        audioSource = GetComponent<AudioSource>();
        Weapon weapon = new Weapon();
        weapon.setSettings(this, audioSource, defaultMissle, defaultFireSound,camRatio);
        weapon.setLevelOneWeapon();
        int level = weapon.randomWeaponUpgrades(maxLevel, 0);

        weapon.spread = Random.value * 50;
        //weapon.RandomValues(maxLevel, angle);
        allWeapons.Add(weapon);
        weaponsCount++;

        return level;
    }

    public float addRandomWeapon(float diff, int angle, bool random) {
        if (reset) {
            allWeapons = new List<Weapon>();
            reset = true;
        }

        audioSource = GetComponent<AudioSource>();
        Weapon weapon = new Weapon();
        weapon.setSettings(this, audioSource, defaultMissle, defaultFireSound, camRatio);

        weapon.spread = Random.value * 50;
        allWeapons.Add(weapon);
        weaponsCount++;

        return weapon.RandomValues(diff, angle);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < weaponsCount; i++) {
            allWeapons[i].Update();
        }
    }

    public void fire() {
        for (int i = 0; i < weaponsCount; i++) {
            allWeapons[i].fire();
        }
    }

}
