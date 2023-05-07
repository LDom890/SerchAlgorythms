using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelector : MonoBehaviour
{
    public Camera main;
    public Tilemap tilemap;
    public Vector3 offSet = new Vector3(0, 0.02f, 0);

    public TileBase originTile;
    public TileBase destinationTile;

    private Dictionary<Tilemap, Vector3Int> _previousPosition = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Tilemap, Vector3Int> _origin = new Dictionary<Tilemap, Vector3Int>();
    private Dictionary<Tilemap, Vector3Int> _goal = new Dictionary<Tilemap, Vector3Int>();

    public Heuristic heuristic;
    public FloofFill floodFill;
    public Dijkstra dijkstra;
    public AStar aStar;

    [SerializeField] private bool _floodfill;
    [SerializeField] private bool _heuristic;
    [SerializeField] private bool _dijkstra;
    [SerializeField] private bool _aStar;

    private void Start()
    {
        _previousPosition[tilemap] = new Vector3Int(-1, -1, 0);
    }


    private void Update()
    {
        SelectTile();
        if (Input.GetMouseButton(0))
        {
            DetectTileClick(true);
        }
        else if (Input.GetMouseButton(1))
        {
            DetectTileClick(false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(_floodfill)StartFloodFill();
            if(_dijkstra)StartDijkstra();
            if(_heuristic)StartHeuristic();
            if(_aStar)StartAStar();
        }
    }


    private void DetectTileClick(bool isOrigin)
    {
        Vector3 mousePosition = main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePosition = tilemap.WorldToCell(mousePosition);
        tilePosition.z = 0;

        TileBase newTile = isOrigin ? originTile : destinationTile;
        Dictionary<Tilemap, Vector3Int> selectedDictionary = isOrigin ? _origin : _goal;

        if (tilemap.HasTile(tilePosition))
        {
            var oldTile = tilemap.GetTile(tilePosition);
            tilemap.SetTile(tilePosition, newTile);
            if (selectedDictionary.ContainsKey(tilemap))
            {
                tilemap.SetTile(selectedDictionary[tilemap], oldTile);
            }
            selectedDictionary[tilemap] = tilePosition;
        }
    }


    private void SelectTile()
    {
        Vector3 mousePosition = main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePosition = tilemap.WorldToCell(mousePosition);
        tilePosition.z = 0;

        if (tilePosition != _previousPosition[tilemap])
        {
            if (tilemap.HasTile(tilePosition))
            {
                tilemap.SetTransformMatrix(tilePosition, Matrix4x4.TRS(offSet, Quaternion.Euler(0, 0, 0), Vector3.one));
            }
            if (tilemap.HasTile(_previousPosition[tilemap]))
            {
                tilemap.SetTransformMatrix(_previousPosition[tilemap], Matrix4x4.identity);
            }
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
    

    private void StartDijkstra()
    {
        dijkstra.Origin = _origin[tilemap];
        dijkstra.Goal = _goal[tilemap];
        dijkstra.tileMap = tilemap;
        dijkstra.visitedTile = originTile;
        dijkstra.pathTile = destinationTile;
        StartCoroutine(dijkstra.FloodFill2D());
    } 
    

    private void StartHeuristic()
    {
        heuristic.Origin = _origin[tilemap];
        heuristic.Goal = _goal[tilemap];
        heuristic.tileMap = tilemap;
        heuristic.visitedTile = originTile;
        heuristic.pathTile = destinationTile;
        StartCoroutine(heuristic.FloodFill2D());
    }


    private void StartAStar()
    {
        aStar.Origin = _origin[tilemap];
        aStar.Goal = _goal[tilemap];
        aStar.tileMap = tilemap;
        aStar.visitedTile = originTile;
        aStar.pathTile = destinationTile;
        StartCoroutine(aStar.FloodFill2D());
    }
}
