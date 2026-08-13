using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Controla el conteo regresivo del temporizador de la partida.
/// Debe colocarse en un GameObject vacío llamado "TimerManager" o similar.
/// </summary>
public class GameTime : MonoBehaviour
{
    [Header("Configuración del temporizador")]
    [SerializeField] private float tiempoInicial = 120f;

    [Header("Referencias UI")]
    [SerializeField] private TMP_Text textoTiempo; // Cambiar a TMP_Text si se usa TextMeshPro

    private float tiempoRestante;
    private bool temporizadorActivo = true;

    private void Start()
    {
        tiempoRestante = tiempoInicial;
        StartCoroutine(ContarTiempo());
    }

    /// <summary>
    /// Coroutine que reduce el tiempo restante cada segundo y actualiza la UI.
    /// </summary>
    private IEnumerator ContarTiempo()
    {
        while (tiempoRestante > 0 && temporizadorActivo)
        {
            ActualizarTextoUI();
            yield return new WaitForSeconds(1f);
            tiempoRestante--;
        }

        // Si el tiempo llega a 0, se activa la condición de derrota
        if (temporizadorActivo)
        {
            ActualizarTextoUI();
            if (GameManage.Instance != null)
            {
                GameManage.Instance.Derrota();
            }
        }
    }

    private void ActualizarTextoUI()
    {
        if (textoTiempo != null)
        {
            textoTiempo.text =  Mathf.CeilToInt(tiempoRestante).ToString() + "s" ;
        }
    }

    /// <summary>
    /// Detiene el temporizador (se llama al ganar o perder).
    /// </summary>
    public void DetenerTemporizador()
    {
        temporizadorActivo = false;
        StopAllCoroutines();
    }
}