using UnityEngine;

namespace RiverSoft
{
    public static class ViewportCoordinatePlacer
    {
        /// <summary>
        /// Move a transform to the world position defined by a viewport anchor and depth,
        /// optionally centering by a Collider or Renderer bounds.
        /// </summary>
        /// <param name="anchor">Normalized viewport coordinates (0–1)</param>
        /// <param name="depth">Distance from camera along its forward axis</param>
        /// <param name="targetTransform">Transform to move</param>
        /// <param name="mainCamera">Camera whose viewport is used</param>
        /// <param name="boundsSource">
        /// Optional: Collider for its extents, or Renderer for its mesh-center offset.
        /// Pass null to ignore.</param>
        public static void SetPositionFromViewport(Vector2 anchor, float depth, Transform targetTransform, Camera mainCamera, object boundsSource = null)
        {
            if (targetTransform == null)
            {
                Debug.LogWarning("ViewportPositioner: targetTransform is null");
                return;
            }

            if (mainCamera == null)
            {
                Debug.LogWarning("ViewportPositioner: mainCamera is null");
                return;
            }

            // compute any bounds offset
            Vector3 extents = Vector3.zero;
            if (boundsSource is Collider collider)
            {
                extents = ViewportWorldUtility.GetColliderExtents(collider);
            }
            else if (boundsSource is Renderer renderer)
            {
                extents = renderer.bounds.center - renderer.transform.position;
            }

            // calculate world position and apply
            Vector3 worldPos = ViewportWorldUtility.GetWorldPosition(mainCamera, anchor, depth, extents);

            targetTransform.position = worldPos;
        }
    }
}
