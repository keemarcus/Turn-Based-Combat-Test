using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public class TileMapGenerator : MonoBehaviour
{

    public string refFileName;
    private Tilemap groundLayer;
    private Tilemap wallLayer;

    public Tile groundTile;
    public RuleTile wallTile;

    public TextAsset refFile;
    private int numberOfRows;
    private int numberOfColumns;

    void Awake()
    {
        // get all the tilemap objects
        groundLayer = this.gameObject.transform.Find("Ground").GetComponent<Tilemap>();
        if(groundLayer == null) { Debug.Log("No tilemap found for ground layer"); }
        wallLayer = this.gameObject.transform.Find("Walls").GetComponent<Tilemap>();
        if (wallLayer == null) { Debug.Log("No tilemap found for wall layer"); }

        // get the ref file
        string refData = refFile.ToString();
        string[] refDataLines = refData.Split('\n');
        for(int ctr = 0; ctr < refDataLines.Length; ctr++)
        {
            refDataLines[ctr] = refDataLines[ctr].Trim();
        }

        // determine the number of rows in the tilemap
        numberOfRows = refDataLines.Length;
        Debug.Log("Tilemap Height = " + numberOfRows);

        // determine the number of columns in the tilemap
        numberOfColumns = 0;
        foreach(string line in refDataLines)
        {
            if((line.Length - 1) > numberOfColumns) { numberOfColumns = line.Length - 1; }
        }
        Debug.Log("Tilemap Width = " + numberOfColumns);

        GenerateTileMap(refDataLines);
    }

    private void GenerateTileMap(string[] refDataLines)
    {
        for(int ctr = numberOfRows - 1; ctr > -1; ctr--)
        {
            GenerateRow(ctr, refDataLines[ctr]);
        }
    }

    private void GenerateRow(int rownumber, string row)
    {
        for(int ctr = 0; ctr < row.Length; ctr++)
        {
            switch (row[ctr])
            {
                case '-':
                    // leave this space empty
                    break;
                case 'w':
                    // place a wall tile in this space
                    wallLayer.SetTile(new Vector3Int(ctr, rownumber), wallTile);
                    // also place a ground tile under this space
                    groundLayer.SetTile(new Vector3Int(ctr, rownumber), groundTile);
                    break;
                case 'g':
                    // place a ground tile in this space
                    groundLayer.SetTile(new Vector3Int(ctr, rownumber), groundTile);
                    break;
                default:
                    Debug.Log("Unknown Tile ID found at (" + ctr + "," + rownumber + ") = " + row[ctr]);
                    break;
            }
            
        }
    }
}
