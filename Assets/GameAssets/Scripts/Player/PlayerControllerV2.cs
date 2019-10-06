using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerV2 : MonoBehaviour
{
    public Dictionary<int, UpgradeLogic> upgrades = new Dictionary<int, UpgradeLogic>();

    public static PlayerControllerV2 instance;
    public static float goldWon;
    public static bool inShop = false;

    public static Transform player;
    Camera cam;

    public float speed = 15;
    public WeaponSystem fire;
    UnitLife life;

    public float CurrentEnergy { get => life.life; }
    public float maxEnergy = 5;
    public float energy_regen_start = 2f;


    float time_not_hit;

    public float gold_income = 0;
    private bool loaded = false;

    public float ds;

    public float LifePercentage {
        get { return life.life / maxEnergy; }
    }

    Upgrade[] upgradesObjects;
    // Start is called before the first frame update
    IEnumerator Start() {
        GameData.LoadUpgradeData();

        instance = this;
        life = GetComponent<UnitLife>();
        life.life = maxEnergy;
        life.onGetHitCallback += OnGetDamaged;

        player = transform;
        fire = GetComponent<WeaponSystem>();
        cam = Camera.main;
        fire.camRatio = 1f;

        while (!UISystem.instance) {
            yield return null;
        }

        if (Application.isMobilePlatform) {
            speed *= 0.7f;
            //maybe do something?!
        }

        loaded = true;
    }

    public void AddGold(float amount) {
        goldWon += amount;
    }
    // Update is called once per frame
    public void Update() {
        if (CurrentEnergy < 1)
            return;


        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) {
            //Fire(transform.right, transform.right);
            if (Time.timeScale != 0)
                fire.fire();
            //FireAllDir(UpShoot, transform.right * 0.2f, transform.up);
        }
        Regenerate();
    }

    public void Regenerate() {
        if (life.life < maxEnergy) {
            if (time_not_hit > energy_regen_start) {
                life.life += Time.deltaTime;
                goldWon -= 1f * Time.deltaTime;
            }
        } else {
            //gold_income = RandomEnemies.STR * 0.25f;
            goldWon += RandomEnemies.maxDifficulty * Time.deltaTime * 0.1f;
        }
        time_not_hit += Time.deltaTime;
    }

    private void FixedUpdate() {
        if (CurrentEnergy < 1)
            return;
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        //Vector3 np = Vector3.Lerp(transform.position, mousePos, Time.fixedDeltaTime * this.speed);
        //transform.position = np;

        Vector2 lookDirection = Vector2.zero;
        Vector2 diff = mousePos - (Vector2)transform.position;
        if (Application.isMobilePlatform) {
            diff = Joystick.getFixDirection();
            lookDirection = Joystick.getDirection();
        } else {
            lookDirection = diff;
        }
        //diff = Joystick.instance.Direction * 1f;
        //diff = Joystick.getDirection();

        float speed = Mathf.Clamp(diff.sqrMagnitude, 0, 4);

        transform.position += Vector3.up * CameraFlow.getSpeed * Time.fixedDeltaTime;
        if (diff.sqrMagnitude > ds) {
            transform.position += transform.up * Time.fixedDeltaTime * speed * this.speed;
            transform.position = CameraFlow.instance.setInBounds(transform.position);

            float rot_z = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, rot_z - 90), Time.fixedDeltaTime * 30f);
        } else if (lookDirection.sqrMagnitude > 0.01) {
            float rot_z = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, rot_z - 90), Time.fixedDeltaTime * 30f);
        }
    }

    public bool LifeGreaterThen(float n) {
        return (int)life.life > n;
    }

    void OnGetDamaged() {
        time_not_hit = 0;
    }
}
