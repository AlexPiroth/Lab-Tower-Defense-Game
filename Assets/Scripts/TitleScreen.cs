using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public void Load(bool cont)
    {
        if (cont)
            GameManager.continuing = true;
        SceneManager.LoadScene("SampleScene");
    }
}
