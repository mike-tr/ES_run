using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class OpenLink : MonoBehaviour
{
    [SerializeField] private string link = "test.com";

    public void openLink() {
        Application.OpenURL(link);
    }

//    public void OpenLinkJSPlugin() {
//#if !UNITY_EDITOR
//		    openWindow(link);
//#endif
//    }

//    [DllImport("__Internal")]
//    private static extern void openWindow(string url);
}
