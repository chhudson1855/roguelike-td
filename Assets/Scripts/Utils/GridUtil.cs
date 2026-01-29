using UnityEngine;

public static class GridUtil
{
    private static GameObject grid;
    private static GameObject entities;

    /// Call once before using GetObject
    public static void Initialize()
    {
        if (grid == null)
        {
            grid = GameObject.Find("Grid");
            if (grid == null)
            {
                Debug.LogError("Grid GameObject not found in scene!");
            }
        }
        if (entities == null)
        {
            entities = GameObject.Find("Entities");
            if (entities == null)
            {
                Debug.LogError("Entities GameObject not found in scene!");
            }
        }
    }

    public static GameObject GetObject(int x, int y)
    {
        Initialize();

        foreach (Transform child in grid.transform)
        {
            Tile tile = child.GetComponent<Tile>();
            if (tile == null) continue;

            if (tile.xCordinate == x && tile.yCordinate == y)
            {
                return child.gameObject;
            }
        }

        return null; // not found
    }

    public static GameObject FindEntity(int x, int y)
    {
        Initialize();

        foreach (Transform child in entities.transform)
        {
            Tile tile = child.GetComponent<Tile>();
            if (tile == null) continue;

            if (tile.xCordinate == x && tile.yCordinate == y)
            {
                return child.gameObject;
            }
        }

        return null; // not found
    }

    public static GameObject[] GetCardinals(int x, int y, bool include = false)
    {
        if (include)
        {
            return new GameObject[]
            {
                GridUtil.GetObject(x, y+1),
                GridUtil.GetObject(x+1, y+1),
                GridUtil.GetObject(x-1, y-1),
                GridUtil.GetObject(x-1, y+1),
                GridUtil.GetObject(x+1, y-1),
                GridUtil.GetObject(x+1, y),
                GridUtil.GetObject(x, y-1),
                GridUtil.GetObject(x-1, y)
            };
        }
        else
        {
            return new GameObject[]
            {
                GridUtil.GetObject(x, y+1), //N
                GridUtil.GetObject(x+1, y), //E
                GridUtil.GetObject(x, y-1), //S
                GridUtil.GetObject(x-1, y)  //W
            };
        }
    }
}
