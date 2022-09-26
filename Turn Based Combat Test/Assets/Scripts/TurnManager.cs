using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public CharacterManager [] initiative;
    int currentTurn = 0;

    CameraController cameraController;
    public TMPro.TextMeshProUGUI moveCounter;
    
    // Start is called before the first frame update
    void Start()
    {
        // get the camera controller
        cameraController = FindObjectOfType<CameraController>();
        
        // get all the characters in the scene
        initiative = FindObjectsOfType<CharacterManager>();
        
        // set the first character in initiative as active
        SetActiveCharacter(0);
    }


    public void ChangeTurn()
    {
        if (initiative[currentTurn].interacting) { return; }

        currentTurn += 1;
        if(currentTurn >= initiative.Length)
        {
            currentTurn = 0;
        }

        //SetActiveCharacter(currentTurn);
        if(initiative[currentTurn].currentState == initiative[currentTurn].deadState)
        {
            ChangeTurn();
        }
        else
        {
            SetActiveCharacter(currentTurn);
        }
    }

    private void SetActiveCharacter(int placeInInitiative)
    {
        cameraController.playerTransform = initiative[placeInInitiative].gameObject.transform;
        initiative[placeInInitiative].StartTurn();
        UpdateMoveCounter(initiative[placeInInitiative].charPathfinding.movesLeft);

        for (int i = 0; i < initiative.Length; i++)
        {
            if(i == placeInInitiative)
            {
                initiative[i].turn = true;
                //Debug.Log("Active Character = " + initiative[i].gameObject.name);
            }
            else
            {
                initiative[i].turn = false;
            }
        }
    }

    public void UpdateMoveCounter(int movesLeft)
    {
        moveCounter.text = "Moves Left: " + movesLeft;
    }
}
