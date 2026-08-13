using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objet : MonoBehaviour
{
    [SerializeField] private AudioClip sonidoRecoleccion;
    [SerializeField] private float volumen = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManage.Instance != null)
            {
                GameManage.Instance.SumarColeccionable();
            }

            if (sonidoRecoleccion != null)
            {
                AudioSource.PlayClipAtPoint(sonidoRecoleccion, transform.position, volumen);
            }

            Destroy(gameObject);
        }
    }
}