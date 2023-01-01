using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorIndicatorManager : ActionIndicatorManager
{
    public TMPro.TextMeshProUGUI costText;
    private void Awake()
    {
        TMPro.TextMeshProUGUI[] texts = this.gameObject.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        costText = texts[2];
    }

    public void UpdateCostText(string cost)
    {
        costText.text = "Cost: " + cost;
    }
}
