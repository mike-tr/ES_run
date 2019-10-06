using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnemies : MonoBehaviour
{
    public static float maxDifficulty = 1;
    public static float difficultyScaler = 1f;
    public static float BossChance = 0;


    public static float liveDifficulty = 0;


    public float reviveChance = 0.05f;
    public AiObject enemyPrefab;

    Camera cam;
    public void Start() {
        cam = Camera.main;
        StartCoroutine(LevelUp());

        liveDifficulty = 0;
        maxDifficulty = 10;
    }

    IEnumerator LevelUp() {
        maxDifficulty = 10 * difficultyScaler;
        while (true) {
            //UISystem.Log(liveDifficulty + " .. mx - " + maxDifficulty, true);
            //UISystem.Log((int)(BossChance * 100) + "% - boss spawn chance!");

            maxDifficulty += difficultyScaler * 0.3f;
            maxDifficulty *= 1.0007f;

            yield return new WaitForSeconds(2f);
            if(AiObject.Enemies < 10) {
                if(AiObject.Enemies <= 0) {
                    AiObject.Enemies = 0;
                    liveDifficulty = 0;
                }
                float gc = (maxDifficulty - liveDifficulty) * reviveChance;
                if (gc > Random.value) {
                    SpawnRandom();
                }
            }
        }
    }

    public void SpawnRandom() {
        //Vector3 screenPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, Camera.main.nearClipPlane+5)); //will get the middle of the screen

        Vector3 screenPosition = cam.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width), Random.Range(Screen.height * 1.1f, Screen.height * 1.2f), 0));
        screenPosition.z = 0;

        AiObject enemy = Instantiate(enemyPrefab, screenPosition, Quaternion.identity, transform);
    }
}
