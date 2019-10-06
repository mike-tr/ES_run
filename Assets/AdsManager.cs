using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;

    [SerializeField] private string gameId = "";
    [SerializeField] private bool testMode = true;
    [SerializeField] private string rewardedVideoPlacementId;
    [SerializeField] private string regularPlacementId;

    private void Awake() {
        if (instance != null && instance != this)
            Destroy(gameObject);
        #if UNITY_ADS
        DontDestroyOnLoad(this.gameObject);
        instance = this;
        Advertisement.Initialize(gameId, testMode);
#endif
    }

    #if UNITY_ADS
    public void showRegularAd(Action<ShowResult> callback) {
        if (Advertisement.IsReady(regularPlacementId)) {
            ShowOptions so = new ShowOptions();
            so.resultCallback = callback;
            Advertisement.Show(regularPlacementId, so);
        } else {
            Debug.Log("ad is not ready!");
        }
    }
#endif


#if UNITY_ADS
    public void showRewardAd(Action<ShowResult> callback) {
        if (Advertisement.IsReady(rewardedVideoPlacementId)) {
            ShowOptions so = new ShowOptions();
            so.resultCallback = callback;
            Advertisement.Show(rewardedVideoPlacementId, so);
        } else {
            Debug.Log("ad is not ready!");
        }
    }
#endif

}
