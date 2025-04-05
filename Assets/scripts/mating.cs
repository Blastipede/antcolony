using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mating : MonoBehaviour
{
    public enum Gender { Male, Female }
    public Gender gender;

    public float detectionRadius = 2f;
    public GameObject babyPrefab; // Assign in Inspector
    public float babySpawnOffset = 0.2f;

    private bool isGoingToBurrow = false;
    private bool onCooldown = false;
    private GameObject partnerAnt;
    private Transform targetBurrow;

    private static List<Transform> burrows;
    private static bool isBurrowOccupied = false;

    void Start()
    {
        if (burrows == null)
        {
            burrows = new List<Transform>();
            GameObject[] burrowObjs = GameObject.FindGameObjectsWithTag("finalburrow");
            foreach (GameObject b in burrowObjs)
            {
                burrows.Add(b.transform);
            }
        }
    }

    void Update()
    {
        if (!isGoingToBurrow && !onCooldown)
        {
            CheckForPartner();
        }
        else if (targetBurrow != null)
        {
            float step = 2f * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetBurrow.position, step);

            if (partnerAnt != null &&
                Vector2.Distance(transform.position, targetBurrow.position) < 0.1f &&
                Vector2.Distance(partnerAnt.transform.position, targetBurrow.position) < 0.1f)
            {
                // Only the female handles spawning
                if (this.gender == Gender.Female)
                {
                    StartCoroutine(HandleMating());
                }

                isBurrowOccupied = false;
                isGoingToBurrow = false;
            }
        }
    }

    void CheckForPartner()
    {
        if (burrows == null || burrows.Count == 0) return; // ❌ No burrows, nothing happens

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue;

            mating other = hit.GetComponent<mating>();
            if (other != null && !other.isGoingToBurrow && !other.onCooldown && other.gender != this.gender)
            {
                if (Random.value <= 0.5f && !isBurrowOccupied)
                {
                    Transform nearest = FindNearestBurrow();
                    if (nearest == null) return; // ❌ No available burrow, do nothing

                    isGoingToBurrow = true;
                    other.isGoingToBurrow = true;

                    partnerAnt = other.gameObject;
                    other.partnerAnt = this.gameObject;

                    targetBurrow = nearest;
                    other.targetBurrow = nearest;

                    isBurrowOccupied = true;
                    break;
                }
            }
        }
    }

    Transform FindNearestBurrow()
    {
        Transform closest = null;
        float shortestDist = Mathf.Infinity;
        Vector2 currentPos = transform.position;

        foreach (Transform burrow in burrows)
        {
            float dist = Vector2.Distance(currentPos, burrow.position);
            if (dist < shortestDist)
            {
                shortestDist = dist;
                closest = burrow;
            }
        }

        return closest;
    }

    IEnumerator HandleMating()
    {
        // Disable ant attribute components
        antattributes myAttr = GetComponent<antattributes>();
        antattributes partnerAttr = partnerAnt.GetComponent<antattributes>();

        if (myAttr != null) myAttr.enabled = false;
        if (partnerAttr != null) partnerAttr.enabled = false;

        // Spawn 4 babies
        for (int i = 0; i < 4; i++)
        {
            Vector2 spawnPos = (Vector2)targetBurrow.position + Random.insideUnitCircle * babySpawnOffset;
            Instantiate(babyPrefab, spawnPos, Quaternion.identity);
        }

        yield return new WaitForSeconds(2f);

        if (myAttr != null) myAttr.enabled = true;
        if (partnerAttr != null) partnerAttr.enabled = true;

        // Start cooldown
        onCooldown = true;
        mating partnerScript = partnerAnt.GetComponent<mating>();
        if (partnerScript != null) partnerScript.StartCoroutine(partnerScript.Cooldown());

        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(15f);
        onCooldown = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
