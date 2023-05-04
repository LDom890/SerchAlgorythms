using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InitialPos : MonoBehaviour
{
    public LayerMask cubeLayer; // Capa de los cubos en el grid
    public Tilemap cubeTilemap; // el Tilemap que contiene los cubos
    public TileBase tile;



    public Vector3Int currentCellPosition; // la posición de la celda actual

    private Queue<Vector3Int> frontier = new Queue<Vector3Int>();
    private HashSet<Vector3Int> reached = new HashSet<Vector3Int>();

    List<Vector3Int> GetNeighbors(Vector3Int cell)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        Vector3Int[] directions = new Vector3Int[]
        {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };
        foreach (Vector3Int dir in directions)
        {
            Vector3Int neighbor = cell + dir;
            if (cubeTilemap.HasTile(neighbor) && !reached.Contains(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // verifica si el botón izquierdo del mouse ha sido presionado
        {
            Vector3 clickPosition = Input.mousePosition; // obtiene la posición del mouse en la pantalla
            //Debug.Log("Posición en el mundo: " + clickPosition); // imprime la posición del clic en la consola

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // obtiene la posición del mouse en el mundo
            Vector3Int gridPosition = cubeTilemap.WorldToCell(mouseWorldPos); // convierte la posición del mouse al centro de la celda en el Tilemap

            TileBase Origin = cubeTilemap.GetTile(gridPosition); // obtiene el Tile en la posición de la celda
            Vector3Int startCell = cubeTilemap.WorldToCell(mouseWorldPos);
            frontier.Enqueue(startCell);
            reached.Add(startCell);
            
            Debug.Log("Posición del cubo: " + gridPosition + "Posición en el mundo: " + clickPosition); // imprime la posición del cubo en la consola
        }
        if (frontier.Count > 0)
        {

            Vector3Int current = frontier.Dequeue();
            Debug.Log(current);
            foreach (Vector3Int next in GetNeighbors(current))
            {
                if (!reached.Contains(next))
                {
                    frontier.Enqueue(next);
                    reached.Add(next);
                }
                else
                {
                    cubeTilemap.SetTileFlags(next, TileFlags.None);
                    cubeTilemap.SetColor(next, Color.black);
                }
            }
        }
        //SerchAndPrint();
    }

}
