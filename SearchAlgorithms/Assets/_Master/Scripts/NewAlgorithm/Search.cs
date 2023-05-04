using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Search : MonoBehaviour
{
    private Queue<Vector3> _frontier = new Queue<Vector3>();
    private Dictionary<Vector3, Vector3> _came = new Dictionary<Vector3, Vector3>();
    public Vector3 Origin { get; set; }
    public Vector3 Goal { get; set; }
    public Tilemap tileMap;
    public TileBase visitedTile;
    public TileBase pathTile;
    public float delay = 0.4f;
    private bool isEarlyExit = false;

    public IEnumerator FloodFill2D()
    {
        _frontier.Enqueue(Origin);
        _came[Origin] = Vector3.zero;

        while (_frontier.Count > 0 & !isEarlyExit)
        {
            Vector3 current = _frontier.Dequeue();
            foreach (Vector3 next in GetNeighbours(current))
            {
                if (next == Goal)
                {
                    isEarlyExit = true;
                }
                if (!_came.ContainsKey(next))
                {
                    yield return new WaitForSeconds(delay);
                    _frontier.Enqueue(next);
                    _came[next] = current;
                }
            }
        }
        DrawPath(Goal);
        //yield return _frontier;
    }
    public void DrawPath(Vector3 goal)
    {
        Vector3 current = goal;
        while (current != Origin)
        {
            Vector3Int currentInt = new Vector3Int((int) current.x, (int) current.y, (int) current.z);
            tileMap.SetTile(currentInt, pathTile);
            current = _came[current];
        }
    }

    List<Vector3> GetNeighbours(Vector3 current)
    {
        List<Vector3> neighbours = new List<Vector3>();
        ValidateCoord(current + Vector3.right, neighbours);
        ValidateCoord(current + Vector3.left, neighbours);
        ValidateCoord(current + Vector3.up, neighbours);
        ValidateCoord(current + Vector3.down, neighbours);
        return neighbours;
    }
    void ValidateCoord(Vector3 neighbour, List<Vector3> neighbours)
    {
        Vector3Int coordInt = new Vector3Int((int) neighbour.x, (int) neighbour.y, (int) neighbour.z);

        if (!tileMap.HasTile(coordInt)) return;
        if (!_frontier.Contains(coordInt))
        {
            //TileFlags flags = tileMap.GetTileFlags(coordInt);
            neighbours.Add(neighbour);
            tileMap.SetTile(coordInt, visitedTile);
            //tileMap.SetTileFlags(coordInt, flags);

        }
    }
}
