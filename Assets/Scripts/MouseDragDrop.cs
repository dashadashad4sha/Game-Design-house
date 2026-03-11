using UnityEngine;


public class DragDropItemMouse : MonoBehaviour
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
        HandleTouch();

        // ВРЕМЕННАЯ ОТЛАДКА
        if (isDragging)
        {
            Debug.Log($"🟢 Dragging... isDragging = {isDragging}");
        }
        else
        {
            Debug.Log($"🔴 Not dragging");
        }
    }

    void HandleTouch()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        Debug.Log("More than 0");
        // ToDo добавить на два касания кручение сцены
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
            TryStartDrag(touch);

        if (isDragging && touch.phase == TouchPhase.Moved)
            Drag(touch);

        if (isDragging &&
            (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            EndDrag();
    }

    void TryStartDrag(Touch touch)
    {
        Debug.Log("TryStartDrag");

        Ray ray = mainCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log($"Луч попал в: {hit.transform.name}");

            if (hit.transform == transform)
            {
                Debug.Log("✅ ЭТО НАШ ПРЕДМЕТ!");
                isDragging = true;
                startPosition = transform.position;
            }
            else
            {
                Debug.Log($"❌ Это другой объект: {hit.transform.name}");
            }
        }
        else
        {
            Debug.Log("Луч никуда не попал");
        }
    }

    void Drag(Touch touch)
    {
        Debug.Log("Drag");

        Ray ray = mainCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;

        // Пытаемся попасть в пол или другую поверхность
        if (Physics.Raycast(ray, out hit))
        {
            // Двигаем предмет в точку попадания, но сохраняем его высоту
            Vector3 newPos = hit.point;
            newPos.y = transform.position.y; // сохраняем текущую высоту
            rb.MovePosition(newPos);
        }
    }

    void EndDrag()
    {
        Debug.Log("EndDrag");
        isDragging = false;

        if (!placeableItem.CanBePlaced())
        {
            rb.MovePosition(startPosition);
        }
    }


    // Эти методы будут вызываться из скрипта ItemBox
    // чтобы начать перетаскивание сразу после появления предмета
    public void ForceStartDrag()
    {
        isDragging = true;
        startPosition = transform.position;
        Debug.Log("ForceStartDrag вызван для " + gameObject.name);
    }

    public void ForceEndDrag()
    {
        if (isDragging)
        {
            isDragging = false;
            if (!placeableItem.CanBePlaced())
            {
                rb.MovePosition(startPosition);
            }
            Debug.Log("ForceEndDrag вызван для " + gameObject.name);
        }
    }

}