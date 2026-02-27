using UnityEngine;


public class DragDropItem : MonoBehaviour
{
    Camera mainCamera;
    Rigidbody rb;
    PlaceableItem placeableItem;

    Vector3 startPosition;
    bool isDragging;

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

    /*void MouseDrag()
    {
        

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            rb.MovePosition(new Vector3(point.x, transform.position.y, point.z));
            Debug.Log("Перетаскиваем мышью");
        }
    }*/

    void MouseDrag()
    {
        int standLayer = LayerMask.NameToLayer("Stand");
        int placesToStand = 1 << standLayer;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, placesToStand))
        {
            // тут я недопонимаю как получить высоту без этой хуйни
            // ToDo исправить использовав placeableItem
            Collider itemObject = GetComponent<Collider>();
            // переменная для высоты объекта чтоб ровно ставить на Stand-object не вдавливать
            float objectHeight = itemObject.bounds.extents.y;

            // в point в аргументах x и z зависят от пересечения, а у еще от высоты
            Vector3 point = hit.point + Vector3.up * objectHeight;

            // в point в аргументах x и z зависят от пересечения, а у от ***
            rb.MovePosition(point);
        }
        else 
        {
            Plane plane = new(-mainCamera.transform.forward, 
                mainCamera.transform.position + mainCamera.transform.forward * 30f);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);
                rb.MovePosition(new Vector3(point.x, point.y, point.z));
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

}