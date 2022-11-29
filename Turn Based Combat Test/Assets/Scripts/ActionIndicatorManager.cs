using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionIndicatorManager : MonoBehaviour
{
    void Update()
    {
        this.gameObject.transform.position = Input.mousePosition;
    }
}
