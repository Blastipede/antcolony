using UnityEngine;
using TMPro;
public class deadant : MonoBehaviour
{
    [Header("Tag of the hoverable object")]
    public string hoverTag = "Hoverable";

    [Header("Text to enable on hover")]
    public TextMeshProUGUI hoverText;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        if (hoverText != null)
        {
            hoverText.gameObject.SetActive(false); // Hide at start
        }
    }

    void Update()
    {
        if (cam == null || hoverText == null) return;

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag(hoverTag))
        {
            hoverText.gameObject.SetActive(true);
        }
        else
        {
            hoverText.gameObject.SetActive(false);
        }
    }
}
