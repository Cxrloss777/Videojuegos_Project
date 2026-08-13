using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Maneja la salud del jugador y su representación visual mediante un Slider.
/// Debe colocarse en el GameObject "Player".
/// </summary>
public class HealSystem : MonoBehaviour
{
    [Header("Configuración de salud")]
    [SerializeField] private float saludMaxima = 100f;
    private float saludActual;

    [Header("Referencias UI")]
    [SerializeField] private Slider barraDeSalud;

    private bool jugadorVivo = true;

    private void Start()
    {
        saludActual = saludMaxima;

        if (barraDeSalud != null)
        {
            barraDeSalud.maxValue = saludMaxima;
            barraDeSalud.value = saludActual;
        }
    }

    /// <summary>
    /// Reduce la salud del jugador. Se llama desde los enemigos/obstáculos.
    /// </summary>
    /// <param name="cantidadDano">Cantidad de daño a restar</param>
    public void RecibirDano(float cantidadDano)
    {
        if (!jugadorVivo) return;

        saludActual -= cantidadDano;
        saludActual = Mathf.Clamp(saludActual, 0f, saludMaxima);

        if (barraDeSalud != null)
        {
            barraDeSalud.value = saludActual;
        }

        // Si la salud llega a cero, se activa la condición de derrota
        if (saludActual <= 0f)
        {
            jugadorVivo = false;
            if (GameManage.Instance != null)
            {
                GameManage.Instance.Derrota();
            }
        }
    }
}