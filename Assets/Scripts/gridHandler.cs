using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class gridHandler : MonoBehaviour
{
    public string board_name = "BoardTiles";
    public int board_height = 8;
    public int board_width = 12;
    public Tilemap board_tilemap;
    private int[,] matrix;

    [SerializeField] Tile basic_tile;
    [SerializeField] Tile hover_tile;
   
    void Awake()
    {
        matrix = new int[board_height, board_width];
        board_tilemap = GameObject.Find(board_name).GetComponent<Tilemap>();
        board_tilemap.tileAnchor = Vector3.zero;
        for (int i = 0; i< board_height;i++) {
            for (int j = 0; j < board_width; j++) {
                Vector3Int current_pos = new Vector3Int(j, i, 0);
                board_tilemap.SetTile(ConvertToTilemapGrid(current_pos), basic_tile);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            //OnMouseOver();
        }
        
    }

    public Vector3Int ConvertToTilemapGrid(Vector3Int matrixPos) {
        Vector3Int ret_val = new Vector3Int(matrixPos.x - Mathf.FloorToInt(board_width/2), matrixPos.y - Mathf.FloorToInt(board_height / 2), 0);
        return ret_val;
    }

    public Vector3Int tilemapGridToMatrixPos(Vector3Int tileMapGridPos)
    {
        Vector3Int ret_val = new Vector3Int(tileMapGridPos.x + Mathf.FloorToInt(board_width / 2), tileMapGridPos.y + Mathf.FloorToInt(board_height / 2), 0);
        return ret_val;
    }

    /// <summary> 0,0 is bottom left corner </summary>
    public Vector3 matrixPosToWorld(Vector3Int position)
    {
        Vector3Int tile_pos = ConvertToTilemapGrid(position);
        Vector3 ret_val = board_tilemap.CellToWorld(tile_pos);
        ret_val.x += board_tilemap.tileAnchor.x;
        ret_val.y += board_tilemap.tileAnchor.y;
        return ret_val;
    }

    /// <summary> returns the position snapped to the grid </summary>
    public Vector3 snapWorldPositionToBoard(Vector3 positionInWorld) {

        Vector3 newPos = positionInWorld + new Vector3(0.5f, 0.5f, 0) - board_tilemap.tileAnchor;
        newPos = board_tilemap.WorldToCell(positionInWorld);
        return newPos;
    }

    public Vector3Int mousePositionToTileMapPosition(Vector3 mousePosition) {
        Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition) + new Vector3(0.5f, 0.5f, 0);
        return board_tilemap.WorldToCell(position);
            
    }

    void update_matrix() {

        Vector3Int playerPos = GameObject.Find("Player").GetComponent<playerMovement>().playerPos;
        Vector3Int enemyPos = GameObject.Find("Enemy").GetComponent<enemyMovement>().enemyPos;
        matrix = new int[board_height, board_width];
        matrix[playerPos.x, playerPos.y] = 1;
        matrix[enemyPos.x, enemyPos.y] = 2;

    }



    private void OnMouseOver() {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition) +new Vector3(0.5f,0.5f,0);
        
        Debug.Log(Input.mousePosition + "  vs  " + board_tilemap.WorldToCell(position));
        if (board_tilemap.HasTile(board_tilemap.WorldToCell(position)))
        { 
            board_tilemap.SetTile(board_tilemap.WorldToCell(position), hover_tile);

            Debug.Log("PLACED on " + position);
        }
      
    }

    //<summary> shows and returns possible moves from a given start position that are distance away </summary>
    public HashSet<Vector3Int> possible_moves(Vector3Int start, int distance) {
        ArrayList path = new ArrayList();
        Queue<Vector3Int> q = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        
        visited.Add(start);
        q.Enqueue(start);
        while (q.Count > 0) {
            Vector3Int test = q.Dequeue();
            visited.Add(test);
            board_tilemap.SetTile(ConvertToTilemapGrid(test), hover_tile);

            Vector3Int up = new Vector3Int(test.x, test.y+1, test.z);
            Vector3Int right = new Vector3Int(test.x + 1, test.y, test.z);
            Vector3Int down = new Vector3Int(test.x, test.y-1, test.z);
            Vector3Int left = new Vector3Int(test.x-1, test.y, test.z);
            Vector3Int[] directions = { up, right, left, down };

            foreach(Vector3Int direction in directions) {
                if (!visited.Contains(direction))
                {
                    if (validCoordinate(direction, start, distance))
                    {
                        q.Enqueue(direction);
                    }
                }
                
            }
            
            
        }
        

        return visited;
    }

    public class BFS_Node: IEquatable<BFS_Node>{
        public BFS_Node parent;
        public Vector3Int coordinates;
        public BFS_Node(BFS_Node _parent, Vector3Int _coordinates) {
            parent = _parent;
            coordinates = _coordinates;
        }

        public bool Equals(BFS_Node other)
        {
            return (coordinates).Equals(other.coordinates);
        }
    }
    public ArrayList BFS_Path(Vector3Int start, Vector3Int end)
    {

        ArrayList path = new ArrayList();
        Queue<BFS_Node> q = new Queue<BFS_Node>();
        HashSet<BFS_Node> visited = new HashSet<BFS_Node>();
        BFS_Node begin = new BFS_Node(null, start);
        visited.Add(begin);
        q.Enqueue(begin);
        BFS_Node final = null;
        while (q.Count > 0)
        {
            BFS_Node test = q.Dequeue();
            if(test.coordinates == end){
                final = test;
            }
            visited.Add(test);
            board_tilemap.SetTile(ConvertToTilemapGrid(test.coordinates), hover_tile);
            
            Vector3Int up = new Vector3Int(test.coordinates.x, test.coordinates.y + 1, test.coordinates.z);
            Vector3Int right = new Vector3Int(test.coordinates.x + 1, test.coordinates.y, test.coordinates.z);
            Vector3Int down = new Vector3Int(test.coordinates.x, test.coordinates.y - 1, test.coordinates.z);
            Vector3Int left = new Vector3Int(test.coordinates.x - 1, test.coordinates.y, test.coordinates.z);
            BFS_Node[] directions = { new BFS_Node(test, up), new BFS_Node(test, right), new BFS_Node(test, down), new BFS_Node(test, left) };

            foreach (BFS_Node direction in directions)
            {
                if (!visited.Contains(direction))
                {
                    if (validCoordinate(direction.coordinates, start, board_height+board_width))
                    {
                        q.Enqueue(direction);
                    }
                }

            }

        }

        while (final != null) {
            path.Add(final.coordinates);
            final = final.parent;
        }
        return path;
    }

    public void clearPaths() {
        for (int i = 0; i < board_height; i++) {
            for (int j = 0; j < board_width; j++) {
                board_tilemap.SetTile(ConvertToTilemapGrid(new Vector3Int(j, i, 0)), basic_tile);
            }
        }
    }

    private bool validCoordinate(Vector3Int coordinate, Vector3Int start, int allowed_distance)
    {
        bool valid_distance = false;
        if (Mathf.Abs(start.x - coordinate.x) + Mathf.Abs(start.y - coordinate.y) <= allowed_distance) {
            valid_distance = true;
        }

        return (coordinate.x < board_width && coordinate.y < board_height && coordinate.x >= 0 && coordinate.y >= 0) && valid_distance;
    }

}


