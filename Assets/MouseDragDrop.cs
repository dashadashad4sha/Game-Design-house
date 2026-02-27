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
        mainCamera = Camera.main;  // основна€ камера (единственна€) будет в mainCamera
        // в силу единственности вынесли в переменную
        rb = GetComponent<Rigidbody>();
        placeableItem = GetComponent<PlaceableItem>();
    }

    void Update()
    {
        HandleTouch();
    }

    void HandleTouch()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        Debug.Log("More than 0");
        // ToDo добавить на два касани€ кручение сцены
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
        // луч от камеры до позиции касани€. надо думать как на двумерном экране
        // размещать предметы трехмерно
        Ray ray = mainCamera.ScreenPointToRay(touch.position);
        RaycastHit hit;  // структура дл€ инфы о столкновении ray и того, куда он попал

        // тут out уже который раз встречаетс€ это аналог передачи 
        // аргумента по ссылке как & в C++, но мы ожидаем, что во
        // врем€ исполнени€ метода переменна€ заполнитс€
        if (Physics.Raycast(ray, out hit))  // проверка попали ли в какой-то объект
        {
            if (hit.transform == transform) // если попали именно в скрипта айтем
            {
                isDragging = true;
                // чтоб при неверном размещении объект отправл€лс€ в начало
                startPosition = transform.position;
            }
        }
    }

    void Drag(Touch touch)
    {
        Debug.Log("Drag");
        // это »»шный код чтоб преобразовать 2д в 3д посмотрим как будет работать
        // ToDo переделать, придумать нормальную логику

        // PS работает пр€м ужасно
        Ray ray = mainCamera.ScreenPointToRay(touch.position);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            rb.MovePosition(new Vector3(point.x, transform.position.y, point.z));
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

}