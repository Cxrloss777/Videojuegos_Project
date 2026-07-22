using UnityEngine;

public class CursorLock : MonoBehaviour
{
    void Start()
    {
        LockCursor();
    }

    void Update()
    {
        // Permite liberar el cursor con Escape (útil para probar/depurar)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Volver a bloquear si se hace click dentro de la ventana del juego
        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            LockCursor();
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}