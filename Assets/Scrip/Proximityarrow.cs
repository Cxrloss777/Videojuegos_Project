using UnityEngine;

/// <summary>
/// SCR_ProximityArrow - Hace que una flecha en el HUD (UI) apunte
/// hacia el fragmento de datos (coleccionable) más cercano al jugador.
/// Colocar en un GameObject del Canvas, o en cualquier GameObject de escena
/// y asignar la referencia de la flecha desde el Canvas.
/// </summary>
public class ProximityArrow : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("RectTransform de la imagen de la flecha en el Canvas (UI).")]
    public RectTransform flecha;
    public Transform jugador;
    public Camera camaraPrincipal;

    [Header("Configuración")]
    [Tooltip("Tag asignado a los coleccionables (ej. ITM_DataFragment).")]
    public string tagColeccionable = "Coleccionable";
    [Tooltip("Cada cuántos segundos se busca al coleccionable más cercano (optimización).")]
    public float intervaloBusqueda = 0.3f;

    private Transform objetivoMasCercano;
    private float temporizadorBusqueda = 0f;

    void Update()
    {
        if (jugador == null || camaraPrincipal == null || flecha == null) return;

        temporizadorBusqueda -= Time.deltaTime;
        if (temporizadorBusqueda <= 0f)
        {
            BuscarMasCercano();
            temporizadorBusqueda = intervaloBusqueda;
        }

        if (objetivoMasCercano == null)
        {
            flecha.gameObject.SetActive(false);
            return;
        }

        if (!flecha.gameObject.activeSelf) flecha.gameObject.SetActive(true);
        ApuntarFlecha();
    }

    void BuscarMasCercano()
    {
        GameObject[] coleccionables = GameObject.FindGameObjectsWithTag(tagColeccionable);
        float distanciaMinima = Mathf.Infinity;
        Transform masCercano = null;

        foreach (GameObject obj in coleccionables)
        {
            float distancia = Vector3.Distance(jugador.position, obj.transform.position);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                masCercano = obj.transform;
            }
        }

        objetivoMasCercano = masCercano;
    }

    void ApuntarFlecha()
    {
        // Dirección hacia el objetivo, ignorando altura (Y) para que la flecha
        // funcione como una brújula plana sin importar el desnivel del terreno.
        Vector3 direccionObjetivo = objetivoMasCercano.position - jugador.position;
        direccionObjetivo.y = 0f;

        Vector3 direccionCamara = camaraPrincipal.transform.forward;
        direccionCamara.y = 0f;

        // Ángulo entre hacia dónde mira la cámara y hacia dónde está el objetivo.
        float angulo = Vector3.SignedAngle(direccionCamara, direccionObjetivo, Vector3.up);

        flecha.localRotation = Quaternion.Euler(0f, 0f, -angulo);
    }
}