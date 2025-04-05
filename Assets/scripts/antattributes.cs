using UnityEngine;
using TMPro;
[RequireComponent(typeof(Rigidbody2D))]

public class antattributes : MonoBehaviour
{
    [Header("Ant Attributes")]
    public int strength;
    public int looks;
    public string gender = "Male"; // "Male" or "Female"
    public string mood;
    public string antName;

    [Header("Hunger Settings")]
    [SerializeField] public float hunger = 0f;
    public float hungerIncreaseRate = 2f;
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
    public TextMeshProUGUI nameText;

    [Header("Mating Settings")]
    public Transform finalBurrow;           // Final burrow Transform found by tag "finalburrow"
    public GameObject heartEffectPrefab;    // Visual effect instantiated on arrival
    public GameObject[] babyAntPrefabs;       // Array of 4 baby ant prefabs
    public bool isMating = false;
    public bool reachedBurrow = false;
    public antattributes mate = null;
    public static bool burrowInUse = false; // Ensures only one mating pair uses the burrow at once

    private float moveSpeed;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private float directionTimer;
    private GameObject targetSugar;

    // 🟡 Regular burrow roaming behavior
    private GameObject nearestBurrow = null;
    public float roamRadius = 1f;
    private Vector2 roamTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        RandomizeAttributes();
        // Boost low-end speed relative to strength.
        moveSpeed = baseSpeed * (0.4f + 0.6f * (strength / 100f));
        UpdateMood();
        PickNewDirection();
        UpdateNameUI();

        // Find the final burrow by tag if not assigned.
        if (finalBurrow == null)
        {
            GameObject fb = GameObject.FindGameObjectWithTag("finalburrow");
            if (fb != null)
            {
                finalBurrow = fb.transform;
            }
        }
    }

    void Update()
    {
        directionTimer -= Time.deltaTime;
        IncreaseHungerOverTime();
        UpdateMood();
        UpdateHungerUI();
        DetectSugarNearby();

        // --- Mating Behavior ---
        if (finalBurrow != null && !burrowInUse)
        {
            // Female ants find the best male in range.
            if (gender == "Female" && !isMating)
            {
                Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, attractionDistance);
                antattributes bestMale = null;
                float bestLooks = -1f;
                foreach (Collider2D col in nearby)
                {
                    if (col.gameObject == this.gameObject) continue;
                    antattributes other = col.GetComponent<antattributes>();
                    if (other != null && other.gender == "Male" && !other.isMating)
                    {
                        if (other.looks > bestLooks)
                        {
                            bestLooks = other.looks;
                            bestMale = other;
                        }
                    }
                }
                if (bestMale != null)
                {
                    // Begin mating: assign each other as mates and mark the burrow as in use.
                    isMating = true;
                    mate = bestMale;
                    bestMale.isMating = true;
                    bestMale.mate = this;
                    burrowInUse = true;
                }
            }
        }
        // --- End Mating Behavior ---

        // --- Regular Behavior (Roaming, Sugar, etc.) ---
        if (!isMating)
        {
            if (nearestBurrow == null)
            {
                GameObject[] burrows = GameObject.FindGameObjectsWithTag("placedburrow");
                if (burrows.Length > 0)
                {
                    float minDist = Mathf.Infinity;
                    foreach (GameObject b in burrows)
                    {
                        float d = Vector2.Distance(transform.position, b.transform.position);
                        if (d < minDist)
                        {
                            minDist = d;
                            nearestBurrow = b;
                        }
                    }
                    if (nearestBurrow != null)
                    {
                        roamTarget = GetRandomRoamPoint();
                    }
                }
            }

            if (nearestBurrow != null && directionTimer <= 0f && targetSugar == null)
            {
                roamTarget = GetRandomRoamPoint();
                directionTimer = Random.Range(directionChangeInterval * 0.5f, directionChangeInterval * 1.5f);
            }
        }
        // --- End Regular Behavior ---
    }

    void FixedUpdate()
    {
        // --- Mating Movement ---
        if (isMating && finalBurrow != null)
        {
            // Both ants move toward the final burrow.
            Vector2 dir = ((Vector2)finalBurrow.position - rb.position).normalized;
            rb.velocity = dir * moveSpeed;

            // Check if this ant has reached the burrow.
            if (Vector2.Distance(transform.position, finalBurrow.position) < 0.2f)
            {
                reachedBurrow = true;
                rb.velocity = Vector2.zero;
            }

            // If both mates have reached, instantiate the heart effect and baby ants.
            if (mate != null && reachedBurrow && mate.reachedBurrow)
            {
                if (heartEffectPrefab != null)
                {
                    GameObject fx = Instantiate(heartEffectPrefab, finalBurrow.position, Quaternion.identity);
                    Destroy(fx, 2f);
                }

                if (babyAntPrefabs != null)
                {
                    foreach (GameObject baby in babyAntPrefabs)
                    {
                        Vector3 spawnPos = finalBurrow.position + (Vector3)Random.insideUnitCircle * 0.3f;
                        Instantiate(baby, spawnPos, Quaternion.identity);
                    }
                }

                // Reset mating state for both ants.
                isMating = false;
                reachedBurrow = false;
                mate.isMating = false;
                mate.reachedBurrow = false;
                mate.mate = null;
                mate = null;
                burrowInUse = false;
            }
            return; // Skip normal movement while mating.
        }

        // --- Regular Movement ---
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
                looks += Random.Range(1, 4); // Increase looks when sugar is eaten
                targetSugar = null;
                PickNewDirection();
            }
        }
        else if (nearestBurrow != null)
        {
            Vector2 toRoamTarget = (roamTarget - rb.position);
            if (toRoamTarget.magnitude < 0.2f)
            {
                roamTarget = GetRandomRoamPoint();
            }
            finalMoveDir = toRoamTarget.normalized;
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
        hunger = 0f;

        string[] goofyNames = new string[]
        {
            "Crumbthief", "Legz McGee", "Sir Scurriersworth", "Wigglebum", "Dirt Snacker",
            "Chonk Mandible", "Buzzfoot", "Skitterman", "Toastercrumb", "Ziggy Zoom",
            "Grainzilla", "Nibbles Jr.", "Captain Crawly", "Antony Stark", "Bitey McByteface",
            "Cheezlegs", "Gobbles", "Lord Tunneltrot", "Peanut Wiggler", "Sir Crunch",
            "blastipede", "husky", "charvi"
        };

        antName = goofyNames[Random.Range(0, goofyNames.Length)];
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

    void UpdateNameUI()
    {
        if (nameText != null)
        {
            nameText.text = "Name: " + antName;
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

    Vector2 GetRandomRoamPoint()
    {
        if (nearestBurrow == null) return transform.position;

        Vector2 center = nearestBurrow.transform.position;
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Random.Range(0.2f, roamRadius);
        return center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
    }

    void IncreaseHungerOverTime()
    {
        hunger += hungerIncreaseRate * Time.deltaTime;
        hunger = Mathf.Clamp(hunger, 0f, maxHunger);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attractionDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);

        if (nearestBurrow != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(nearestBurrow.transform.position, roamRadius);
        }
    }
}