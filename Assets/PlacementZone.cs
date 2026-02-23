using UnityEngine;

public class PlacementZone : MonoBehaviour
{
    public PlacementType placementType;
}

public enum PlacementType
{
    Floor,
    Table,
    Chair,
    Shelf
}