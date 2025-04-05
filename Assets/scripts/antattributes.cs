using UnityEngine;
using TMPro;
[RequireComponent(typeof(Rigidbody2D))]

public class antattributes : MonoBehaviour
{
    [Header("Ant Attributes")]
    public int strength;
    public int looks;
    public string gender = "Male"; // Set manually
    public string mood;

    [Header("Hunger Settings")]
    [SerializeField] public float hunger = 0f; // Starts at 0
    public float hungerIncreaseRate = 2f; // Units per second
    private const float maxHunger = 100f;

    [Header("Movement Settings")]
    public float baseSpeed = 2f;
    public float rotationSpeed = 5f;
    public float directionChangeInterval = 2f;

    [Header("Avoidance Settings")]
    public float avoidanceRadius = 1.5f;
    public float avoidanceForce = 1.5f;

    [Header("Sugar Settings")]
    public float attractionDistance = 5f;
    public float consumeDistance = 0.5f;
    public float hungerReductionOnSugar = 30f;

    [Header("UI")]
    public TextMeshProUGUI hungerText;

    [Header("Death Settings")]
    public GameObject deadAntPrefab; // Assign in Inspector

    private float moveSpeed;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private float directionTimer;
    private GameObject targetSugar;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        RandomizeAttributes();
        moveSpeed = baseSpeed * (strength / 100f);
        UpdateMood();
        PickNewDirection();
    }

    void Update()
    {
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0f && targetSugar == null)
        {
            PickNewDirection();
            directionTimer = Random.Range(directionChangeInterval * 0.5f, directionChangeInterval * 1.5f);
        }

        IncreaseHungerOverTime();
        UpdateMood();
        UpdateHungerUI();
        DetectSugarNearby();

        // 🔥 Check if hunger is maxed
        if (hunger >= maxHunger)
        {
            Die();
        }
    }

    void FixedUpdate()
    {
        if (hunger >= maxHunger) return;

        Vector2 finalMoveDir;
        float currentSpeed = moveSpeed;

        if (targetSugar != null)
        {
            Vector2 toSugar = ((Vector2)targetSugar.transform.position - rb.position).normalized;
            finalMoveDir = toSugar;
            currentSpeed *= 1.5f;

            float dist = Vector2.Distance(rb.position, targetSugar.transform.position);
            if (dist < consumeDistance)
            {
                Destroy(targetSugar);
                hunger = Mathf.Max(0f, hunger - hungerReductionOnSugar);
                targetSugar = null;
                PickNewDirection();
            }
        }
        else
        {
            Vector2 avoidance = CalculateAvoidance();
            finalMoveDir = (moveDirection + avoidance).normalized;
        }

        rb.velocity = finalMoveDir * currentSpeed;

        if (finalMoveDir.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(finalMoveDir.y, finalMoveDir.x) * Mathf.Rad2Deg;
            rb.MoveRotation(Mathf.LerpAngle(rb.rotation, angle, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    void PickNewDirection()
    {
        float angle = Random.Range(0f, 360f);
        moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    Vector2 CalculateAvoidance()
    {
        Vector2 avoidance = Vector2.zero;
        Collider2D[] nearbyAnts = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius);

        foreach (Collider2D col in nearbyAnts)
        {
            if (col.gameObject != this.gameObject && col.CompareTag("Ant"))
            {
                Vector2 diff = (Vector2)transform.position - (Vector2)col.transform.position;
                float dist = diff.magnitude;

                if (dist > 0)
                {
                    avoidance += diff.normalized / dist;
                }
            }
        }

        return avoidance * avoidanceForce;
    }

    void RandomizeAttributes()
    {
        strength = Random.Range(1, 101);
        looks = Random.Range(1, 101);
        hunger = 0f; // Starts at 0
    }

    void IncreaseHungerOverTime()
    {
        hunger += hungerIncreaseRate * Time.deltaTime;
        hunger = Mathf.Clamp(hunger, 0f, maxHunger);
    }

    void UpdateMood()
    {
        if (hunger > 70f)
        {
            mood = "Sad";
        }
        else if (hunger >= 50f)
        {
            mood = "Indifferent";
        }
        else
        {
            mood = "Happy";
        }
    }

    void UpdateHungerUI()
    {
        if (hungerText != null)
        {
            hungerText.text = "Hunger: " + Mathf.RoundToInt(hunger).ToString();
        }
    }

    void DetectSugarNearby()
    {
        if (targetSugar != null) return;

        GameObject[] allSugar = GameObject.FindGameObjectsWithTag("Sugar");

        float closestDist = attractionDistance;
        GameObject closest = null;

        foreach (GameObject sugar in allSugar)
        {
            float dist = Vector2.Distance(transform.position, sugar.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = sugar;
            }
        }

        if (closest != null)
        {
            targetSugar = closest;
        }
    }

    void Die()
    {
        if (deadAntPrefab != null)
        {
            GameObject deadAnt = Instantiate(deadAntPrefab, transform.position, Quaternion.identity);
            Destroy(deadAnt, 30f); // Destroy after 30 seconds
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attractionDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);
    }
}