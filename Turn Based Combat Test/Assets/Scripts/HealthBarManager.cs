using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    private GameObject fillSprite;

    private void Awake()
    {
        fillSprite = this.gameObject.transform.GetChild(1).gameObject;
        Debug.Log(fillSprite.name);
    }

    public void SetValue(float value)
    {
        Debug.Log(value);

        // set the scale of the healthbar fill
        fillSprite.transform.localScale = new Vector3(value, fillSprite.transform.localScale.y, fillSprite.transform.localScale.z);

        // set the position of the healthbar fill
        fillSprite.transform.localPosition = new Vector3(CalculateOffset(value), fillSprite.transform.localPosition.y, fillSprite.transform.localPosition.z);
    }
    private float CalculateOffset(float value)
    {
        Debug.Log((1 - value) / -2);
        return (1 - value) / -2;
    }
}
