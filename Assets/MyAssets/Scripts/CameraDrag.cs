using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDrag : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private float zoomStep, minCamSize, maxCamSize;

    public float mapMinX, mapMaxX, mapMinY, mapMaxY;

    private Vector3 dragOrigin;
    private Touch touchZero, touchOne;

    void Update()
    {
        PanCamera();

        // Handle mouse scroll wheel input
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            if (scroll > 0)
            {
                ZoomIn();
            }
            else if (scroll < 0)
            {
                ZoomOut();
            }
        }

        // Handle touch input for zooming
        if (Input.touchCount == 2)
        {
            touchZero = Input.GetTouch(0);
            touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
            float initialZoom = cam.orthographicSize;

            float currentDistance = Vector2.Distance(touchZeroPrevPos, touchOnePrevPos);
            float zoomChange = currentDistance - initialDistance;

            if (zoomChange > 0)
            {
                ZoomOut();
            }
            else if (zoomChange < 0)
            {
                ZoomIn();
            }
        }
    }

    private void PanCamera()
    {
        if (Input.touchCount == 2)
            return;

        if (Input.GetMouseButtonDown(0))
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            cam.transform.position = ClampCamera(cam.transform.position + difference);
        }
    }

    public void ZoomIn()
    {
        float newSize = cam.orthographicSize - zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);

        cam.transform.position = ClampCamera(cam.transform.position);
    }

    public void ZoomOut()
    {
        float newSize = cam.orthographicSize + zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);

        cam.transform.position = ClampCamera(cam.transform.position);
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }

    public void SetXMaxValue(float val)
    {
        mapMaxX = val;
    }

    public void IncreaseYMaxValue()
    {
        mapMaxY = 35;
    }
}