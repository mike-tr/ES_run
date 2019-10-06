using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFlow : MonoBehaviour {
    public static CameraFlow instance;
    public static float getSpeed { get => instance.cameraSpeed; }
    public static Vector2 position { get => instance.transform.position; }

    [SerializeField]
    private float cameraSpeed = 1;
    public static Vector2 ScreenBounds;

    Camera cam;

    public static Vector2 screenCenter;
    private void Start() {
        instance = this;
        cam = Camera.main;
        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;
        //worldScreenBotders = cam.ScreenToWorldPoint(worldScreenBotders);

        float ratio = 600f / Screen.width ;
        if (ratio < 1f)
            ratio = 1f;
        cam.orthographicSize = ratio * 10;

        if (Application.isMobilePlatform) {
            //float r = (Screen.width / 2000f) * 0.25f + 0.75f;
            cam.orthographicSize = 12;
        }

        float height = cam.orthographicSize * 2;
        float screenRatio = ((float)Screen.width / Screen.height);
        ScreenBounds =  new Vector2(height * screenRatio, height);
    }

    public Vector2 setInBounds(Vector2 current) {
        current.x = Mathf.Clamp(current.x, transform.position.x - ScreenBounds.x / 2 + .2f, transform.position.x + ScreenBounds.x / 2 - .2f);
        current.y = Mathf.Clamp(current.y, transform.position.y - ScreenBounds.y / 2 + .2f, transform.position.y + ScreenBounds.y / 2 - .2f);
        return current;
    }

    public static Vector2 outSideScreen(Vector2 dir) {
        dir = screenCenter * dir * 1.44f + screenCenter;

        return (Vector2)instance.cam.ScreenToWorldPoint(dir);
    }

    public static Vector2 randomScreenPos() {
        Vector2 pos = new Vector2(Screen.width * ((Random.value * 0.8f)  + 0.1f), Screen.height * ((Random.value * 0.8f) + 0.1f));
        return instance.cam.ScreenToWorldPoint(pos);
    }

    public static bool inScreenView(Vector2 worldPos, float r) {
        Vector2 screenPos = instance.cam.WorldToScreenPoint(worldPos);    
        if (screenPos.x > -Screen.width * r && screenPos.x < Screen.width * (r + 1)) {
            if (screenPos.y > -Screen.height * r && screenPos.y < Screen.height * (r + 1)) {
                return true;
            }
        }
        return false;
    }

    public static float getScreenMinBorderY() {
        return instance.transform.position.y - ScreenBounds.y * .75f;
    }

    //private void Update() {
    //    float height = cam.orthographicSize * 2;
    //    float screenRatio = ((float)Screen.width / Screen.height);
    //    box.size = new Vector2(height * screenRatio, height);
    //}

    private void FixedUpdate() {
        transform.position += Vector3.up * cameraSpeed * Time.deltaTime;
    }
}
