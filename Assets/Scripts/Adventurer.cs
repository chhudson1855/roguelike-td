using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Adventurer : MonoBehaviour
{
    
    public int health =5; 
    public float gridSize = 0.4f;
    public int movesPerTurn = 4;
    public float moveSpeed = 0.15f;
    
    //coords
  
    public Vector2Int currPos;
    public Vector2Int targetPos;

    private bool isMoving = false;
    
    private void Update()
    {
        if (health <= 0)
        {
            Debug.Log("Adventurer died");
            Destroy(this.gameObject);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Turn();
        }
    }
     //not sure if this is what you meant by make a turn function. Call this function to make the enemy start his turn 

    public void Turn()
    {
        StartCoroutine(TurnMovement());
    }
    private IEnumerator TurnMovement()
    {
        isMoving = true;
        for(int i = 0; i < movesPerTurn; i++)
        {
            if(currPos == targetPos)
            {
                break;
            }
            
            Vector2Int nextStep = GetBestNeighbor (currPos, targetPos);
            
            yield return StartCoroutine(SmoothMove(nextStep));
            
            currPos = nextStep;

        }
        isMoving =false;
    }

    private Vector2Int GetBestNeighbor(Vector2Int current, Vector2Int target)
    {
        Vector2Int[] directions = {Vector2Int.up,Vector2Int.down, Vector2Int.left, Vector2Int.right};

        Vector2Int bestMove = current;
        float shortestDist = float.MaxValue;

        foreach (Vector2Int dir in directions)
        {
            Vector2Int potentialMove = current + dir;
            
            float dist = (potentialMove - target).sqrMagnitude;

            // If this neighbor is closer to the target than our current best, pick it
            if (dist < shortestDist)
            {
                shortestDist = dist;
                bestMove = potentialMove;
            }
        }
        return bestMove;
    }
    private IEnumerator SmoothMove(Vector2Int targetGridPos)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(targetGridPos.x * gridSize, targetGridPos.y * gridSize, transform.position.z);

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
   

    
   
    
}
