using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] bool isPath;
    [SerializeField] int price;
    [SerializeField] TMP_Text text;
    bool active = true;
    GameManager gameManager;
    Button button;
    Image image;
    [SerializeField] GameObject tower;
    [SerializeField] int direction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        if (tower != null)
        {
            SpriteRenderer towerSprite = tower.GetComponent<SpriteRenderer>();
            image.sprite = towerSprite.sprite;
            image.color = new Color(towerSprite.color.r, towerSprite.color.g, towerSprite.color.b);
        }
        if (text != null)
            text.text = price.ToString();
        CheckActive();
        gameManager.Reset += new GameManager.ResetButtons(CheckActive);
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
        if (text != null)
            text.color = new Color(127, 205, 98);
        if (isPath)
            image.color = Color.white;
    }

    void SetInactive()
    {
        active = false;
        button.enabled = false;
        if (price > gameManager.memory && text != null)
            text.color = Color.red;
        if (isPath)
            image.color = Color.red;
    }

    bool CheckPath()
    {
        return gameManager.checkLegality(direction);
    }
}
