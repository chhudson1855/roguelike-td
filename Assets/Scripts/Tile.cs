using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public List<GameObject> occupiedBy = new List<GameObject>();
    public int xCordinate, yCordinate = 0;
    public bool BuiltOn = false;
    public bool Walkable = false;
    public void Init(int x, int y, List<GameObject> occupants)
    {
        xCordinate = x;
        yCordinate = y;
        occupiedBy = occupants;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
