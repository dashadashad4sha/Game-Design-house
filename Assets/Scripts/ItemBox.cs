using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [Header("Настройки коробки")]
    [SerializeField] private GameObject[] itemPrefabs;  // массив префабов предметов
    [SerializeField] private Transform spawnPoint;      // точка появления предмета
    [SerializeField] private float spawnOffset = 0f;  // смещение от коробки

    private Camera mainCamera;
    private GameObject lastSpawnedObject;
    PlaceableItem itemOfLastSpawnedObject;
    int SpawnItemIndex = 0;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // для тачей потом сделаю

        if (Input.GetMouseButtonDown(0))
            HandleMouseClick();
    }

    void HandleMouseClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("Box"))
            {
                if (lastSpawnedObject == null || itemOfLastSpawnedObject.isRightPlaced)
                    SpawnItem();
            }

        }
    }

    void SpawnItem()
    {
        if (SpawnItemIndex >= itemPrefabs.Length)
        {
            Debug.LogWarning("Закончились предметы");
            return;
        }

        GameObject newItem = Instantiate(itemPrefabs[SpawnItemIndex], spawnPoint.position, Quaternion.identity);
        lastSpawnedObject = newItem;
        itemOfLastSpawnedObject = lastSpawnedObject.GetComponent<PlaceableItem>();
        SpawnItemIndex++;
    }
}