using UnityEngine;

public class Knight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.TryGetComponent<Adventurer>(out Adventurer adventurer))
        {
            adventurer.myhealth -= 75;
            Destroy(this.gameObject);
        }
    }
}
