using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int globalTime;
    public int Mana;
    public int Health;
    public int Round;

    // Start is called before the first frame update
    void Start()
    {
        globalTime = 0;
        Mana = 100;
        Health = 100;
        Round = 1;
    }

    private void Awake()
    {
        // Ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // optional, keep across scenes
        }
        else
        {
            Destroy(gameObject); // destroy duplicates
        }
    }

    public void RemoveGold(int amount)
    {
        Health -= amount;
    }

    void UpdateTime()
    {
        
    }

    void UpdateMana()
    {
        
    }

    void UpdateHealth()
    {
        
    }

    void RoundOver()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTime();
        UpdateMana();
        UpdateHealth();
        RoundOver();
    }
}
