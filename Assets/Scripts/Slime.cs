using UnityEngine;

public class Slime : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.TryGetComponent<Adventurer>(out Adventurer adventurer))
        {
            adventurer.myhealth -= 34;
            Destroy(this.gameObject);
        }
    }
}