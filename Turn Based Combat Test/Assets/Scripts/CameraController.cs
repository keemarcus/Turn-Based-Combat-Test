using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;

    void Update()
    {
        this.transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, -10f);
    }
}
