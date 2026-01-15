using System.Collections;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    
    public int xCordinate;
    public int yCordinate;

    public bool Walkable = true;
    public bool BuiltOn = false;
    // Allow holding down for movement
    [SerializeField] private bool isHoldDown = false;
    // Hangtime (time to move)
    [SerializeField] private float hTime = 0.1f;
    // Size of grid 
    [SerializeField] private float gridSize = 0.4f;

    private bool isMoving = false;

    private void Update()
    {
        // If already moving, don't accept input
        if (isMoving) return;

        System.Func<KeyCode, bool> inputFunction;

        if (isHoldDown)
        {
            // GetKey repeatedly fires
            inputFunction = Input.GetKey;
        }
        else
        {
            // GetKeyDown fires once per keypress
            inputFunction = Input.GetKeyDown;
        }

        // If input func active, move in direction
        if (inputFunction(KeyCode.UpArrow))
        {
            StartCoroutine(Move(Vector2.up));
        }
        else if (inputFunction(KeyCode.DownArrow))
        {
            StartCoroutine(Move(Vector2.down));
        }
        else if (inputFunction(KeyCode.LeftArrow))
        {
            StartCoroutine(Move(Vector2.left));
        }
        else if (inputFunction(KeyCode.RightArrow))
        {
            StartCoroutine(Move(Vector2.right));
        }
    }

    private IEnumerator Move(Vector2 direction)
    {
        isMoving = true;

        Vector2 startPos = transform.position;
        Vector2 endPos = startPos + (direction * gridSize);

        float elapsed = 0;

        while (elapsed < hTime)
        {

            elapsed += Time.deltaTime; 

            float percent = elapsed / hTime;
            transform.position = Vector2.Lerp(startPos, endPos, percent);
            yield return null;
        }

       
        transform.position = endPos;

        isMoving = false;
    }
}