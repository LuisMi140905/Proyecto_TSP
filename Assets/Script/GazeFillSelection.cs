using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public sealed class GazeFillSelection : MonoBehaviour
{
    [SerializeField] private float fillTime = 1.5f;
    [SerializeField] private Image radialImage;
    [SerializeField] private UnityEvent onFillComplete;

    private Coroutine fillCoroutine;
    private bool isGazedAt;
    private bool hasCompleted;

    private void Awake()
    {
        ResetFill();
    }

    public void BeginGaze()
    {
        isGazedAt = true;
        hasCompleted = false;

        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
        }

        fillCoroutine = StartCoroutine(FillRadial());
    }

    public void EndGaze()
    {
        isGazedAt = false;
        hasCompleted = false;

        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
            fillCoroutine = null;
        }

        ResetFill();
    }

    private IEnumerator FillRadial()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fillTime)
        {
            if (!isGazedAt)
            {
                yield break;
            }

            elapsedTime += Time.deltaTime;

            if (radialImage != null)
            {
                radialImage.fillAmount = Mathf.Clamp01(elapsedTime / fillTime);
            }

            yield return null;
        }

        if (!hasCompleted)
        {
            hasCompleted = true;
            onFillComplete?.Invoke();
        }
    }

    private void ResetFill()
    {
        if (radialImage != null)
        {
            radialImage.fillAmount = 0f;
        }
    }

    private void OnDisable()
    {
        EndGaze();
    }
}