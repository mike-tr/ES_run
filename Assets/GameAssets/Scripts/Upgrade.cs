using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Upgrades/New Upgrade")]
public class Upgrade : ScriptableObject
{
    public string label = "UI label";
    public int cost = 100;
    public int maxLevel = -1;
    public float cost_scale = 1.5f;
    public UpgradeList type;
    public KeyCode UpgradeKey;

    public Upgrade deLevelOther;

    public bool disabled = false;
    public bool MobileOnly = false;

    [SerializeField]private int id = -1;
   
    public int getId() {
        return id;
    }

    private void OnValidate() {
        if(id == -1) {
            id = GetInstanceID();
        }
    }
}
