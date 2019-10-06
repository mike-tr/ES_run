using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeList {
    damage,
    energy,
    repairSpeed,
    missle_up,
    missle_right,
    missle_left,
    shooting_rate,
    missle_speedUp,
    missle_speedDown,
    life,
    missleLife,
    FasterShip,
    SlowerShip,
}

/// <summary>
/// responsible to upgrade any upgrade!
/// </summary>
public class UpgradeLogic {
    public UpgradeLogic(Upgrade parent, PlayerController player) {
        this.parent = parent;
        lastCost = parent.cost;
        currentCost = parent.cost;
        cost_scale = parent.cost_scale;
        type = parent.type;
        shortCutKey = parent.UpgradeKey;
        this.player = player;
        level = 1;

        setLevel(GameData.getUpgrade(parent.getId()));
    }

    public PlayerController player;
    public Upgrade parent;
    public UpgradeList type;
    public UpgradeUI UI;

    public float lastCost { get; private set; } = 100;
    public float currentCost { get; private set; } = 100;

    public float cost_scale = 1.5f;

    public KeyCode shortCutKey;

    public int level = 1;

    public bool tryUpgrade() {
        if(GameData.gold > currentCost) {
            return forceNextUpgrade(false);
        }
        return false;
    }
    private bool forceNextUpgrade(bool force) {
        if (parent.maxLevel > 0 && parent.maxLevel < level + 1)
            return false;

        player.UpgradeByType(type);

        lastCost = currentCost;
        if (!force) {
            GameData.gold -= lastCost;
        }
            
        
        currentCost *= cost_scale;
        level++;

        GameData.setUpgrade(parent.getId(), level);
        if (UI)
            UI.UpdateUI();
        return true;
    }

    private void setLevel(int level) {
        for (; this.level < level;) {
            forceNextUpgrade(true);
        }
    }
}

/// <summary>
/// this is the PLayer controller!!!!!!!!!!!!!!!!!!!!!!!!!!
/// </summary>
public class PlayerController : MonoBehaviour
{
    public Dictionary<int, UpgradeLogic> upgrades = new Dictionary<int, UpgradeLogic>();

    public static bool joystick = true;
    public static PlayerController instance;
    public static bool inShop = false;

    public static Transform player;
    Camera cam;
    
    public float speed = 15;
    public FireSystem fire;
    UnitLife life;

    public float CurrentEnergy { get => life.life; }
    public float maxEnergy = 5;
    public float repairSpeed = 1f;


    public float gold_income = 0;
    private bool loaded = false;

    public float LifePercentage {
        get { return life.life / maxEnergy; }
    }

    private float maxSpeed = 0;
    Upgrade[] upgradesObjects;

    private float joystickSpeed = 0;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        maxSpeed = speed;
        GameData.LoadUpgradeData();
        
        instance = this;
        life = GetComponent<UnitLife>();
        life.life = maxEnergy;
        life.onGetHitCallback += OnGetDamaged;

        player = transform;
        fire = GetComponent<FireSystem>();
        cam = Camera.main;
        fire.camRatio = 1f;

        while (!UISystem.instance) {
            yield return null;
        }

        if (Application.isMobilePlatform) {
            //speed *= 0.25f;
            joystickSpeed = speed * .25f;
            ds = 3f;
            //maybe do something?!
        }

        Upgrade[] upgradesObjects = Resources.LoadAll<Upgrade>("Upgrades");
        upgrades = new Dictionary<int, UpgradeLogic>();
        foreach (Upgrade upg in upgradesObjects) {
            if (!upg.disabled) {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
                upgrades.Add(upg.getId(), new UpgradeLogic(upg, this));
#else
                if(!upg.MobileOnly)
                    upgrades.Add(upg.getId(), new UpgradeLogic(upg, this));
#endif
            }
        }
        UISystem.instance.SetupShop(upgrades);
        loaded = true;

