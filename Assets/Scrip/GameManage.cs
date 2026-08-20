using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Administra el estado general del juego: contador de coleccionables,
/// condición de victoria y condición de derrota.
/// Debe colocarse en un GameObject vacío llamado "GameManager".
/// Usa el patrón Singleton para que otros scripts puedan acceder fácilmente a él.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class GameManage : MonoBehaviour
{
    public static GameManage Instance { get; private set; }

    [Header("Configuración de coleccionables")]
    [SerializeField] private int totalColeccionables = 5;
    private int coleccionablesRecogidos = 0;

    [Header("Referencias UI")]
    [SerializeField] private TMP_Text textoContador;
    [SerializeField] private GameObject panelVictoria;
    [SerializeField] private GameObject panelDerrota;

    [Header("Referencias de sistemas")]
    [SerializeField] private GameTime temporizador;

    [Header("Sonidos de fin de partida")]
    [SerializeField] private AudioClip sonidoVictoria;
    [SerializeField] private AudioClip sonidoDerrota;

    [Header("Transición de nivel")]
    [SerializeField] private float retrasoAntesDeCambiarNivel = 3f;

    [Header("Nombres de escenas (deben coincidir EXACTO con el nombre real y estar en Build Settings)")]
    [SerializeField] private string nombreNivel1 = "Level 1";
    [SerializeField] private string nombreNivel2 = "Level 2";

    private AudioSource audioSource;
    private bool juegoTerminado = false;

    private void Awake()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        ActualizarTextoContador();

        if (panelVictoria != null) panelVictoria.SetActive(false);
        if (panelDerrota != null) panelDerrota.SetActive(false);
    }

    /// <summary>
    /// Se llama cada vez que el jugador recoge un coleccionable.
    /// </summary>
    public void SumarColeccionable()
    {
        if (juegoTerminado) return;

        coleccionablesRecogidos++;
        ActualizarTextoContador();

        // Si recogió todos los coleccionables, gana la partida
        if (coleccionablesRecogidos >= totalColeccionables)
        {
            Victoria();
        }
    }

    private void ActualizarTextoContador()
    {
        if (textoContador != null)
        {
            textoContador.text = "Coleccionables: " + coleccionablesRecogidos + " / " + totalColeccionables;
        }
    }

    public void Victoria()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        if (temporizador != null) temporizador.DetenerTemporizador();
        if (panelVictoria != null) panelVictoria.SetActive(true);

        if (sonidoVictoria != null) audioSource.PlayOneShot(sonidoVictoria);
        if (MusicManager.Instance != null) MusicManager.Instance.BajarVolumen();

        Time.timeScale = 0f; // Pausa el juego

        string escenaActual = SceneManager.GetActiveScene().name;

        if (escenaActual == nombreNivel1)
        {
            // Victoria en el nivel 1 -> pasa al nivel 2
            StartCoroutine(CargarNivel(nombreNivel2));
        }
        else
        {
            // Victoria en el nivel 2 (último nivel) -> el juego termina aquí.
            // Se queda mostrando el panel de Victoria, no carga ninguna escena más.
            Debug.Log("¡Juego completado! Victoria final.");
        }
    }

    public void Derrota()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        if (temporizador != null) temporizador.DetenerTemporizador();
        if (panelDerrota != null) panelDerrota.SetActive(true);

        if (sonidoDerrota != null) audioSource.PlayOneShot(sonidoDerrota);
        if (MusicManager.Instance != null) MusicManager.Instance.BajarVolumen();

        Time.timeScale = 0f; // Pausa el juego
        // No cambia de escena en ningún caso: mismo comportamiento en Level 1 y Level 2.
    }

    private System.Collections.IEnumerator CargarNivel(string nombreEscena)
    {
        // WaitForSecondsRealtime porque Time.timeScale está en 0 (pausado)
        yield return new WaitForSecondsRealtime(retrasoAntesDeCambiarNivel);

        Time.timeScale = 1f; // restaurar antes de cambiar de escena
        SceneManager.LoadScene(nombreEscena);
    }
}