using UnityEngine;

public sealed class ReticleGazeInteractor : MonoBehaviour
{
    [SerializeField] private float maxDistance = 30f;
    [SerializeField] private LayerMask interactiveLayers = ~0;

    private GazeFillSelection currentSelection;

    private void Update()
    {
        GazeFillSelection detectedSelection = GetGazeTarget();
        UpdateCurrentSelection(detectedSelection);
    }

    private GazeFillSelection GetGazeTarget()
    {
        Ray gazeRay = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(gazeRay, out RaycastHit hit, maxDistance, interactiveLayers, QueryTriggerInteraction.Collide))
        {
            return hit.collider.GetComponentInParent<GazeFillSelection>();
        }

        return null;
    }

    private void UpdateCurrentSelection(GazeFillSelection detectedSelection)
    {
        if (detectedSelection == currentSelection)
        {
            return;
        }

        if (currentSelection != null)
        {
            currentSelection.EndGaze();
        }

        currentSelection = detectedSelection;

        if (currentSelection != null)
        {
            currentSelection.BeginGaze();
        }
    }

    private void OnDisable()
    {
        if (currentSelection != null)
        {
            currentSelection.EndGaze();
            currentSelection = null;
        }
    }
}