using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed = 3.0f; 
    private int damage = 1;
    
    
    private bool hasHit = false; 

    public void Setup(Vector3 target, float gridSpeed)
    {
        targetPosition = target;
        
        speed = gridSpeed * 5f; 
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        
        // Optional: Destroy if it reaches the destination without hitting anything
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (hasHit) return;

        Adventurer player = collision.GetComponent<Adventurer>();
        
        if (player != null)
        {
           
            hasHit = true; 

            player.myhealth -= 20;
            Debug.Log("Adventurer hit! HP: " + player.myhealth);
            
            Destroy(gameObject); 
        }
    }
}