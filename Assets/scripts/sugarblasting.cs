using UnityEngine;
using System.Collections;

public class sugarblasting : MonoBehaviour
{
    [Header("Sugar Blast Settings")]
    public GameObject sugarPrefab;
    public float blastForce = 10f;
    public float blastInterval = 3f; // Time between blasts

    private float blastTimer;

    void Update()
    {
        blastTimer -= Time.deltaTime;

        if (blastTimer <= 0f)
        {
            BlastSugar();
            blastTimer = blastInterval;
        }
    }

    void BlastSugar()
    {
        GameObject sugar = Instantiate(sugarPrefab, transform.position, Quaternion.identity);

        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        Rigidbody2D rb = sugar.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.AddForce(randomDirection * blastForce, ForceMode2D.Impulse);

            StartCoroutine(DisableRigidbodyAfterTime(rb, 1f)); // now disables after 1 second
        }
    }

    IEnumerator DisableRigidbodyAfterTime(Rigidbody2D rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rb != null)
        {
            rb.simulated = false;
        }
    }
}