using UnityEngine;
using UnityEngine.UI;

public class snapposspawn : MonoBehaviour
{
    public Button spawnButton;
    public GameObject objectToSpawn;
    private GameObject currentObject;
    private bool isPlaced = false;
    private occupycheck currentSnapPoint = null;

    void Start()
    {
        spawnButton.onClick.AddListener(HandleSpawn);
    }

    void Update()
    {
        if (currentObject != null && !isPlaced)
        {
            SnapToClosestAvailablePoint();

            if (Input.GetMouseButtonDown(0)) // Left-click to place
            {
                if (currentSnapPoint != null && !currentSnapPoint.isOccupied)
                {
                    currentSnapPoint.isOccupied = true;

                    // 🟢 Change tag to "placedburrow"
                    currentObject.tag = "placedburrow";

                    isPlaced = true;
                    currentObject = null;
                    currentSnapPoint = null;
                }
            }

            if (Input.GetMouseButtonDown(1)) // Right-click to cancel
            {
                Destroy(currentObject);
                currentObject = null;
                currentSnapPoint = null;
                isPlaced = false;
            }
        }
    }

    void HandleSpawn()
    {
        if (currentObject != null && !isPlaced)
        {
            Destroy(currentObject);
        }

        isPlaced = false;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        currentObject = Instantiate(objectToSpawn, mouseWorld, Quaternion.identity);
    }

    void SnapToClosestAvailablePoint()
    {
        GameObject[] snapPoints = GameObject.FindGameObjectsWithTag("snappos");
        if (snapPoints.Length == 0) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        GameObject closest = null;
        float minDist = Mathf.Infinity;
        occupycheck bestSnapPoint = null;

        foreach (GameObject point in snapPoints)
        {
            occupycheck sp = point.GetComponent<occupycheck>();
            if (sp == null || sp.isOccupied) continue;

            float dist = Vector3.Distance(mouseWorld, point.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = point;
                bestSnapPoint = sp;
            }
        }

        if (closest != null && currentObject != null)
        {
            currentObject.transform.position = closest.transform.position;
            currentSnapPoint = bestSnapPoint;
        }
    }
}