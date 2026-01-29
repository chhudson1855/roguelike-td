using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Archer : MonoBehaviour
{
    public int health = 3;

    public float maxRangeTiles = 4f;
    public float minRangeTiles = 1f;
    public float gridSize = 0.4f;

    public GameObject arrowPrefab;


    public IEnumerator Turn()
    {

        foreach (Transform adventurerTarget in GameManager.instance.Entities.transform)
        {
            if (adventurerTarget.gameObject.GetComponent<Adventurer>() == null) continue;

            if (adventurerTarget == null || arrowPrefab == null) yield return true;

            float worldDistance = Vector3.Distance(transform.position, adventurerTarget.position);
            float tileDistance = worldDistance / gridSize;

            if (tileDistance <= maxRangeTiles && tileDistance > minRangeTiles)
            {
                GameObject newArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                Arrow arrowScript = newArrow.GetComponent<Arrow>();
                if (arrowScript != null)
                {
                    arrowScript.Setup(adventurerTarget.position, gridSize);
                    break;
                }
            }
        }

        yield return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.TryGetComponent<Adventurer>(out Adventurer adventurer))
        {
            Destroy(this.gameObject);
        }
    }
}