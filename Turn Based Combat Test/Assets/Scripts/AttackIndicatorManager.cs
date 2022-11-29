using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackIndicatorManager : ActionIndicatorManager
{
    public TMPro.TextMeshProUGUI targetText;
    public TMPro.TextMeshProUGUI costText;
    private void Awake()
    {
        TMPro.TextMeshProUGUI[] texts = this.gameObject.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        targetText = texts[1];
        costText = texts[2];

        //UpdateTargetText("target");
        //UpdateCostText("1");
    }

    public void UpdateTargetText(string targetName)
    {
        targetText.text = "Target: " + targetName;
    }
    public void UpdateCostText(string cost)
    {
        costText.text = "Cost: " + cost;
    }
}
