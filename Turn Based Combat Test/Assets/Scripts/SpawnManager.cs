using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private List<Vector2> playerSpawnPoints;

    private void Awake()
    {
        // initialize all the lists of spawn points
        playerSpawnPoints = new List<Vector2>();
    }

    public void AddPlayerSpawnPoint(Vector2 position)
    {
        playerSpawnPoints.Add(position);
    }

    public void SpawnPlayers()
    {
        PlayerCharacterManager[] players = FindObjectsOfType<PlayerCharacterManager>();
        for(int ctr = 0; ctr < players.Length; ctr++)
        {
            if(ctr < playerSpawnPoints.Count && playerSpawnPoints[ctr] != null)
            {
                players[ctr].gameObject.transform.SetPositionAndRotation(playerSpawnPoints[ctr], Quaternion.identity);
            }
            else
            {
                Debug.Log("No spawn point found for player: " + players[ctr].gameObject.name);
                players[ctr].gameObject.SetActive(false);
            }
            
        }
    }
}
