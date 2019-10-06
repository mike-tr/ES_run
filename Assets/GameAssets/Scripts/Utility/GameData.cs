using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class GameData
{
    public static bool upgradesLoaded = false;
    public static int highScore = 0;
    public static int score = 0;
    public static bool muteGame = false;
    public static float gold;
    public static bool firstTime = true;
    public static int timeDied = 0;
    public static float speedMultiplier = 1;

    public const string saveGameVersion = "0.9.10b";
    public const string upgradesFile = "upgrades.gsd";
    public const string dataFile = "mdata.gsd";
    private static Dictionary<int, int> playerCurrentUpgrades = new Dictionary<int, int>();

    //private static Dictionary<int, Upgrade> shopUpgrades = new Dictionary<int, Upgrade>();

    //public static void loadShopUpgrades() {
    //    Upgrade[] upgradesObjects = Resources.LoadAll<Upgrade>("Upgrades");
    //    for (int i = 0; i < upgradesObjects.Length; i++) {
    //        Upgrade current = upgradesObjects[i];
    //        shopUpgrades.Add(current.getId(), current);
    //    }
    //}

    public static int getUpgrade(int uniqueId) {
        if (playerCurrentUpgrades.TryGetValue(uniqueId, out int level))
            return level;
        if (!upgradesLoaded) {
            LoadUpgradeData();
            return getUpgrade(uniqueId);
        }          
        return 1;
    }

    public static void setUpgrade(int unqiueId, int level) {
        if (playerCurrentUpgrades.ContainsKey(unqiueId)) {
            playerCurrentUpgrades[unqiueId] = level;
            return;
        }
        playerCurrentUpgrades.Add(unqiueId, level);
        saveUpgradeData();
    }

    public static void resetAll() {
        highScore = 0;
        gold = 0;
        score = 0;
        playerCurrentUpgrades = new Dictionary<int, int>();
        saveGameData();
        saveUpgradeData();
    }

    public static void LoadGameData() {
        SaveSystem<SaveGameData>.loadSettings(dataFile);
    }

    public static void saveGameData() {
        SaveSystem<SaveGameData>.saveSettings(new SaveGameData(), dataFile);
    }


    public static void LoadUpgradeData() {
        upgradesLoaded = true;
        SaveSystem<SaveUpgradeData>.loadSettings(upgradesFile);
    }

    public static void saveUpgradeData() {
        SaveSystem<SaveUpgradeData>.saveSettings(new SaveUpgradeData(), upgradesFile);
    }

    [System.Serializable]
    public class SaveUpgradeData : ISavable {
        public int[] uids;
        public int[] upt;

        public SaveUpgradeData() {
            uids = playerCurrentUpgrades.Keys.ToArray();
            upt = playerCurrentUpgrades.Values.ToArray();
        }

        public void empty() {
            uids = new int[0];
            upt = new int[0];
            //nothing
        }

        public void load() {
            playerCurrentUpgrades = new Dictionary<int, int>();
            for (int i = 0; i < uids.Length; i++) {
                playerCurrentUpgrades.Add(uids[i], upt[i]);
            }
        }
    }


    [System.Serializable]
    public class SaveGameData :ISavable {
        public int highScore;
        public bool muteGame;
        public float speedMultiplier;
        public float gold;
        public string saveGameVersion;
        public int timeDied;

        public SaveGameData() {
            highScore = GameData.highScore;
            muteGame = GameData.muteGame;
            speedMultiplier = GameData.speedMultiplier;
            gold = GameData.gold;
            saveGameVersion = GameData.saveGameVersion;
            timeDied = GameData.timeDied;
        }

        public void empty() {
            resetAll();
        }

        public void load() {
            GameData.muteGame = muteGame;
            GameData.gold = gold;
            GameData.speedMultiplier = speedMultiplier;
            GameData.highScore = highScore;
            GameData.timeDied = timeDied;
            if (speedMultiplier <= 0)
                GameData.speedMultiplier = 1;

            if(GameData.saveGameVersion != saveGameVersion) {
                resetAll();
            }
        }
    }
}
