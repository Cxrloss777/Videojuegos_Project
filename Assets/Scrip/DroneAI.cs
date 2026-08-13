using UnityEngine;

/// <summary>
/// SCR_DroneAI - Controla el comportamiento del dron de seguridad:
/// patrulla entre waypoints, detecta al jugador por rango + línea de visión,
/// lo persigue y le hace daño al contacto.
/// Compatible con HealSystem (llama a TakeDamage).
/// </summary>
public class DroneAI : MonoBehaviour
{
    public enum DroneState
    {
        Patrulla,
        Alerta,
        Persecucion,
        Ataque
    }

    [Header("Referencias")]
    [Tooltip("Puntos de patrulla. Si está vacío, el dron se queda como centinela fijo.")]
    public Transform[] waypoints;
    public Transform jugador;

    [Header("Movimiento")]
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 4f;
    public float distanciaLlegadaWaypoint = 0.3f;

    [Header("Detección")]
    public float rangoDeteccion = 8f;
    public float rangoAtaque = 1.5f;
    [Tooltip("Si está activo, ignora el ángulo de visión y detecta al jugador apenas entra al rango, sin importar hacia dónde mire el dron.")]
    public bool deteccionOmnidireccional = true;
    [Range(0, 360)]
    public float anguloVision = 90f;
    public LayerMask capaObstaculos; // paredes/props que bloquean la vista
    public float tiempoPerderJugador = 3f; // segundos sin verlo antes de volver a patrullar

    [Header("Daño")]
    public float dañoPorAtaque = 10f;
    public float cooldownAtaque = 1.5f;

    [Header("Debug")]
    public bool mostrarGizmos = true;

    private DroneState estadoActual = DroneState.Patrulla;
    private int waypointActual = 0;
    private float temporizadorSinVerJugador = 0f;
    private float temporizadorAtaque = 0f;

    void Update()
    {
        if (jugador == null) return;

        switch (estadoActual)
        {
            case DroneState.Patrulla:
                Patrullar();
                if (PuedeVerJugador()) CambiarEstado(DroneState.Persecucion);
                break;

            case DroneState.Persecucion:
                Perseguir();
                if (DistanciaAlJugador() <= rangoAtaque)
                {
                    CambiarEstado(DroneState.Ataque);
                }
                else if (!PuedeVerJugador())
                {
                    temporizadorSinVerJugador += Time.deltaTime;
                    if (temporizadorSinVerJugador >= tiempoPerderJugador)
                    {
                        CambiarEstado(DroneState.Patrulla);
                    }
                }
                else
                {
                    temporizadorSinVerJugador = 0f;
                }
                break;

            case DroneState.Ataque:
                temporizadorAtaque -= Time.deltaTime;
                if (DistanciaAlJugador() > rangoAtaque)
                {
                    CambiarEstado(DroneState.Persecucion);
                }
                else if (temporizadorAtaque <= 0f)
                {
                    Atacar();
                    temporizadorAtaque = cooldownAtaque;
                }
                break;
        }
    }

    // ---------- PATRULLA ----------
    void Patrullar()
    {
        if (waypoints == null || waypoints.Length == 0) return; // centinela fijo

        Transform destino = waypoints[waypointActual];
        MoverHacia(destino.position, velocidadPatrulla);

        if (Vector3.Distance(transform.position, destino.position) <= distanciaLlegadaWaypoint)
        {
            waypointActual = (waypointActual + 1) % waypoints.Length;
        }
    }

    // ---------- PERSECUCIÓN ----------
    void Perseguir()
    {
        MoverHacia(jugador.position, velocidadPersecucion);
    }

    void MoverHacia(Vector3 destino, float velocidad)
    {
        Vector3 direccion = (destino - transform.position);
        direccion.y = 0f; // mantener el dron en su plano (quitar si vuela libremente en Y)
        if (direccion.sqrMagnitude < 0.001f) return;

        direccion.Normalize();
        transform.position += direccion * velocidad * Time.deltaTime;
        transform.forward = Vector3.Lerp(transform.forward, direccion, 10f * Time.deltaTime);
    }

    // ---------- DETECCIÓN ----------
    bool PuedeVerJugador()
    {
        float distancia = DistanciaAlJugador();
        if (distancia > rangoDeteccion) return false;

        Vector3 direccionJugador = (jugador.position - transform.position).normalized;

        if (!deteccionOmnidireccional)
        {
            float angulo = Vector3.Angle(transform.forward, direccionJugador);
            if (angulo > anguloVision * 0.5f) return false;
        }

        // Raycast para confirmar que no hay pared entre el dron y el jugador
        if (Physics.Raycast(transform.position, direccionJugador, out RaycastHit hit, rangoDeteccion, capaObstaculos))
        {
            return false; // algo bloquea la vista
        }

        return true;
    }

    float DistanciaAlJugador()
    {
        return Vector3.Distance(transform.position, jugador.position);
    }

    // ---------- ATAQUE ----------
    void Atacar()
    {
        HealSystem salud = jugador.GetComponent<HealSystem>();
        if (salud != null)
        {
            salud.RecibirDano(dañoPorAtaque);
        }
    }

    void CambiarEstado(DroneState nuevoEstado)
    {
        estadoActual = nuevoEstado;
        temporizadorSinVerJugador = 0f;
    }

    // ---------- DEBUG VISUAL ----------
    void OnDrawGizmosSelected()
    {
        if (!mostrarGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}