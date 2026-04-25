using UnityEngine;

public class Shop : MonoBehaviour
{
    readonly float RAISE = 0.32f;
    bool raised = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RaiseLowerShop()
    {
        if (raised)
            transform.position -= new Vector3(0, Screen.height * RAISE);
        else
        {
            transform.position += new Vector3(0, Screen.height * RAISE);
        }
        raised = !raised;
    }


}
