using UnityEngine;
public class SpinItem : MonoBehaviour { [SerializeField] private float velocidadGiro = 90f; // grados por segundo

 [SerializeField] private Vector3 ejeGiro = Vector3.up; private void Update() { 

        transform.Rotate(ejeGiro * velocidadGiro * Time.deltaTime);

    } 
}