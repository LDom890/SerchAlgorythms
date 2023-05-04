using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    public Camera main;
    public Tilemap tilemap;
    public Vector3 offSet = new Vector3 (0, 0.3f, 0);

    public TileBase originTile;
    public TileBase destinationTile;

    private Dictionary<Tilemap, Vector3Int> _previousPosition = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Tilemap, Vector3Int> _origin = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Tilemap, Vector3Int> _goal = new Dictionary<Tilemap, Vector3Int>();

    public DijkstraAlgorithm floodFill;
    private void Start()
    {
        _previousPosition[tilemap] = new Vector3Int(-1, -1, 0);
        //_origin[tilemap] = new Vector3Int(0, 0, 0);
    }

    private void Update()
    {
        SelectTile();
        if(Input.GetMouseButton(0))
        {
            DetectTileClick(true);
        }
        else if(Input.GetMouseButton(1))
        {
            DetectTileClick(false);
        }
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            StartFloodFill();
        }
    }
    private void DetectTileClick(bool isOrigin)
    {
        Vector3 mousePosition = main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePosition = tilemap.WorldToCell(mousePosition);
        tilePosition.z = 0;

        TileBase newTile = isOrigin ? originTile : destinationTile;
        Dictionary<Tilemap, Vector3Int> selectedDictionary = isOrigin ? _origin : _goal;

        if(tilemap.HasTile(tilePosition))
        {
            var oldTile = tilemap.GetTile(tilePosition);
            tilemap.SetTile(tilePosition, newTile);
            if (selectedDictionary.ContainsKey(tilemap))
            {
                tilemap.SetTile(selectedDictionary[tilemap], oldTile);
            }
            //tilemap.SetTile(_origin[tilemap], oldTile);
            selectedDictionary[tilemap] = tilePosition;
        }
    }
    private void SelectTile()
    {
        Vector3 mousePosition = main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePosition = tilemap.WorldToCell(mousePosition);
        tilePosition.z = 0;

        if (tilemap.HasTile(tilePosition))
        {
            tilemap.SetTransformMatrix(tilePosition, Matrix4x4.TRS(offSet, Quaternion.Euler(0,0,0), Vector3.one));
            tilemap.SetTransformMatrix(_previousPosition[tilemap], Matrix4x4.identity);
            _previousPosition[tilemap] = tilePosition;
        }
    }

    private void StartFloodFill()
    {
        floodFill.Origin = _origin[tilemap];
        floodFill.Goal = _goal[tilemap];
        floodFill.tileMap = tilemap;
        floodFill.visitedTile = originTile;
        floodFill.pathTile = destinationTile;
        StartCoroutine(floodFill.FloodFill2D());
    }
}
