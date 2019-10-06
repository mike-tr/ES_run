using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public enum JoystickState {
    none,
    moveDirection,
}

public class Joystick : MonoBehaviour
{
    public static float Horizontal { get { return direction.x; } }
    public static float Vertical { get { return direction.y; } }
    private static Vector2 direction;
    private static Vector2 fixDirection;
    public static float magnitude = 0;
    public static Vector2 getDirection() {
        return direction;
    }

    public static Vector2 getFixDirection() {
        return fixDirection;
    }

    public float repositionSpeed = 1.5f;
    public float repositionStart = 1f;

    public Material mat;

    public Image startImage;
    public Image endImage;
    Vector2 posStart;
    Vector2 currentPos;

    Vector2 magPosStart;

    public int DrawTestTimes = 10;

    public float divRatio = 1;

    public int lineWidth = 1;

    public bool OnUI;

    public void Start() {
        divRatio = Screen.width > Screen.height ? Screen.width : Screen.height;
        divRatio = divRatio / 25f;
        
        if (Application.isMobilePlatform) {
            divRatio *= 2.5f;
            repositionStart *= 0.25f;
            repositionSpeed *= 10;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Application.isMobilePlatform) {
            int c = Input.touchCount;
            if(c > 0) {
                OnGetDirection(Input.GetTouch(c - 1));
                LineDrawer_GL.addLine(new Color(1, 1, 1, 0.5f), lineWidth, posStart, currentPos, 4, 8);
            } else {
                onRelease();             
                magnitude = 0;
            }
            /*
             
            if (c >= 2) {
                UISystem.Log("- Set Ship speed -");
                OnGetDirection(Input.GetTouch(0));
                OnGetMagnitude(Input.GetTouch(1));
                //UISystem.Log("\nMagnitude = " + magnitude);
            } else if(c > 0) {
                OnGetDirection(Input.GetTouch(0));
                //magnitude = 0;
            } else {
                onRelease();
                magnitude = 0;
            }
            */
        } else {

            if (Input.GetMouseButtonDown(0)) {
                onDirectionStart(Input.mousePosition);
            } else if (Input.GetMouseButton(0)) {
                if (OnUI)
                    return;

                OnUpdateDirection(Input.mousePosition);       
                LineDrawer_GL.addLine(new Color(1,1,1, 0.1f), lineWidth, posStart, currentPos, 4, 8);
            } else {
                onRelease();
            }
        }
    }

    public void OnGetDirection(Touch touch) {
        if (OnUI)
            return;

        if (touchReset || touch.phase == TouchPhase.Began) {
            onDirectionStart(touch.position, touch.fingerId);
            return;
        } else if(touch.phase == TouchPhase.Ended) {
            onRelease();
        }
        OnUpdateDirection(touch.position);
        LineDrawer_GL.addLine(new Color(1, 1, 1, 0.1f), lineWidth, posStart, currentPos, 4, 8);
    }

    public void OnGetMagnitude(Touch touch) {
        if (OnUI)
            return;


        if (touch.phase == TouchPhase.Began) {
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) {
                OnUI = true;
                return;
            }
            magPosStart = touch.position;
            //onDirectionStart(touch.position, touch.fingerId);
            return;
        }else if (touch.phase == TouchPhase.Ended) {
            GameData.saveGameData();
            return;
        }

        GameData.speedMultiplier += ((touch.position.y - magPosStart.y) * 4) / Screen.height;
        GameData.speedMultiplier = Mathf.Clamp(GameData.speedMultiplier, .05f, 2.5f);
        magPosStart = touch.position;

        UISystem.Log("\nSpeed - " + GameData.speedMultiplier * 100 + "%");
        //OnUpdateDirection(touch.position);
        //LineDrawer_GL.addLine(new Color(1, 1, 1, 0.1f), lineWidth, posStart, currentPos, 4, 8);
    }

    ///////////////// Other
    public bool onDirectionStart(Vector2 input, int fingerID) {
        if (EventSystem.current.IsPointerOverGameObject(fingerID)) {
            OnUI = true;
            return false;
        }

        touchReset = false;
        posStart = input;
        currentPos = input;
        startImage.enabled = true;
        startImage.transform.position = posStart;
        endImage.enabled = true;
        endImage.transform.position = posStart;
        return true;
    }

    public bool onDirectionStart(Vector2 input) {
        if (EventSystem.current.IsPointerOverGameObject()) {
            OnUI = true;
            return false;
        }

        posStart = input;
        currentPos = input;
        startImage.enabled = true;
        startImage.transform.position = posStart;
        endImage.enabled = true;
        endImage.transform.position = posStart;
        return true;
    }

    public void OnUpdateDirection(Vector2 input) {
        currentPos = input;
        endImage.transform.position = currentPos;

        if (direction.sqrMagnitude > repositionStart * repositionStart) {
            fixDirection = direction.normalized * (direction.magnitude + 0.25f - repositionStart) * repositionSpeed;
            posStart += fixDirection;
            startImage.transform.position = posStart;
            magnitude = fixDirection.magnitude;
        } else {
            fixDirection = Vector2.zero;
            magnitude = 0;
        }

        direction = currentPos - posStart;
        direction /= divRatio;
        //fixDirection = direction;
    }

    bool touchReset = true;
    public void onRelease() {
        startImage.enabled = false;
        endImage.enabled = false;
        direction = Vector2.zero;
        fixDirection = Vector2.zero;
        touchReset = true;
        OnUI = false;
        //touchReset = true;
    }

}
