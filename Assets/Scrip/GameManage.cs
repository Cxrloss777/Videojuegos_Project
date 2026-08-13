using UnityEngine;
using TMPro;
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
    [SerializeField] private TMP_Text textoContador;      // Cambiar a TMP_Text si se usa TextMeshPro
    [SerializeField] private GameObject panelVictoria;
    [SerializeField] private GameObject panelDerrota;

    [Header("Referencias de sistemas")]
    [SerializeField] private GameTime temporizador;

    [Header("Sonidos de fin de partida")]
    [SerializeField] private AudioClip sonidoVictoria;
    [SerializeField] private AudioClip sonidoDerrota;

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
    }
}