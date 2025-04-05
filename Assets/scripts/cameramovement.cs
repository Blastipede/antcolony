using UnityEngine;
using UnityEngine.UI;
public class cameramovement : MonoBehaviour
{
    public float moveSpeed = 5f;                  // Speed multiplier for movement
    public float zoomSpeed = 5f;                  // Zoom sensitivity
    public float minZoom = 2f;                    // Min orthographic size
    public float maxZoom = 20f;                   // Max orthographic size

    void Update()
    {
        HandleCameraMoveTowardsMouse();
        HandleZoom();
    }

    void HandleCameraMoveTowardsMouse()
    {
        if (Input.GetMouseButton(0)) // Left mouse button
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 mousePos = Input.mousePosition;
            Vector2 direction = (mousePos - screenCenter).normalized;

            float distance = Vector2.Distance(mousePos, screenCenter);
            float scaledSpeed = moveSpeed * (distance / Screen.height); // Scale with distance from center

            Vector3 move = new Vector3(direction.x, direction.y, 0) * scaledSpeed * Time.deltaTime;
            transform.position += move;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            Camera.main.orthographicSize -= scroll * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
        }
    }
}