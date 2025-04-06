using UnityEngine;
using UnityEngine.SceneManagement;

public class cutscenechange : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Name of the scene to load")]
    public string sceneName;

    [Tooltip("Time in seconds before the scene changes")]
    public float delayInSeconds = 3f;

    void Start()
    {
        StartCoroutine(ChangeSceneAfterDelay());
    }

    private System.Collections.IEnumerator ChangeSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayInSeconds);
        SceneManager.LoadScene(sceneName);
    }
}