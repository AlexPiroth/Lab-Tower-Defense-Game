using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] int HP;

    public void LoseHP(int lose)
    {
        HP -= lose;
        if (HP <= 0 )
        {
            Destroy(this.gameObject);
        }    
    }
}
