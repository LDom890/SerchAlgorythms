using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Selector : MonoBehaviour
{
    public Camera main;
    public Tilemap tilemap;
    public Vector3 offset = new Vector3(0, 0, 0);
    public TileBase originTile;
    public TileBase destinationTile;
    public Search floodFill;


    private Dictionary<Tilemap, Vector3Int> _previousPosition = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Tilemap, Vector3Int> _origin = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Tilemap, Vector3Int> _goal = new Dictionary<Tilemap, Vector3Int>();

    

    private void Start()
    {
        _previousPosition[tilemap] = new Vector3Int(-1, -1, 0);
    }
    private void Update()
    {
        SelectTile();
        if (Input.GetMouseButtonDown(0)) DetectTileClock(isOrigin:true);
        if (Input.GetMouseButtonDown(1)) DetectTileClock(isOrigin:false);
        if (Input.GetKeyDown(KeyCode.Space)) StartFloodFill2D();
    }


    private void SelectTile()
    {
        Vector3 mousePosition = main.ScreenToViewportPoint(Input.mousePosition);
        Vector3Int tilePosition = tilemap.WorldToCell(mousePosition);
        tilePosition.z = 0;

        if(tilemap.HasTile(tilePosition))
        {
            tilemap.SetTransformMatrix(tilePosition, Matrix4x4.TRS(offset, Quaternion.Euler(0, 0, 0), Vector3.one));

            tilemap.SetTransformMatrix(_previousPosition[tilemap], Matrix4x4.identity);

            _previousPosition[tilemap] = tilePosition;
        }
    }


    private void DetectTileClock(bool isOrigin)
    {
        Vector3 mousePosition = main.ScreenToViewportPoint(Input.mousePosition);
        Vector3Int tilePosition = tilemap.WorldToCell(mousePosition);
        tilePosition.z = 0;

        TileBase newTile = isOrigin ? originTile : destinationTile;
        Dictionary<Tilemap, Vector3Int> selectedDictionary = isOrigin ? _origin : _goal;

        if(tilemap.HasTile(tilePosition))
        {
            var oldTile = tilemap.GetTile(tilePosition);
            tilemap.SetTile(tilePosition, newTile);
            if(selectedDictionary.ContainsKey(tilemap))
            {
                tilemap.SetTile(selectedDictionary[tilemap], oldTile);
            }
            selectedDictionary[tilemap] = tilePosition;
        }
    }


    private void StartFloodFill2D()
    {
        floodFill.Origin = _origin[tilemap];
        floodFill.Goal = _goal[tilemap];
        floodFill.tileMap = tilemap;
        floodFill.visitedTile = originTile;
        floodFill.pathTile = destinationTile;

        StartCoroutine(floodFill.FloodFill2D());
    }
}
