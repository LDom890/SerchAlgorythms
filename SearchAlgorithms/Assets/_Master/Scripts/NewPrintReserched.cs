using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NewPrintReserched : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase tile;
    public Vector3Int start;

    private HashSet<Vector3Int> reached = new HashSet<Vector3Int>();

   
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BFS(start);

        }
    }
    private void BFS(Vector3Int start)
    {
        Queue<Vector3Int> frontier = new Queue<Vector3Int>();
        frontier.Enqueue(start);
        reached.Add(start);
        tilemap.SetTile(start, tile); // Set the start tile

        while (frontier.Count > 0)
        {
            Vector3Int current = frontier.Dequeue();

            foreach (Vector3Int next in GetNeighbors(current))
            {
                if (!reached.Contains(next))
                {
                    frontier.Enqueue(next);
                    reached.Add(next);
                    tilemap.SetTile(next, tile); // Set the new tile for the neighbor that has just been reached
                }
            }
        }
    }

    private IEnumerable<Vector3Int> GetNeighbors(Vector3Int current)
    {
        // Get the coordinates of the neighboring cells
        Vector3Int[] neighbors = new Vector3Int[]
        {
            new Vector3Int(current.x + 1, current.y, current.z),
            new Vector3Int(current.x - 1, current.y, current.z),
            new Vector3Int(current.x, current.y + 1, current.z),
            new Vector3Int(current.x, current.y - 1, current.z)
        };

        // Check if the neighboring cells are within the bounds of the grid and contain the target tile
        foreach (Vector3Int neighbor in neighbors)
        {
            if (tilemap.HasTile(neighbor) && !reached.Contains(neighbor))
            {
                yield return neighbor;
            }
        }
    }
}