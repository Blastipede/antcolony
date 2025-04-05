using UnityEngine;

public class burrowscript : MonoBehaviour
{
    public GameObject finalBurrow; // Assign in Inspector
    public float requiredProximity = 0.3f; // Very close
    public float timeToConvert = 5f;

    private float timer = 0f;
    private bool conversionStarted = false;

    void Update()
    {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, requiredProximity);
        int antCount = 0;

        foreach (Collider2D col in nearby)
        {
            if (col.CompareTag("Ant"))
            {
                antCount++;
            }
        }

        if (antCount >= 2)
        {
            if (!conversionStarted)
            {
                conversionStarted = true;
                timer = timeToConvert;
            }
            else
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    Instantiate(finalBurrow, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            conversionStarted = false;
            timer = 0f;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, requiredProximity);
    }
}