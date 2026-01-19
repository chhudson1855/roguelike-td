using System.Collections;
using System.Collections.Generic; 
using UnityEngine;

public class Tile : MonoBehaviour
{
    
    public int xCordinate;
    public int yCordinate;

  
    public bool Walkable = true;
    public bool BuiltOn = false;

  
 
    public List<GameObject> occupiedBy = new List<GameObject>(); 
}