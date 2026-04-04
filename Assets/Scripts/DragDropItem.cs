using UnityEngine;


public class DragDropItem : MonoBehaviour
{
    Camera mainCamera;
    Rigidbody rb;
    PlaceableItem placeableItem;

    Vector3 startPosition;
    static DragDropItem currentDragging;

    // Vector3 touchOffset = new Vector2(20, 30);
    // �������� ������� ��� ������� ����
    Vector3 touchOffset = new Vector2(0, 0);

    void Start()
    {
        mainCamera = Camera.main;  // �������� ������ (������������) ����� � mainCamera
        // � ���� �������������� ������� � ����������
        rb = GetComponent<Rigidbody>();
        placeableItem = GetComponent<PlaceableItem>();
        startPosition = transform.position;
    }

    void Update()
    {
        HandleMouse();
    }

    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
            MouseTouch();

        if (currentDragging == this)
        {
            if (Input.GetMouseButton(0))
                MouseDrag();

            if (Input.GetMouseButtonUp(0))
                MouseDrop();
        }

    }

    void MouseDrag()
    {
        int standLayer = LayerMask.NameToLayer("Stand");
        int placesToStand = 1 << standLayer;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition + touchOffset);

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, 300f, placesToStand))
        {
            // ��� � ����������� ��� �������� ������ ��� ���� �����
            // ToDo ��������� ����������� placeableItem
            Collider itemObject = GetComponent<Collider>();
            placeableItem.onPlacementPlane = true;
            // ���������� ��� ������ ������� ���� ����� ������� �� Stand-object �� ����������
            float objectHeight = itemObject.bounds.extents.y;

            // � point � ���������� x � z ������� �� �����������, � � ��� �� ������
            Vector3 point = hit.point + Vector3.up * objectHeight;

            // � point � ���������� x � z ������� �� �����������, � � �� ***
            Vector3 targetPosition = new Vector3(point.x, point.y, point.z);

            if (CanMoveTo(targetPosition))
            {
                rb.MovePosition(targetPosition);
            }
        }
        else
        {
            placeableItem.onPlacementPlane = false;
            Plane plane = new(-mainCamera.transform.forward,
                mainCamera.transform.position + mainCamera.transform.forward * 50f);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);

                Vector3 targetPosition = new Vector3(point.x, point.y, point.z);

                if (CanMoveTo(targetPosition))
                {
                    rb.MovePosition(targetPosition);
                }
            }
        }
    }

    void MouseTouch()
    {
        int itemLayerMask = 1 << LayerMask.NameToLayer("Item");

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);
        if (Physics.Raycast(ray, out RaycastHit hit, 300f, itemLayerMask))
        {
            if (hit.collider.gameObject == gameObject)
            {
                currentDragging = this;
                if (placeableItem.isRightPlaced)
                    startPosition = transform.position;
            }
        }
    }

    void MouseDrop()
    {
        if (currentDragging == this && !placeableItem.CanBePlaced())
        {
            rb.MovePosition(startPosition);
        }
        else
        {
            placeableItem.isRightPlaced = true;
        }
        currentDragging = null;

    }

    bool CanMoveTo(Vector3 targetPosition)
    {
        return true;
        Collider col = GetComponent<Collider>();

        Vector3 center = targetPosition + col.bounds.center - transform.position;
        Vector3 halfExtents = col.bounds.extents;

        int obstacleMask = 1 << LayerMask.NameToLayer("Obstacle");

        return !Physics.CheckBox(
            center,
            halfExtents,
            transform.rotation,
            obstacleMask
        );
    }

}