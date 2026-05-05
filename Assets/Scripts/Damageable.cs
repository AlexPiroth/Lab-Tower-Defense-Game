using TMPro;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int HP;
    [SerializeField] bool isHome;
    GameObject restart;
    TMP_Text HPText;
    GameManager gameManager;

    private void Start()
    {
        if (isHome)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            gameManager.home = this;
            if (gameManager.loadHP > 0)
                HP = gameManager.loadHP;
            HPText = GameObject.Find("HPText").GetComponent<TMP_Text>();
            HPText.text = "Remaining HP: " + HP;
            restart = GameObject.Find("Restart Prompt");
            restart.SetActive(false);
        }
    }
    public void LoseHP(int lose)
    {
        HP -= lose;
        if (isHome)
            HPText.text = "Remaining HP: " + HP;
        if (HP <= 0 )
        {
            if (isHome)
            {
                Time.timeScale = 0;
                restart.SetActive(true);
            }
            Destroy(this.gameObject);
        }    
    }
}
