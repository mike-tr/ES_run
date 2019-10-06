using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UpgradeUI : MonoBehaviour
{
    public TextMeshProUGUI label_text;
    public TextMeshProUGUI cost_text;
    public TextMeshProUGUI lv_text;

    public string label = "Upgrade Dis";
    public string cost = "100g";

    public UpgradeLogic parent;

    public Button button;

    public void Setup(UpgradeLogic parent) {
        this.parent = parent;
        parent.UI = this;
        button.onClick.AddListener(() => parent.tryUpgrade());
        //button.onClick.AddListener(() => UpdateUI());
        UpdateUI();
    }

    public void UpdateUI() {
        label = parent.parent.label;
        cost = ((int)parent.currentCost).ToString();
        label_text.text = label;
        cost_text.text = cost + "g";
        lv_text.text = "Lv." + parent.level;

    }
}
