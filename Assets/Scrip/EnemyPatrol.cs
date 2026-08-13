using UnityEngine;

/// <summary>
/// Controla el movimiento de patrullaje de un enemigo/obstáculo entre dos puntos,
/// y aplica daño al jugador al colisionar con él.
/// Debe colocarse en el prefab del enemigo, junto con un Rigidbody y un Collider.
/// </summary>
public class EnemyPatrol : MonoBehaviour
{
    [Header("Configuración de patrullaje")]
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;
    [SerializeField] private float velocidadPatrullaje = 3f;

    [Header("Configuración de daño")]
    [SerializeField] private float danoAlJugador = 10f;

    private Transform objetivoActual;

    private void Start()
    {
        // Empieza moviéndose hacia el punto B
        objetivoActual = puntoB;
    }
    // Esto lo dejo por si quisiera usar enemigos movibles al principio queria pero preferi un parkour
    //private void Update()
    //{
    //    if (puntoA == null || puntoB == null) return;

    //    // Mueve al enemigo hacia el objetivo actual
    //    transform.position = Vector3.MoveTowards(
    //        transform.position,
    //        objetivoActual.position,
    //        velocidadPatrullaje * Time.deltaTime
    //    );

    //    // Si llega al punto objetivo, cambia de dirección
    //    if (Vector3.Distance(transform.position, objetivoActual.position) < 0.2f)
    //    {
    //        objetivoActual = (objetivoActual == puntoA) ? puntoB : puntoA;
    //    }
    //}

    // Usar este método si el Collider del enemigo NO es un trigger (colisión física)
    private void OnCollisionEnter(Collision collision)
    {
        AplicarDano(collision.gameObject);
    }

    // Usar este método si el Collider del enemigo SÍ es un trigger
    private void OnTriggerEnter(Collider other)
    {
        AplicarDano(other.gameObject);
    }

    private void AplicarDano(GameObject objeto)
    {
        if (objeto.CompareTag("Player"))
        {
            HealSystem salud = objeto.GetComponent<HealSystem>();
            if (salud != null)
            {
                salud.RecibirDano(danoAlJugador);
            }
        }
    }
}