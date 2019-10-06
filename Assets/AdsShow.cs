using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdsShow : MonoBehaviour
{
    public Text log;
    public static void showAd(bool reward = false) {
#if UNITY_ADS
        if (!reward) {
            AdsManager.instance.showRegularAd(regularAd);
        } else {
            AdsManager.instance.showRewardAd(rewardAd);
        }
#endif
    }

#if UNITY_ADS
    public static void regularAd(ShowResult result) {
        Debug.Log("Regular Ad closed");
    }

    public static void rewardAd(ShowResult result) {

        switch (result) {
            case ShowResult.Failed:
                Debug.Log("failed to show ad");
                break;
            case ShowResult.Finished:
                Debug.Log("reward player");
                break;
            case ShowResult.Skipped:
                Debug.Log("no reward! skipped :(");
                break;
        }
    }
#endif
}
