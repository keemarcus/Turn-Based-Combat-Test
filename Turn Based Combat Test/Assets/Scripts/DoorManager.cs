using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public Animator animator;
    private void Awake()
    {
        animator = this.gameObject.GetComponent<Animator>();
        animator.SetBool("Closed", true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleState();
        }
    }

    public void ToggleState()
    {
        animator.SetBool("Closed", !animator.GetBool("Closed"));
    }
}
