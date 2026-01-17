using System.Collections;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    
    public int xCordinate;
    public int yCordinate;

    public bool Walkable = true;
    public bool BuiltOn = false;
    [SerializeField] private float gridSize = 0.4f;

    // Start Hangtime (time to move initially)
    [SerializeField] private float startHTime = 0.1f;
    //max velo
    [SerializeField] private float maxVelo = 0.05f;
    
    //how much time is shaven off per step (acceleration)
    [SerializeField] private float accerlation = 0.02f;
    private bool isMoving = false;
    private float currentHTime;
    private void Update()
    {
        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && 
            !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            currentHTime = startHTime;
        }
        // If already moving, don't accept input
        if (isMoving) return;

        //movement
       if (Input.GetKey(KeyCode.UpArrow))
        {
            StartCoroutine(Move(Vector2.up));
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            StartCoroutine(Move(Vector2.down));
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            StartCoroutine(Move(Vector2.left));
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            StartCoroutine(Move(Vector2.right));
        }
    }

    private IEnumerator Move(Vector2 direction)
    {
        isMoving = true;

        //calc start and end
        Vector2 startPos = transform.position;
        Vector2 endPos = startPos + (direction * gridSize);

        float elapsed = 0;
        //loop until elapsed match curr speed
        while (elapsed < currentHTime)
        {

            elapsed += Time.deltaTime; 

            float percent = elapsed / currentHTime;
            transform.position = Vector2.Lerp(startPos, endPos, percent);
            yield return null;
        }

        //snap to final pos
        transform.position = endPos;
        //accel logic
        currentHTime = Mathf.Max(currentHTime - accerlation, maxVelo);
        isMoving = false;
    }
}