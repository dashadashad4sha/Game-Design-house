using UnityEngine;

public class AutoDragStarter : MonoBehaviour
{
    private Camera mainCamera;
    private DragDropItemMouse dragComponent;
    private bool initialized = false;

    public void Initialize(Camera camera)
    {
        mainCamera = camera;
        initialized = true;

        // Добавляем компонент перетаскивания, если его нет
        dragComponent = GetComponent<DragDropItemMouse>();
        if (dragComponent == null)
        {
            dragComponent = gameObject.AddComponent<DragDropItemMouse>();
        }

        // Даем Unity один кадр на инициализацию всех компонентов,
        // затем активируем перетаскивание
        Invoke("StartDragging", 0.1f);
    }

    void StartDragging()
    {
        if (dragComponent != null && initialized)
        {
            // Используем рефлексию для доступа к приватным полям
            // (это временное решение, лучше изменить DragDropItemMouse)
            var field = typeof(DragDropItemMouse).GetField("isDragging",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                field.SetValue(dragComponent, true);

                // Также устанавливаем startPosition
                var startPosField = typeof(DragDropItemMouse).GetField("startPosition",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (startPosField != null)
                {
                    startPosField.SetValue(dragComponent, transform.position);
                }
            }
        }
    }
}