using UnityEngine;
using TMPro;
public class stuffdisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI hungerText;
    public TextMeshProUGUI strengthText;
    public TextMeshProUGUI looksText;
    public TextMeshProUGUI moodText;
    public TextMeshProUGUI nameText;

    private antattributes currentAnt;

    void Start()
    {
        HideUI();
    }

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            antattributes ant = hit.collider.GetComponent<antattributes>();
            if (ant != null)
            {
                if (ant != currentAnt)
                {
                    currentAnt = ant;
                    UpdateUI(currentAnt);
                    ShowUI();
                }
                return;
            }
        }

        if (currentAnt != null)
        {
            currentAnt = null;
            HideUI();
        }
    }

    void UpdateUI(antattributes ant)
    {
        if (hungerText) hungerText.text = "Hunger: " + Mathf.RoundToInt(ant.hunger);
        if (strengthText) strengthText.text = "Strength: " + ant.strength;
        if (looksText) looksText.text = "Sweetness: " + ant.looks;
        if (moodText) moodText.text = "Mood: " + ant.mood;
        if (nameText) nameText.text = "Name: " + ant.antName;
    }

    void ShowUI()
    {
        if (hungerText) hungerText.gameObject.SetActive(true);
        if (strengthText) strengthText.gameObject.SetActive(true);
        if (looksText) looksText.gameObject.SetActive(true);
        if (moodText) moodText.gameObject.SetActive(true);
        if (nameText) nameText.gameObject.SetActive(true);
    }

    void HideUI()
    {
        if (hungerText) hungerText.gameObject.SetActive(false);
        if (strengthText) strengthText.gameObject.SetActive(false);
        if (looksText) looksText.gameObject.SetActive(false);
        if (moodText) moodText.gameObject.SetActive(false);
        if (nameText) nameText.gameObject.SetActive(false);
    }
}