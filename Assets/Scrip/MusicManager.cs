using UnityEngine;

/// <summary>
/// Reproduce música de fondo en loop a bajo volumen, solo en esta escena.
/// Colocar en un GameObject vacío llamado "MusicManager".
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioClip musicaFondo;
    [Range(0f, 1f)]
    [SerializeField] private float volumenNormal = 0.2f;
    [Range(0f, 1f)]
    [SerializeField] private float volumenFinDePartida = 0.05f;

    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = musicaFondo;
        audioSource.loop = true;
        audioSource.volume = volumenNormal;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        if (musicaFondo != null)
        {
            audioSource.Play();
        }
    }

    /// <summary>
    /// Baja el volumen de la música (llamar al ganar o perder).
    /// </summary>
    public void BajarVolumen()
    {
        audioSource.volume = volumenFinDePartida;
    }
}