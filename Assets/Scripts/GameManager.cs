using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int globalTime;
    public int Mana;
    public int Health;
    public int Round;
    public int Piles;
    public bool turn;

    public GameObject UI;

    public GameObject Entities;
    public GameObject Buildings;

    public Dictionary<string, (GameObject prefab, int cost)> prefabCostMap;

    // Start is called before the first frame update
    void Start()
    {
        turn = true;
    }

    private void Awake()
    {
        prefabCostMap =
        new Dictionary<string, (GameObject, int)>
        {
            { "Archer", (Resources.Load<GameObject>("Prefabs/Archer"), 15) },
            { "Slime", (Resources.Load<GameObject>("Prefabs/Slime"), 5) },
            { "Knight", (Resources.Load<GameObject>("Prefabs/Knight"), 25) }
        };
        
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

    bool turnRoutineRunning = false;
    IEnumerator UpdateTime()
    {
        turnRoutineRunning = true;
        turn = false;
        
        UI.transform.Find("Turn").GetComponent<TextMeshProUGUI>().text = "Enemy Turn";



        foreach (Transform a in Entities.transform)
        {
            Adventurer adv = a.GetComponent<Adventurer>();
            if (adv != null)
                yield return StartCoroutine(adv.Turn());

            Archer arch = a.GetComponent<Archer>();
            if (arch != null)
                yield return StartCoroutine(arch.Turn());
        }

        foreach (Transform adventurer in Entities.transform)
        {
            if (adventurer.name == "Adventurer")
            {
                Adventurer a = adventurer.GetComponent<Adventurer>();

                if (a.myhealth < -10 && a.gameObject != null)
                {
                    Destroy(a.gameObject);
                }
            }
            if (adventurer.name == "Archer")
            {
                Archer a = adventurer.GetComponent<Archer>();

                if (a.health < -10 && a.gameObject != null)
                {
                    Destroy(a.gameObject);
                }
            }
        }

        foreach (Transform building in Buildings.transform)
        {
            if (building.name == "Gold")
            {
                Gold g = building.GetComponent<Gold>();
                g.Turn();
            }
        }

        UI.transform.Find("Turn").GetComponent<TextMeshProUGUI>().text = "Your Turn";

        turn = true;
        turnRoutineRunning = false;

        Mana += 10;
    }

    void UpdateUI()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Transform select = UI.transform.Find("Menu").Find("Select");
            select.gameObject.SetActive(!select.gameObject.activeSelf);
        }

        UI.transform.Find("Health").GetComponent<TextMeshProUGUI>().text = "Health (" + Health + "/100)";
        UI.transform.Find("Mana").GetComponent<TextMeshProUGUI>().text = "Mana (" + Mana + "/100)";

        if (Health == 0)
        {
            UI.transform.Find("Dead").gameObject.SetActive(true);
        }
    }

    public void RemoveGold(int amount)
    {
        Health -= 100 / Piles;
    }

    // Update is called once per frame
    void Update()
    {
        if (turn && !turnRoutineRunning && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(UpdateTime());
        }

        UpdateUI();
        //UpdateMana();
        //UpdateHealth();
        //RoundOver();
    }
}