        life.life = maxEnergy;
        life.maxDamage = maxEnergy - 1;
    }

    public void AddGold(float amount) {
        GameData.gold += amount;
    }
    // Update is called once per frame
    public void Update() {
        if (CurrentEnergy < 1)
            return;
        

        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) {
            //Fire(transform.right, transform.right);
            if(Time.timeScale != 0)
                fire.Fire();
            //FireAllDir(UpShoot, transform.right * 0.2f, transform.up);
        }
        if(inShop)
        foreach(UpgradeLogic upgrade in upgrades.Values) {
            if (Input.GetKeyDown(upgrade.shortCutKey)) {
                upgrade.tryUpgrade();
            }
        }
        Regenerate();
    }

    public void Regenerate() {
        if (life.life < maxEnergy) {
            life.life += Time.deltaTime * repairSpeed;
           // GameData.gold -= 1f * Time.deltaTime;
        } else {
            //gold_income = RandomEnemies.STR * 0.25f;
            GameData.gold += 0.6f * Time.deltaTime; 
        }
    }

    public float ds = 0.32f;

    private void FixedUpdate() {
        if (CurrentEnergy < 1)
            return;
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        //Vector3 np = Vector3.Lerp(transform.position, mousePos, Time.fixedDeltaTime * this.speed);
        //transform.position = np;

        Vector2 lookDirection = Vector2.zero;
        float speed = 0;
        if (Application.isMobilePlatform) {
            if (joystick) {
                speed = Joystick.magnitude;
                lookDirection = Joystick.getDirection();

                transform.position += transform.up * Time.fixedDeltaTime * speed * joystickSpeed;
                transform.position = CameraFlow.instance.setInBounds(transform.position);
                if (lookDirection.sqrMagnitude > 0) {
                    float rot_z = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, rot_z - 90), Time.fixedDeltaTime * 30f);
                }
            } else if (Input.touchCount > 0) {
                Touch touch = Input.GetTouch(Input.touchCount - 1);
                mousePos = cam.ScreenToWorldPoint(touch.position);
                Vector3 diff = mousePos - (Vector2)transform.position;
                speed = diff.magnitude;
                lookDirection = diff;
                if (speed > ds) {
                    if (speed > 2)
                        speed = 2;
                    transform.position += transform.up * Time.fixedDeltaTime * speed * this.speed;
                    transform.position = CameraFlow.instance.setInBounds(transform.position);

                    float rot_z = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, rot_z - 90), Time.fixedDeltaTime * 30f);
                } else if (lookDirection.sqrMagnitude > 0.01) {
                    float rot_z = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, rot_z - 90), Time.fixedDeltaTime * 30f);
                }
            }         
        } else {
            Vector3 diff = mousePos - (Vector2)transform.position;
            speed = Mathf.Clamp(diff.sqrMagnitude, 0, 4);
            lookDirection = diff;
            if (speed > ds) {
                transform.position += transform.up * Time.fixedDeltaTime * speed * this.speed;
                transform.position = CameraFlow.instance.setInBounds(transform.position);

                float rot_z = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, rot_z - 90), Time.fixedDeltaTime * 30f);
            } else if (lookDirection.sqrMagnitude > 0.01) {
                float rot_z = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, rot_z - 90), Time.fixedDeltaTime * 30f);
            }
        }
        //diff = Joystick.instance.Direction * 1f;
        //diff = Joystick.getDirection();      

        //transform.position += Vector3.up * CameraFlow.getSpeed * Time.fixedDeltaTime;
    }

    public bool LifeGreaterThen(float n) {
        return (int)life.life > n;
    }

    public void UpgradeByType(UpgradeList type) {
        switch (type) {
            case UpgradeList.damage:
                fire.damage *= 1.07f;
                fire.damage += 0.77f;
                break;
            case UpgradeList.energy:
                maxEnergy += 1;
                break;
            case UpgradeList.repairSpeed:
                repairSpeed *= 1.2f;
                repairSpeed += 0.1f;
                break;
            case UpgradeList.missle_up:
                fire.damage *= 0.95f;
                if (fire.damage < 2)
                    fire.damage = 2;
                fire.UpShoot++;
                break;
            case UpgradeList.missle_right:
                fire.damage *= 0.95f;
                if (fire.damage < 2)
                    fire.damage = 2;
                fire.RightShoot++;
                break;
            case UpgradeList.missle_left:
                fire.damage *= 0.95f;
                if (fire.damage < 2)
                    fire.damage = 2;
                fire.LeftShoot++;
                break;
            case UpgradeList.shooting_rate:
                fire.reloadTime *= 0.94f;
                break;
            case UpgradeList.missle_speedUp:
                fire.missleSpeed *= 1.05f;
                break;
            case UpgradeList.life:
                maxEnergy *= 1.07f;
                maxEnergy += 1f;
                life.maxDamage = maxEnergy - 1;
                break;
            case UpgradeList.missleLife:
                fire.missleLife *= 1.2f;
                break;
            case UpgradeList.FasterShip:
                maxSpeed *= 1.1f;
                setSpeed();
                break;
            case UpgradeList.SlowerShip:
                maxSpeed *= 0.9f;
                setSpeed();
                break;
            default:
                break;
        }

        if (loaded)
            AudioManager.playOther(playEffect.powerUp, 1);
    }

    private void setSpeed() {
        speed = Mathf.Clamp(maxSpeed, 1, 10);
    }

    void OnGetDamaged() {

    }

}
