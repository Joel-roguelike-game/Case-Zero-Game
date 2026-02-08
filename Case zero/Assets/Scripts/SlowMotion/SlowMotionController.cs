using UnityEngine;
using System.Collections;

/*
 * SlowMotionController
 *
 * Controla la cámara lenta del juego.
 * Se utiliza principalmente como feedback visual durante un parry exitoso.
 * Implementado como Singleton persistente entre escenas.
 */
public class SlowMotionController : MonoBehaviour
{
    public static SlowMotionController Instance;

    [Header("Parry Slow Motion")]
    public float slowScale = 0.33f;
    public float duration = 0.5f;

    private float previousTimeScale;
    private float previousFixedDelta;
    
    private Coroutine slowRoutine;

    /*
     * Inicializa el Singleton.
     * Asegura que solo exista una instancia y persista entre escenas.
     */
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /*
     * Lanza la cámara lenta asociada a un parry.
     * Si ya hay una activa, la reinicia.
     */
    public void TriggerParrySlow()
    {
        if (slowRoutine != null)
        {
            Time.timeScale = previousTimeScale;
            Time.fixedDeltaTime = previousFixedDelta;
            StopCoroutine(slowRoutine);
        }
        slowRoutine = StartCoroutine(SlowRoutine());
    }

    /*
     * Rutina que reduce el timeScale durante un tiempo
     * y luego lo restaura a valores normales.
     */
    private IEnumerator SlowRoutine()
    {
        Debug.Log("SLOW MOTION ACTIVADO");

        previousTimeScale = Time.timeScale;
        previousFixedDelta = Time.fixedDeltaTime;

        Time.timeScale = slowScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = previousTimeScale;
        Time.fixedDeltaTime = previousFixedDelta;


        Debug.Log("SLOW MOTION DESACTIVADO");
    }
}