using UnityEngine;

namespace RiverSoft
{
    public static class ViewportWorldUtility
    {
        /// <summary>
        /// Calculates the world position from a given viewport anchor point, depth, and bounds extents.
        /// The resulting point is adjusted by subtracting the bounds extents (X and Y).
        /// </summary>
        /// <param name="camera">The camera used for conversion.</param>
        /// <param name="anchorPoint">The normalized viewport anchor point (X, Y).</param>
        /// <param name="depth">The depth (Z coordinate) to use for the conversion.</param>
        /// <param name="boundsExtents">The extents of the bounds to adjust the world position.</param>
        /// <returns>The calculated world position.</returns>
        public static Vector3 GetWorldPosition(Camera camera, Vector2 anchorPoint, float depth, Vector3 boundsExtents)
        {
            Vector3 viewportPoint = new Vector3(anchorPoint.x, anchorPoint.y, depth);
            Vector3 worldPoint = camera.ViewportToWorldPoint(viewportPoint);
            return worldPoint - new Vector3(boundsExtents.x, boundsExtents.y, 0);
        }

        /// <summary>
        /// Converts a world position to viewport coordinates while taking the bounds extents into account.
        /// The function adjusts the world position by adding the extents (X and Y) before conversion.
        /// </summary>
        /// <param name="camera">The camera used for conversion.</param>
        /// <param name="worldPosition">The world position to convert.</param>
        /// <param name="boundsExtents">The extents of the bounds to adjust the world position.</param>
        /// <returns>The resulting viewport point.</returns>
        public static Vector3 GetViewportPoint(Camera camera, Vector3 worldPosition, Vector3 boundsExtents)
        {
            Vector3 adjustedPosition = worldPosition + new Vector3(boundsExtents.x, boundsExtents.y, 0);
            return camera.WorldToViewportPoint(adjustedPosition);
        }
        
        /// <summary>
        /// Calculates the extents of the given collider.
        /// This returns the difference between the world position of the collider's local center and its transform's position.
        /// </summary>
        /// <param name="collider">The collider whose extents to calculate.</param>
        /// <returns>A Vector3 representing the offset from the collider's transform position.</returns>
        public static Vector3 GetColliderExtents(Collider collider)
        {
            if (collider == null)
                return Vector3.zero;
            
            Vector3 localCenter;
            if (collider is BoxCollider box)
                localCenter = box.center;
            else if (collider is SphereCollider sphere)
                localCenter = sphere.center;
            else if (collider is CapsuleCollider capsule)
                localCenter = capsule.center;
            else if (collider is MeshCollider mesh && mesh.sharedMesh != null)
                localCenter = mesh.sharedMesh.bounds.center;
            else
                // Fallback calculation if specific collider type is not handled.
                localCenter = collider.bounds.center - collider.transform.position;
            
            return collider.transform.TransformPoint(localCenter) - collider.transform.position;
        }
    }
}