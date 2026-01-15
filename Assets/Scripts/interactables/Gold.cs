using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Gold : MonoBehaviour
{
    public int x;
    public int y;

    // Start is called before the first frame update
    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    void Tick()
    {
        GameObject tileObj = GridUtil.GetObject(x, y);
        foreach (GameObject obj in tileObj.GetComponent<Tile>().occupiedBy)
        {
            if (obj.CompareTag("Enemy"))
            {
                Destroy(this.gameObject);
                GameManager.instance.RemoveGold(1);
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
