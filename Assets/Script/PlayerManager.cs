using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Configuración de Vidas")]
    public int vidas = 3;
    public Transform puntoReaparicionJugador; // Dónde aparece el jugador tras perder una vida

    [Header("Conexión con el Nivel")]
    public float exitRange = 2f;
    public MonoBehaviour scriptMovimiento;
    public EnemyAI scriptMinotauro; // Arrastra al minotauro aquí
    public Transform puntoReaparicionMinotauro; // Dónde vuelve el minotauro

    private bool yaMeAtraparon = false;

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.gameWon || yaMeAtraparon) return;

        float distanceToExit = Vector3.Distance(transform.position, GameManager.Instance.exit.position);
        if (distanceToExit < exitRange)
        {
            GameManager.Instance.WinGame();
        }
    }

    public void IniciarJumpscare(Transform posicionMinotauro)
    {
        if (yaMeAtraparon) return;
        yaMeAtraparon = true;

        // 1. Apagamos controles y físicas
        if (scriptMovimiento != null) scriptMovimiento.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // 2. Restamos vida y decidimos qué hacer
        vidas--;
        Debug.Log("¡Te atraparon! Vidas restantes: " + vidas);

        StartCoroutine(RutinaJumpscare(posicionMinotauro));
    }

    private IEnumerator RutinaJumpscare(Transform objetivo)
    {
        // Giro hacia el minotauro (0.3 segundos)
        Vector3 direccion = objetivo.position - transform.position;
        direccion.y = 0;
        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);

        float tiempoPasado = 0f;
        while (tiempoPasado < 0.3f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, tiempoPasado / 0.3f);
            tiempoPasado += Time.deltaTime;
            yield return null;
        }
        transform.rotation = rotacionObjetivo;

        // Esperamos 1.5 segundos para ver la animación del minotauro
        yield return new WaitForSeconds(1.5f);

        if (vidas > 0)
        {
            // REAPARECER
            RestablecerPosiciones();
        }
        else
        {
            // GAME OVER DEFINITIVO
            Debug.Log("GAME OVER");
            // Aquí puedes llamar a tu GameManager.Instance.LoseGame(); o cargar menú
        }
    }

    private void RestablecerPosiciones()
    {
        // Movemos al jugador
        transform.position = puntoReaparicionJugador.position;
        transform.rotation = puntoReaparicionJugador.rotation;

        // Reactivamos físicas y movimiento
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;
        if (scriptMovimiento != null) scriptMovimiento.enabled = true;

        // --- NUEVO: Mandamos al minotauro a un punto aleatorio ---
        if (scriptMinotauro != null)
        {
            scriptMinotauro.ResetPositionRandom();
        }

        yaMeAtraparon = false;
    }
}