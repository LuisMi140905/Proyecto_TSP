using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Prueba3 : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float velocidadCaminar = 3.5f;

    [Header("Referencias")]
    public Transform camaraVR;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Bloqueamos rotación para que las gafas controlen la visión
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (camaraVR != null)
        {
            Vector3 direccionCamara = camaraVR.forward;
            direccionCamara.y = 0;
            direccionCamara.Normalize();

            Vector3 direccionDerecha = camaraVR.right;
            direccionDerecha.y = 0;
            direccionDerecha.Normalize();

            Vector3 movimiento = (direccionDerecha * x + direccionCamara * z) * velocidadCaminar;
            rb.linearVelocity = new Vector3(movimiento.x, rb.linearVelocity.y, movimiento.z);
        }
    }
}