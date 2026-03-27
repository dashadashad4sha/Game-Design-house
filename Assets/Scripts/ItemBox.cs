using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [Header("Настройки коробки")]
    [SerializeField] private GameObject[] itemPrefabs;  // массив префабов предметов
    [SerializeField] private Transform spawnPoint;      // точка появления предмета
    [SerializeField] private float spawnOffset = 1.5f;  // смещение от коробки

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        Debug.Log($"Камера найдена: {mainCamera != null}");

        // Если точка появления не задана, создаем её автоматически
        if (spawnPoint == null)
        {
            GameObject spawnObj = new GameObject("SpawnPoint");
            spawnPoint = spawnObj.transform;
            spawnPoint.parent = transform;

            // Вычисляем направление "вперед" от коробки
            Vector3 forwardDirection = transform.forward;
            if (forwardDirection == Vector3.zero)
            {
                forwardDirection = Vector3.forward; // запасной вариант
            }

            // Ставим точку перед коробкой с учетом ее поворота
            spawnPoint.localPosition = new Vector3(0, 1f, spawnOffset);

            Debug.Log($"Точка спавна создана: локально (0, 1, {spawnOffset}), глобально: {spawnPoint.position}");
        }

        // Проверяем тег коробки
        Debug.Log($"Коробка имеет тег: {gameObject.tag}");
        Debug.Log($"Позиция коробки: {transform.position}");
    }

    void Update()
    {
        Debug.Log("Update вызван! Время: " + Time.time);

        if (Input.touchCount > 0)
        {
            Debug.Log($"Есть касание! touchCount = {Input.touchCount}");
            HandleTouch();
        }

        // Для теста в редакторе - по клику мыши
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log($"Клик мыши в позиции: {Input.mousePosition}");
            HandleMouseClick();
        }
    }

    void HandleMouseClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log($"Мышь попала в: {hit.transform.name}, тег: {hit.transform.tag}");

            if (hit.transform.CompareTag("Box"))
            {
                Debug.Log("🎯 КЛИК ПО КОРОБКЕ (мышь)!");
                SpawnItem();
            }
        }
        else
        {
            Debug.Log("Мышь не попала ни в какой объект");
        }
    }

    void HandleTouch()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            // Создаем луч от камеры до точки касания
            Ray ray = mainCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            // Рисуем луч в Scene view для отладки
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

            Debug.Log($"Луч создан. Позиция касания: {touch.position}");

            // Проверяем попадание в любой объект
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log($"Луч попал в: {hit.transform.name}, тег: {hit.transform.tag}");

                // Проверяем по тегу "Box"
                if (hit.transform.CompareTag("Box"))
                {
                    Debug.Log("🎯 ПОПАЛ В КОРОБКУ ПО ТЕГУ!");
                    SpawnItem();
                }
            }
            else
            {
                Debug.Log("Луч никуда не попал");
            }
        }
    }

    void SpawnItem()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            Debug.LogWarning("Нет префабов!");
            return;
        }

        int randomIndex = Random.Range(0, itemPrefabs.Length);
        GameObject newItem = Instantiate(itemPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);

        Debug.Log($"Предмет создан: {newItem.name}");

        // Ищем компонент для перетаскивания
        DragDropItemMouse drag = newItem.GetComponent<DragDropItemMouse>();

        if (drag != null)
        {
            Debug.Log("DragDropItemMouse найден, вызываем ForceStartDrag");
            drag.ForceStartDrag();
        }
        else
        {
            Debug.LogError("DragDropItemMouse не найден!");
        }
    }

    // Визуализация точки появления ВСЕГДА (не только при выделении)
    void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            // Рисуем сферу
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(spawnPoint.position, 0.5f);

            // Рисуем линию от коробки до точки спавна
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, spawnPoint.position);

            // Пишем координаты (только в редакторе)
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(spawnPoint.position + Vector3.up,
                $"Spawn: {spawnPoint.position}");
#endif
        }
        else
        {
            // Если точки нет, показываем где она будет
            Gizmos.color = Color.red;
            Vector3 estimatedPos = transform.position + transform.forward * spawnOffset + Vector3.up;
            Gizmos.DrawWireSphere(estimatedPos, 0.5f);
        }
    }

    // Вызывается в редакторе при изменении значений
    void OnValidate()
    {
        // Если точки нет, создаем её для визуализации
        if (spawnPoint == null && Application.isPlaying == false)
        {
            Transform existingSpawn = transform.Find("SpawnPoint");
            if (existingSpawn != null)
            {
                spawnPoint = existingSpawn;
            }
            else
            {
                GameObject spawnObj = new GameObject("SpawnPoint");
                spawnObj.transform.parent = transform;
                spawnObj.transform.localPosition = new Vector3(0, 1f, spawnOffset);
                spawnPoint = spawnObj.transform;
            }
        }
    }
}