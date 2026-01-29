using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Adventurer : MonoBehaviour
{
    
    public int myhealth = 100;
    public float gridSize = 0.35f;
    public int movesPerTurn = 4;
    public float moveSpeed = 0.15f;

    public Vector3 gridOrigin;
    
    //coords
  
    public Vector2Int currPos;
    public Vector2Int targetPos;

    private bool isMoving = false;

    private MapGenerator mapGen;
    private GameObject buildings;
    private GameObject entities;

    public void Init(int x, int y)
    {
        this.currPos = new Vector2Int(x, y);
    }

    private void Start()
    {
        mapGen = FindObjectOfType<MapGenerator>();
        entities = GameManager.instance.Entities;
        buildings = GameManager.instance.Buildings;

        if (mapGen == null)
        {
            Debug.LogError("MapGenerator not found in the scene!");
        }
    }
    
    private void Update()
    {
        if (myhealth <= 0 && myhealth > -10)
        {
            Debug.Log("Adventurer died");

            GameManager.instance.Mana += 5;
            GameManager.instance.Mana *= 2;

            Debug.Log("Create new");

            int mx = (int)mapGen.portal.GetComponent<Entity>().xCordinate;
            int my = (int)mapGen.portal.GetComponent<Entity>().yCordinate;

            Debug.Log(mx + " " + my);

            GameObject adventurer = Instantiate(Resources.Load<GameObject>("Prefabs/Adventurer"), GameManager.instance.Entities.transform);

            adventurer.transform.localPosition = new Vector3(
                -8.6f + (mx * 0.4f),
                -10.55f + (my * 0.4f),
                0f
            );

            adventurer.GetComponent<Adventurer>().Init(mx, my);
            adventurer.GetComponent<Entity>().Init(mx, my);
            adventurer.name = "Adventurer";

            myhealth = -100;
            return;
        }
    }
     //not sure if this is what you meant by make a turn function. Call this function to make the enemy start his turn 

    public new IEnumerator Turn()
    {
        // Determine the target
        Vector2Int? target = FindClosestTarget();
        

        if (target.HasValue)
        {
            targetPos = target.Value;
        }
        else
        {
            // Fallback: move to a random tile in another room
            targetPos = GetRandomTileInAnotherRoom();
        }

        // Move towards the target using the existing TurnMovement
        yield return TurnMovement();
    }

    private IEnumerator TurnMovement()
    {
        isMoving = true;

        for (int i = 0; i < movesPerTurn; i++)
        {
            if (currPos == targetPos)
                break;

            // Get next step using BFS
            Vector2Int nextStep = GetNextStepBFS(currPos, targetPos);

            Debug.Log($"Curr: {currPos}, Target: {targetPos}, NextStep: {nextStep}");

            // Smooth move to next step
            yield return StartCoroutine(SmoothMove(nextStep));

            currPos = nextStep;
        }

        isMoving = false;

        yield return new WaitForSeconds(0.5f);
    }

    private Vector2Int? FindClosestTarget()
    {
        Vector2Int closestPos = new Vector2Int();
        float closestDist = float.MaxValue;
        bool found = false;

        // 1️⃣ Check Buildings (Gold)
        if (buildings != null)
        {
            foreach (Transform child in buildings.transform)
            {
                Gold gold = child.GetComponent<Gold>();
                if (gold == null) continue;

                Vector2Int pos = new Vector2Int(gold.x, gold.y);
                float dist = (pos - currPos).sqrMagnitude;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestPos = pos;
                    found = true;
                }
            }
        }

        // 2️⃣ Check Entities (any object with Entity component)
        if (entities != null)
        {
            foreach (Transform child in entities.transform)
            {
                Entity ent = child.GetComponent<Entity>();
                if (ent == null || ent.gameObject == this.gameObject) continue;

                Vector2Int pos = new Vector2Int((int)ent.xCordinate, (int)ent.yCordinate);
                float dist = (pos - currPos).sqrMagnitude;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestPos = pos;
                    found = true;
                }
            }
        }

        if (found)
            return closestPos;

        return null; // nothing found
    }


    private Vector2Int GetNextStepBFS(Vector2Int start, Vector2Int target)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[start] = start;

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current == target)
                break;

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = current + dir;

                // Skip if already visited
                if (cameFrom.ContainsKey(neighbor)) continue;

                GameObject tileObj = GridUtil.GetObject(neighbor.x, neighbor.y);
                if (tileObj == null) continue;

                Tile tile = tileObj.GetComponent<Tile>();
                if (tile == null || !tile.Walkable) continue; // respect walls

                frontier.Enqueue(neighbor);
                cameFrom[neighbor] = current;
            }
        }

        // If target is unreachable, stay in place
        if (!cameFrom.ContainsKey(target))
            return start;

        // Reconstruct path from target to start
        Vector2Int step = target;
        List<Vector2Int> path = new List<Vector2Int>();
        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Reverse();

        // Return the first step
        return path.Count > 0 ? path[0] : start;
    }
    

    private IEnumerator SmoothMove(Vector2Int targetGridPos)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(
            gridOrigin.x + targetGridPos.x * gridSize,
            gridOrigin.y + targetGridPos.y * gridSize,
            transform.position.z
        );
        
        float elapsed = 0;
        while (elapsed < moveSpeed)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / moveSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, percent);
            yield return null;
        }
        transform.position = endPos;
    }

   

    
    private Vector2Int GetRandomTileInAnotherRoom()
    {
        if (mapGen == null || mapGen.existingRooms.Count == 0)
            return currPos; // fallback to staying in place

        // Choose a random room that is different from current position
        List<RectInt> otherRooms = new List<RectInt>();
        foreach (RectInt room in mapGen.existingRooms)
        {
            if (!room.Contains(currPos))
                otherRooms.Add(room);
        }

        if (otherRooms.Count == 0)
            return currPos; // no other room available

        RectInt chosenRoom = otherRooms[Random.Range(0, otherRooms.Count)];

        // Pick a random walkable tile in the chosen room
        Vector2Int randomTile = new Vector2Int(
            Random.Range(chosenRoom.x + 1, chosenRoom.x + chosenRoom.width - 1),
            Random.Range(chosenRoom.y + 1, chosenRoom.y + chosenRoom.height - 1)
        );

        GameObject tileObj = GridUtil.GetObject(randomTile.x, randomTile.y);
        if (tileObj != null)
        {
            Tile tile = tileObj.GetComponent<Tile>();
            if (tile != null && tile.Walkable)
            {
                return randomTile;
            }
        }

        return currPos; // fallback
    }
    
}
