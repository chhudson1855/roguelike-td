using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Gold : MonoBehaviour
{
    public int x;
    public int y;
    public GameObject Entities;

    // Start is called before the first frame update
    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;

        Entities = GameManager.instance.Entities;
    }

    public void Turn()
    {
        foreach (Transform child in Entities.transform)
        {
            GameObject obj = child.gameObject;  // now it’s a GameObject
            Adventurer adv = obj.GetComponent<Adventurer>();
            if (adv == null) continue;

            if (adv.currPos.x == x && adv.currPos.y == y)
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
