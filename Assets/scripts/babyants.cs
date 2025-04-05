using UnityEngine;

public class babyants : MonoBehaviour
{
    [Header("Follow Settings")]
    public float followSpeed = 2f;
    public float followDistance = 0.5f;

    [Header("Growth Settings")]
    public Vector3 adultSize = new Vector3(2f, 2f, 2f);
    public float growthSpeed = 0.5f;
    public GameObject antPrefab;

    private Transform targetAnt;
    private bool hasGrown = false;

    void Start()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // visible small start
        FindClosestAnt();
    }

    void Update()
    {
        if (hasGrown) return;

        if (targetAnt == null || !targetAnt.gameObject.activeInHierarchy)
        {
            FindClosestAnt();
            return;
        }

        FollowAnt();
        GrowAnt();
    }

    void FindClosestAnt()
    {
        GameObject[] ants = GameObject.FindGameObjectsWithTag("Ant");
        float closestDistance = Mathf.Infinity;
        Transform closestAnt = null;

        foreach (GameObject ant in ants)
        {
            if (!ant.activeInHierarchy) continue;

            float distance = Vector2.Distance(transform.position, ant.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestAnt = ant.transform;
            }
        }

        targetAnt = closestAnt;
    }

    void FollowAnt()
    {
        Vector2 direction = (targetAnt.position - transform.position).normalized;
        float distanceToAnt = Vector2.Distance(transform.position, targetAnt.position);

        if (distanceToAnt > followDistance)
        {
            transform.position += (Vector3)(direction * followSpeed * Time.deltaTime);

            // Rotate to face direction of movement (Z axis in 2D)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    void GrowAnt()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, adultSize, growthSpeed * Time.deltaTime);

        if (transform.localScale == adultSize)
        {
            hasGrown = true;

            if (antPrefab != null)
            {
                Instantiate(antPrefab, transform.position, transform.rotation);
            }

            gameObject.SetActive(false); // Disable after becoming adult
        }
    }
}