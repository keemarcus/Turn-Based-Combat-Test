using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public Animator animator;
    public GridManager gridManager;
    private void Awake()
    {
        animator = this.gameObject.GetComponent<Animator>();
        animator.SetBool("Closed", true);
        gridManager = FindObjectOfType<GridManager>();
        Debug.Log("Door at (" + Mathf.RoundToInt(this.transform.position.x) + ", " + Mathf.RoundToInt(this.transform.position.y) + ")");
        //gridManager.SetTileWalkable(new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y)), 0);
        //gridManager.SetTileWalkable(new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y) + 1), 0);
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
        bool newState = !animator.GetBool("Closed");
        animator.SetBool("Closed", newState);

        if (newState)
        {
            gridManager.SetTileWalkable(new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y)), 0);
            gridManager.SetTileWalkable(new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y) + 1), 0);
        } else
        {
            gridManager.SetTileWalkable(new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y)), 1);
            gridManager.SetTileWalkable(new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y) + 1), 1);
        }

    }
}
