
using System.Collections;
using UnityEditor.Scripting;
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
    public GameObject unit3Prefab; // archer
    //keeps track of selected troop

    public void BuildEntity(GameObject prefab)
    {
        if (GameManager.instance.Mana >= GameManager.instance.prefabCostMap[prefab.name].cost)
        {
            GameManager.instance.Mana -= GameManager.instance.prefabCostMap[prefab.name].cost;

            GameObject newUnit = Instantiate(prefab, transform.position, Quaternion.identity);
               
            newUnit.transform.parent = GameManager.instance.Entities.transform;
            //spawns on top layers
            SpriteRenderer sr = newUnit.GetComponent<SpriteRenderer>();
            if (sr != null) 
            {
                sr.sortingOrder = 10;
            }

            newUnit.GetComponent<Entity>().Init(xCordinate, yCordinate);
        }
        else
        {
            Debug.Log("Not enough mana to build " + prefab.name);
        }
    }

    private void Update()
    {
        if (GameManager.instance.turn == true)
        {

            Transform select = GameManager.instance.UI.transform.Find("Menu").Find("Select");

            if (select.gameObject.activeSelf == true)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    BuildEntity(unit1Prefab);
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    BuildEntity(unit2Prefab);
                }
                else if (Input.GetKeyDown(KeyCode.C))
                {
                    BuildEntity(unit3Prefab);
                }
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
            if (!(yCordinate > 23))
            {
                yCordinate += 1;
                StartCoroutine(Move(Vector2.up));
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            if (!(yCordinate < 1))
            {
                yCordinate -= 1;
                StartCoroutine(Move(Vector2.down));
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (!(xCordinate < 1))
            {
                xCordinate -= 1;
                StartCoroutine(Move(Vector2.left));
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (!(xCordinate > 44))
            {
                xCordinate += 1;
                StartCoroutine(Move(Vector2.right));
            }
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