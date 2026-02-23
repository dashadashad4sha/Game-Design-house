using UnityEngine;

public class PlaceableItem : MonoBehaviour
{
    // ToDo сделать возможность представлять allowedPlacement
    // как список всех возможных разрешенных PlacementType
    public PlacementType allowedPlacement;
    public PlacementZone currentZone;

    // вызывается если попали в зону с включенным On Trigger,
    // и передает объект коллайдера в параметр other
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlacementZone zone))
        // пробуем получить свойство PlacementZone, и, если оно
        // существует, принимаем его значение в параметр zone
        {
            currentZone = zone;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlacementZone zone))
        {
            if (currentZone == zone)
                currentZone = null;
        }
    }

    // ToDo переделать под представление
    // allowedPlacement через список 
    public bool CanBePlaced()
    {
        return currentZone != null &&
               currentZone.placementType == allowedPlacement;
    }

    // для изменения внешнего вида объекта, когда
    // его нельзя поставить
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        // поменять на уменьшение непрозрачности
        if (CanBePlaced())
            rend.material.color = Color.green;
        else
            rend.material.color = Color.red;
    }
}