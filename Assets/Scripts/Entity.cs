using UnityEngine;

public class Entity : MonoBehaviour
{
    public double xCordinate;
    public double yCordinate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Init(int x, int y)
    {
        this.xCordinate = x;
        this.yCordinate = y;
    }

    void Start()
    {
        
    }
}
