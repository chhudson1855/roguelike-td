using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;

public class MapGenerator : MonoBehaviour
{
    public int width = 44;
    public int height = 25;
    public float spacing = 1f;

    public float originX = -8.6f;
    public float originY = -10.55f;

    public float piles;


    private int totalTiles => width * height;
    private int coveredTiles = 0; // count of walkable tiles
    private int targetCoverage = 200;


    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform gridTransform;
    [SerializeField] private Transform buildingsTransform;

    void Start()
    {
        GenerateGrid();

        GenerateMap();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tile = Instantiate(tilePrefab, gridTransform);

                tile.transform.localPosition = new Vector3(
                    originX + (x * spacing),
                    originY + (y * spacing),
                    0f
                );

                Tile tileScript = tile.GetComponent<Tile>();
                if (tileScript != null)
                {
                    tileScript.xCordinate = x;
                    tileScript.yCordinate = y;
                }
            }
        }
    }

    private List<RectInt> existingRooms = new List<RectInt>();
    private List<Vector2Int> roomCenters = new List<Vector2Int>();


    void CarveTile(int x, int y, ref int doors)
    {
        GameObject tileObj = GridUtil.GetObject(x, y);
        if (tileObj == null) return;

        Tile tile = tileObj.GetComponent<Tile>();
        if (tile == null) return;
        
        if (tile.BuiltOn)
        {
            if (doors > 0)
            {
                SpriteRenderer srEntrance = tile.GetComponent<SpriteRenderer>();
                if (srEntrance != null)
                {
                    srEntrance.sprite = Resources.Load<Sprite>("Sprites/dungeondoor");
                    srEntrance.color = new Color(0.6f, 0.3f, 0f);
                }
                doors--;
            }
            else
            {
                SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = Resources.Load<Sprite>("Sprites/walkablesprite");
                    sr.color = Color.white;
                }
            }
        }
        else
        {
            SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = Resources.Load<Sprite>("Sprites/walkablesprite");
                sr.color = Color.white;
            }
        }

        tile.Walkable = true;
    }

    void ConnectTiles(Vector2Int from, Vector2Int to)
    {
        int x = from.x;
        int y = from.y;

        bool horizontalFirst = Random.value < 0.5f; // random L-shape

        if (horizontalFirst)
        {
            int doors = 2;
            while (x != to.x)
            {
                CarveTile(x, y, ref doors);
                x += (to.x > x) ? 1 : -1;
            }
            while (y != to.y)
            {
                CarveTile(x, y, ref doors);
                y += (to.y > y) ? 1 : -1;
            }
        }
        else
        {
            int doors = 2;
            while (y != to.y)
            {
                CarveTile(x, y, ref doors);
                y += (to.y > y) ? 1 : -1;
            }
            while (x != to.x)
            {
                CarveTile(x, y, ref doors);
                x += (to.x > x) ? 1 : -1;
            }
        }
    }

    Vector2Int GetRandomRoomConnection(RectInt room)
    {
        int x = Random.Range(room.x + 1, room.x + room.width - 1);   // avoid left/right edge
        int y = Random.Range(room.y + 1, room.y + room.height - 1);  // avoid top/bottom edge
        return new Vector2Int(x, y);
    }

    void ConnectRoomsMST()
    {
        if (roomCenters.Count <= 1) return;

        List<Vector2Int> connected = new List<Vector2Int>();
        List<RectInt> unconnectedRooms = new List<RectInt>(existingRooms);

        // Start with the first room
        RectInt firstRoom = unconnectedRooms[0];
        connected.Add(GetRandomRoomConnection(firstRoom));
        unconnectedRooms.RemoveAt(0);

        while (unconnectedRooms.Count > 0)
        {
            float closestDistance = float.MaxValue;
            Vector2Int from = Vector2Int.zero;
            Vector2Int to = Vector2Int.zero;
            int indexToRemove = -1;

            for (int i = 0; i < unconnectedRooms.Count; i++)
            {
                RectInt room = unconnectedRooms[i];
                Vector2Int connectionPoint = GetRandomRoomConnection(room);

                foreach (Vector2Int c in connected)
                {
                    float dist = Vector2Int.Distance(c, connectionPoint);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        from = c;
                        to = connectionPoint;
                        indexToRemove = i;
                    }
                }
            }

            // Connect the tiles
            ConnectTiles(from, to);

            // Mark room as connected
            connected.Add(to);
            unconnectedRooms.RemoveAt(indexToRemove);
        }
    }


    bool DoesRoomOverlap(int startX, int startY, int roomWidth, int roomHeight)
    {
        // Check map edges: leave at least 1 tile margin from edges
        if (startX < 1 || startY < 1 || startX + roomWidth + 1 > width - 1 || startY + roomHeight + 1 > height - 1)
        {
            return true; // too close to edge
        }

        // Create a rectangle expanded by 1 tile on all sides for spacing
        RectInt newRoom = new RectInt(startX - 1, startY - 1, roomWidth + 2, roomHeight + 2);

        // Check overlap with all existing rooms
        foreach (RectInt room in existingRooms)
        {
            if (newRoom.Overlaps(room))
            {
                return true; // overlaps existing room or too close
            }
        }

        return false; // no overlap, room can be placed
    }


    void BuildRoom(int startX, int startY)
    {
        // Random size for the room
        int roomWidth = Random.Range(7, 10);
        int roomHeight = Random.Range(7, 10);

        int roomTileCount = 0; // number of tiles added for this room

        if (DoesRoomOverlap(startX, startY, roomWidth, roomHeight))
        {
            Debug.Log($"Skipped room at ({startX},{startY}) due to overlap.");
            return; // skip building this room entirely
        }

        Vector2Int roomCenter = new Vector2Int(
            startX + roomWidth / 2,
            startY + roomHeight / 2
        );
        roomCenters.Add(roomCenter);

        // Add the room to the list to prevent future overlaps
        existingRooms.Add(new RectInt(startX, startY, roomWidth, roomHeight));

        for (int x = startX; x < startX + roomWidth; x++)
        {
            for (int y = startY; y < startY + roomHeight; y++)
            {
                GameObject tileObj = GridUtil.GetObject(x, y);
                if (tileObj == null) continue;

                Tile tile = tileObj.GetComponent<Tile>();
                if (tile == null) continue;

                if (tile.BuiltOn || tile.Walkable)
                    continue;

                bool isBorder = (x == startX || x == startX + roomWidth - 1 ||
                                y == startY || y == startY + roomHeight - 1);

                SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
                roomTileCount++;
                if (isBorder)
                {
                    tile.BuiltOn = true;

                    sr.color = Color.gray;
                }
                else
                {
                    tile.Walkable = true;

                    if (sr != null)
                    {
                        sr.sprite = Resources.Load<Sprite>("Sprites/walkablesprite");
                        sr.color = Color.white;
                    }
                }
            }
        }

        coveredTiles += roomTileCount;
    }

    void BuildObjects()
    {
        // Placeholder for future object placement logic

        for (int i = 0; i < piles; i++)
        {
            int random = Random.Range(0, existingRooms.Count);
            RectInt room = existingRooms[random];

            int x = Random.Range(room.x + 1, room.x + room.width - 1);   // avoid left/right edge
            int y = Random.Range(room.y + 1, room.y + room.height - 1);  // avoid top/bottom edge

            GameObject tileObj = GridUtil.GetObject(x, y);
            if (tileObj == null) continue;

            Tile tile = tileObj.GetComponent<Tile>();

            if (!tile.BuiltOn && tile.Walkable)
            {
                GameObject gold = Instantiate(Resources.Load<GameObject>("Prefabs/gold_pile_0"), buildingsTransform);
                tile.occupiedBy.Add(gold);

                gold.transform.localPosition = new Vector3(
                    originX + (x * spacing),
                    originY + (y * spacing),
                    0f
                );
            }
        }
    }


    void GenerateMap()
    {
        targetCoverage = Mathf.CeilToInt(totalTiles * 0.2f); // 20% of map

        int attempts = 0;
        int maxAttempts = 1000; // prevent infinite loop

        while (coveredTiles < targetCoverage && attempts < maxAttempts)
        {
            attempts++;

            // Pick a random starting tile
            int startX = Random.Range(0, width);
            int startY = Random.Range(0, height);

            BuildRoom(startX, startY);
        }

        BuildObjects();

        // Connect all rooms after they are generated
        ConnectRoomsMST();

        Debug.Log($"Generated {existingRooms.Count} rooms covering {coveredTiles} tiles ({(coveredTiles * 100f / totalTiles):F1}% coverage).");

        foreach (Transform tileTransform in gridTransform)
        {
            GameObject tileObj = tileTransform.gameObject;
            Tile tile = tileObj.GetComponent<Tile>();

            if (tile == null || !tile.Walkable)
                continue;

            GameObject[] cardinals = GridUtil.GetCardinals(tile.xCordinate, tile.yCordinate);
            string[] directions = { "North", "East", "South", "West" };

            for (int i = 0; i < cardinals.Length; i++)
            {
                GameObject neighbor = cardinals[i];
                if (neighbor == null) continue;

                Tile neighborTile = neighbor.GetComponent<Tile>();
                if (neighborTile == null) continue;

                if (!neighborTile.Walkable && !neighborTile.BuiltOn)
                {
                    Transform wallTransform = tileTransform.Find(directions[i]);
                    if (wallTransform == null) continue;

                    SpriteRenderer srWall = wallTransform.GetComponent<SpriteRenderer>();
                    if (srWall != null)
                    {
                        srWall.color = Color.gray;
                    }
                }
            }
        }

    }
}
