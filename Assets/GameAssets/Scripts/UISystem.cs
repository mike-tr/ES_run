using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UISystem : MonoBehaviour
{
    public static UISystem instance;
    public TextMeshProUGUI gold;

    public Text androidDebug;


    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI cGoldText;
    public TextMeshProUGUI goldGotText;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI HelpText;
    public TextMeshProUGUI EnemyLevelText;
    public TextMeshProUGUI ScoreText;

    private static bool reset = false;
    public static void Log(string message) {
        instance.androidDebug.text += message + "\n";
        reset = false;
    }

    public Transform LifePercentage;

    public GameObject DeathScreen;
    public GameObject LiveScreen;
    public GameObject PauseUI;
    public GameObject ShopUI;
    public GameObject HelpPanelUI;
    public GameObject CreditsUI;

    public UpgradeUI ShopItemObject;
    public Button ClosePrefab;
    public bool IsShopOpened = false;

    public PlayerController player;
    Transform camera;

    [SerializeField]private bool IsPaused = false;
    bool deadActive = false;
    // Start is called before the first frame update
    void Start()
    {
        GameData.LoadGameData();
        Mute(GameData.muteGame);

        RandomEnemies.difficultyScaler = 1 + (GameData.highScore * .001f);
        if (LevelText != null)
            LevelText.text = "Level - " + RandomEnemies.difficultyScaler.ToString("F1");

        camera = Camera.main.transform;
        player = PlayerController.instance;
        instance = this;
        //IsPaused = true;
        if (IsPaused) {
            PauseUI.SetActive(true);
            Time.timeScale = 0;
        } else {
            PauseUI.SetActive(false);
            Time.timeScale = 1;
        }

        IsShopOpened = false;
        ShopUI.SetActive(IsShopOpened);
           
        DeathScreen.SetActive(false);
        LiveScreen.SetActive(true);
        if (CreditsUI)
            CreditsUI.SetActive(false);
        
        HelpPanelUI.SetActive(!GameData.firstTime);

        if (!Application.isMobilePlatform) {
            HelpText.text = "- input -" + "\nShip follows the mouse, press Space/Left mouse click to shoot, use shop to get upgrades." +
                  "kill enemies to get more gold, Enjoy!";
        } else {
            Application.targetFrameRate = 60;
        }

        StartCoroutine(updateUIText());

        if (GameData.speedMultiplier > 2.5f || GameData.speedMultiplier <= 0f)
            GameData.speedMultiplier = 1f;
        //AudioListener.pause = !AudioListener.pause;
    }

    public void SetupShop(Dictionary<int, UpgradeLogic> upgrades) {
        foreach(Transform g in ShopUI.transform) {
            //Debug.Log(g.name);
            Destroy(g.gameObject);
        }
        foreach (UpgradeLogic upgrade in upgrades.Values) {
            UpgradeUI current = Instantiate(ShopItemObject, ShopUI.transform);
            current.Setup(upgrade);
        }
        Button close = Instantiate(ClosePrefab, ShopUI.transform);
        
        close.onClick.AddListener(() => OpenShop());
    }

    public void saveAudio() {
        AudioManager.saveAudioSettings();
    }

    public void Update() {
        if(!reset) {
            reset = true;
            instance.androidDebug.text = "";
        }
        //Log("Avg. enemy lvl - " + RandomEnemies.maxDifficulty);
        if (player.CurrentEnergy < 1) {
            if (!deadActive) {
                int nscore = GameData.score;        
                if (GameData.highScore < nscore) {
                    GameData.highScore = nscore;
                    highScoreText.text = "new highscore !!";
                } else
                    highScoreText.text = "highscore - " + GameData.highScore;

                goldGotText.text = "you got " + (int)GameData.gold + " gold!";
                GameData.timeDied++;
                GameData.saveGameData();


#if UNITY_ADS
                if(GameData.timeDied % 6 == 0) {
                    AdsShow.showAd();
                }
#endif

                if(GameData.timeDied % 6 == 0) {

                }

                currentScoreText.text = "score - " + nscore;
                DeathScreen.SetActive(true);
                LiveScreen.SetActive(false);
                GameData.score = 0;
                deadActive = true;
            }
            Time.timeScale = 0;         
        }
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (IsShopOpened)
                OpenShop();
            else
                EnableSettings();
        }
        else if (Input.GetKeyDown(KeyCode.F1)) {
            RestartLevel();
        }else if (Input.GetKeyDown(KeyCode.B)) {
            OpenShop();
        }
        else if (Input.GetKeyDown(KeyCode.M)) {
            muteGame();
        }

        LifePercentage.localScale = new Vector3(player.LifePercentage, 1, 1);
        //Overload.text = (player.fire.OverLoad).ToString("F2");
    }

    public void EnableSettings() {
        IsPaused = !IsPaused;
        if (IsPaused) {
            PauseUI.SetActive(true);
            Time.timeScale = 0;
        } else {
            PauseUI.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void OpenShop() {
        IsShopOpened = !IsShopOpened;
        if (IsShopOpened) {
            ShopUI.SetActive(true);
            if (IsPaused) {
                EnableSettings();
            }
            Time.timeScale = 0;
        } else {
            GameData.saveGameData();
            GameData.saveUpgradeData();
            ShopUI.SetActive(false);
            EnableSettings();
        }
    }

    public void HelpPanel() {
        HelpPanelUI.SetActive(!HelpPanelUI.activeSelf);
        GameData.firstTime = true;
    }

    public void CreditPanel() {
        CreditsUI.SetActive(!CreditsUI.activeSelf);
    }

    public void RestartLevel(bool forced = false) {
        if (!IsPaused && !forced) {
            EnableSettings();
            return;
        }
        Time.timeScale = 1;
        AiObject.Enemies = 0;
        IsPaused = false;
        PauseUI.SetActive(false);
        DeathScreen.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Mute(bool value) {    
        GameData.muteGame = value;
        AudioListener.pause = value;

        GameData.saveGameData();
    }

    public void muteGame() {
        Mute(!GameData.muteGame);     
    }

    public void CloseGame() {
        Application.Quit();
    }

    public void resetGame() {
        GameData.resetAll();
        RestartLevel(true);
    }


    IEnumerator updateUIText() {
        while (true) {
            yield return new WaitForSecondsRealtime(.25f);
            if(ScoreText != null)
                ScoreText.text = "score\n" +  GameData.score;

            if (EnemyLevelText != null)
                EnemyLevelText.text = "Lv." + (int)RandomEnemies.maxDifficulty * 0.1f;

            if(gold != null)
                gold.text = ((int)GameData.gold).ToString() + "g";

            if (cGoldText != null)
                cGoldText.text = GameData.gold.ToString("F0") + "G";
        }
    }

    public void LoadScene(int sceneID) {
        AiObject.Enemies = 0;
        GameData.score = 0;
        RandomEnemies.difficultyScaler = 1 + (GameData.highScore * .0001f);

        GameData.saveGameData();
        GameData.saveUpgradeData();
        SceneManager.LoadScene(sceneID);
    }
}
