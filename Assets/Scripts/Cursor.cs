
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
    [SerializeField] private float acceleration = 0.02f;
    private bool isMoving = false;
    private float currentHTime;
    
    //spawning 
    public GameObject unit1Prefab; // slime
    public GameObject unit2Prefab; // knight
    public GameObject unit3Prefab; //archer
    //keeps track of selected troop
    private GameObject selectedPrefab;
    

    private void Update()
    {
        //selecting troop
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedPrefab = unit1Prefab;
            Debug.Log("Selected slime");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedPrefab = unit2Prefab;
            Debug.Log("Selected knight");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedPrefab = unit3Prefab;
            Debug.Log("Selected archer");
        }

        //enter to build the selected troop
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (selectedPrefab != null)
            {
                // Create the unit
                GameObject newUnit = Instantiate(selectedPrefab, transform.position, Quaternion.identity);
                
                //spawns on top layers
                SpriteRenderer sr = newUnit.GetComponent<SpriteRenderer>();
                if (sr != null) 
                {
                    sr.sortingOrder = 10;
                }
            }
            else
            {
                Debug.Log("No unit selected! Press 1, 2, 3, or 4 first.");
            }
        }
        //movement
        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && 
            !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            currentHTime = startHTime;
        }
        // If already moving, don't accept input
        if (isMoving) return;

        //all diff  type of movement
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
        currentHTime = Mathf.Max(currentHTime - acceleration, maxVelo);
        isMoving = false;
    }
}