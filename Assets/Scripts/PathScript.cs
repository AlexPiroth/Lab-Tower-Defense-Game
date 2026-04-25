using UnityEngine;

public class PathScript : MonoBehaviour
{
    [SerializeField] Sprite[] spawnPoint; //0 = up, 1 = down, 2 = left, 3 = right
    [SerializeField] Sprite straight, corner;
    int startDir;
    SpriteRenderer render;
    public void Spawn(int direction)
    {
        render = gameObject.GetComponent<SpriteRenderer>();
        render.sprite = spawnPoint[direction];
        switch (direction)
        {
            case 0:
                startDir = 1;
                break;

            case 1:
                startDir = 0;
                break;

            case 2:
                startDir = 3;
                break;

            case 3:
                startDir = 2;
                break;
        }
    }

    public void Extend(int direction)
    {
        // Straight paths
        if (startDir + direction == 1 || startDir + direction == 5) // Somehow this is actually the best way to do this
        {
            render.sprite = straight;
            if (startDir + direction == 1)
                transform.rotation *= Quaternion.Euler(0, 0, 90);
        }

        // Corner paths
        else
        {
            render.sprite = corner;
            // Do nothing for 0,2 and 2,0
            if ((startDir == 0 && direction == 3)||(startDir == 3 && direction == 0))
                transform.rotation *= Quaternion.Euler(0, 0, 270);
            else if ((startDir == 1 && direction == 3) || (startDir == 3 && direction == 1))
                transform.rotation *= Quaternion.Euler(0, 0, 180);
            else if ((startDir == 1 && direction == 2) || (startDir == 2 && direction == 1))
                transform.rotation *= Quaternion.Euler(0, 0, 90);
        }
    }
}
