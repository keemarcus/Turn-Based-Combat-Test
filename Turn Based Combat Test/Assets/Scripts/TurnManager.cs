using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public CharacterPathfinding [] initiative;
    int currentTurn = 0;

    CameraController cameraController;
    
    // Start is called before the first frame update
    void Start()
    {
        // get the camera controller
        cameraController = FindObjectOfType<CameraController>();
        
        // get all the characters in the scene
        initiative = FindObjectsOfType<CharacterPathfinding>();
        
        // set the first character in initiative as active
        SetActiveCharacter(0);
    }


    public void ChangeTurn()
    {
        currentTurn += 1;
        if(currentTurn >= initiative.Length)
        {
            currentTurn = 0;
        }

        SetActiveCharacter(currentTurn);
    }

    private void SetActiveCharacter(int placeInInitiative)
    {
        cameraController.playerTransform = initiative[placeInInitiative].gameObject.transform;

        for (int i = 0; i < initiative.Length; i++)
        {
            if(i == placeInInitiative)
            {
                initiative[i].turn = true;
                
            }
            else
            {
                initiative[i].turn = false;
            }
        }
    }
}
