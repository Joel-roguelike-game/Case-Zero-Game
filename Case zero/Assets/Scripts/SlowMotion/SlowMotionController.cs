using UnityEngine;
using System.Collections;

public class SlowMotionController : MonoBehaviour
{
    public static SlowMotionController Instance;

    [Header("Parry Slow Motion")]
    public float slowScale = 0.33f;
    public float duration = 0.5f;

    private Coroutine slowRoutine;

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

    public void TriggerParrySlow()
    {
        if (slowRoutine != null)
            StopCoroutine(slowRoutine);

        slowRoutine = StartCoroutine(SlowRoutine());
    }

    private IEnumerator SlowRoutine()
    {
        Debug.Log("SLOW MOTION ACTIVADO");

        Time.timeScale = slowScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        Debug.Log("SLOW MOTION DESACTIVADO");
    }
}