using ESarkis;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class Heuristic : MonoBehaviour
{

    private Dictionary<Vector3, Vector3> _came = new Dictionary<Vector3, Vector3>();
    private PriorityQueue<Vector3> _frontier = new PriorityQueue<Vector3>();
    private Dictionary<Vector3, double> _costSoFar = new Dictionary<Vector3, double>();

    public Vector3 Origin { get; set; }
    public Vector3 Goal { get; set; }
    public Tilemap tileMap;
    public TileBase visitedTile;
    public TileBase pathTile;
    public float delay = 0.01f;
    private bool isEarlyExit = false;

    public IEnumerator FloodFill2D()
    {
        _frontier.Enqueue(Origin,0);
        _came[Origin] = Vector3.zero;
        _costSoFar[Origin] = 0;

        while (_frontier.Count > 0 & !isEarlyExit)
        {
            Vector3 current = _frontier.Dequeue();
            
            foreach (Vector3 next in GetNeighbours(current))
            {
                if (next == Goal) { isEarlyExit = true;}
                var newCost = _costSoFar[current] + GetCost(next);
                if (!_costSoFar.ContainsKey(next) || newCost < _costSoFar[next])
                {
                    yield return new WaitForSeconds(delay);
                    _costSoFar[next] = GetHeuristic(Goal,next);
                    _frontier.Enqueue(next, newCost);
                    _came[next] = current;
                }
            }
        }
        DrawPath(Goal);
        
    }

   
    private double GetCost(Vector3 next)
    {
        var nextTile = tileMap.GetTile(new Vector3Int((int)next.x, (int)next.y, (int)next.z));
        double cost = nextTile.name switch
        {
            "isometric_angled_pixel_0040" => 1,
            "isometric_angled_pixel_0019" => 20,
            "isometric_angled_pixel_0014" => 30,
            "isometric_angled_pixel_0037" => 40,
            _ => 1,
        };

        
       return cost;

    }
    public void DrawPath(Vector3 goal)
    {
        Vector3 current = goal;
        while (current != Origin)
        {
            Vector3Int currentInt = new Vector3Int((int)current.x, (int)current.y, (int)current.z);
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
        Vector3Int coordInt = new Vector3Int((int)neighbour.x, (int)neighbour.y, (int)neighbour.z);

        if (!tileMap.HasTile(coordInt)) return;
        if (!_frontier.Contains(coordInt))
        {
            neighbours.Add(neighbour);
            //tileMap.SetTile(coordInt, visitedTile);
        }
    }

    float GetHeuristic(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }
}
