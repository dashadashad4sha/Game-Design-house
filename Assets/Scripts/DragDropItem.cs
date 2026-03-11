using UnityEngine;


public class DragDropItem : MonoBehaviour
{
    Camera mainCamera;
    Rigidbody rb;
    PlaceableItem placeableItem;

    Vector3 startPosition;
    bool isDragging;

    // Vector3 touchOffset = new Vector2(20, 30);
    // оставляю нулевой для отладки пока
    Vector3 touchOffset = new Vector2(0, 0);

    void Start()
    {
        mainCamera = Camera.main;  // основная камера (единственная) будет в mainCamera
        // в силу единственности вынесли в переменную
        rb = GetComponent<Rigidbody>();
        placeableItem = GetComponent<PlaceableItem>();
    }

    void Update()
    {
        HandleMouse();
    }

    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
            MouseTouch();

        if (isDragging && Input.GetMouseButton(0))
            MouseDrag();

        if (isDragging && Input.GetMouseButtonUp(0))
            MouseDrop();
        
    }

    void MouseDrag()
    {
        int standLayer = LayerMask.NameToLayer("Stand");
        int placesToStand = 1 << standLayer;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition + touchOffset);

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, placesToStand))
        {
            // тут я недопонимаю как получить высоту без этой хуйни
            // ToDo исправить использовав placeableItem
            Collider itemObject = GetComponent<Collider>();
            placeableItem.onPlacementPlane = true;
            // переменная для высоты объекта чтоб ровно ставить на Stand-object не вдавливать
            float objectHeight = itemObject.bounds.extents.y;

            // в point в аргументах x и z зависят от пересечения, а у еще от высоты
            Vector3 point = hit.point + Vector3.up * objectHeight;

            // в point в аргументах x и z зависят от пересечения, а у от ***
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
                mainCamera.transform.position + mainCamera.transform.forward * 30f);

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
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, itemLayerMask))
        {
            isDragging = true;
            startPosition = transform.position;
        }
    }

    void MouseDrop()
    {
        
        isDragging = false;
        Debug.Log("Закончили перетаскивание мышью");

        if (!placeableItem.CanBePlaced())
        {
            rb.MovePosition(startPosition);
        }
        
    }

    bool CanMoveTo(Vector3 targetPosition)
    {
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