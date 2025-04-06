using UnityEngine;
using UnityEngine.SceneManagement;

public class mainemenu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("cutscene");
        }
    }
}