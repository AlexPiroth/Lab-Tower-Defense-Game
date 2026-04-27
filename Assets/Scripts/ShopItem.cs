using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] bool isPath;
    [SerializeField] int price;
    [SerializeField] int[] directions;
    bool active = false;
    GameManager gameManager;
    Button button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckActive()
    {
        if (price > gameManager.memory)
        {
            if (active)
                SetInactive();
        }
        else
        {
            if (isPath && !CheckPath())
            {
                if (active)
                    SetInactive();
            }
            else if (!active)
                SetActive();
        }


    }

    void SetActive()
    {
        active = true;
        button.enabled = true;
    }

    void SetInactive()
    {
        active = false;
        button.enabled = false;
    }

    bool CheckPath()
    {
        return true;
    }
}
