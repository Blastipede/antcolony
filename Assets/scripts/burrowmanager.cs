using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class burrowmanager : MonoBehaviour
{
    public float burrowTime = 5f;
    public GameObject burrowedPrefab;

    private bool isBurrowing = false;

    void Update()
    {
        if (!isBurrowing)
        {
            GameObject burrow = GameObject.FindGameObjectWithTag("antburrow");
            if (burrow != null)
            {
                StartCoroutine(HandleBurrowing(burrow));
            }
        }
    }

    IEnumerator HandleBurrowing(GameObject burrow)
    {
        isBurrowing = true;

        GameObject[] ants = GameObject.FindGameObjectsWithTag("ant");

        if (ants.Length < 2)
        {
            Debug.LogWarning("Not enough ants in the scene!");
            yield break;
        }

        // Find the 2 closest ants to the burrow
        GameObject[] closestAnts = ants
            .OrderBy(a => Vector3.Distance(a.transform.position, burrow.transform.position))
            .Take(2)
            .ToArray();

        GameObject ant1 = closestAnts[0];
        GameObject ant2 = closestAnts[1];

        Vector3 center = burrow.transform.position;
        float angle1 = 0f;
        float angle2 = 180f; // opposite start
        float radius = 0.5f;
        float speed = 120f;

        float timer = 0f;

        while (timer < burrowTime)
        {
            timer += Time.deltaTime;

            angle1 += speed * Time.deltaTime;
            angle2 += speed * Time.deltaTime;

            Vector3 offset1 = new Vector3(Mathf.Cos(angle1 * Mathf.Deg2Rad), Mathf.Sin(angle1 * Mathf.Deg2Rad), 0f) * radius;
            Vector3 offset2 = new Vector3(Mathf.Cos(angle2 * Mathf.Deg2Rad), Mathf.Sin(angle2 * Mathf.Deg2Rad), 0f) * radius;

            ant1.transform.position = center + offset1;
            ant2.transform.position = center + offset2;

            yield return null;
        }

        // Replace the burrow with the burrowed version
        Vector3 spawnPos = burrow.transform.position;
        Destroy(burrow);
        Instantiate(burrowedPrefab, spawnPos, Quaternion.identity);

        isBurrowing = false;
    }
}