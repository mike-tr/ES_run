using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    //public float followRatio = 0.9f;
    public float bounds = 0;
    public float scrollSpeed = 1;
    // Start is called before the first frame update

    void Start()
    {
        bounds = GetComponentInChildren<SpriteRenderer>().bounds.size.y;
        StartCoroutine(shift());
    }

    // Update is called once per frame
    private void FixedUpdate() {
        transform.position -= scrollSpeed * Vector3.up * Time.deltaTime;
    }

    IEnumerator shift() {
        while (true) {
            yield return new WaitForSeconds(.25f);
            float m = transform.position.y + bounds * .5f - CameraFlow.getScreenMinBorderY();
            if(m < 0) {
                transform.position += Vector3.up * bounds * 2f;
            }         
        }
    }
}
