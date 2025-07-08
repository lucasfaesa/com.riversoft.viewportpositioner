using Sirenix.OdinInspector;
using UnityEngine;

namespace RiverSoft
{
    /// <summary>
    /// ViewportPositioner positions a target Transform in world space based on a specified viewport anchor point and depth.
    /// 
    /// Features:
    /// - Uses a Camera to convert viewport coordinates to world space
    /// - Optionally offsets position using a Renderer or Collider to center the target based on bounds
    /// - Supports editor-time movement when 'moveInEditor' is enabled
    /// - Anchor point (0 to 1) determines viewport placement (e.g., center, corner)
    /// - Depth controls distance from the camera
    /// - Gizmo visualization to aid in positioning
    /// </summary>
    [ExecuteAlways]
    public class ViewportPositioner : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] private Camera mainCamera;
        [Header("Target References")]
        [SerializeField] private Transform target;
        [InfoBox("Optional: Will get the center of the object based on its mesh")]
        [SerializeField] private Renderer referenceRenderer;
        [InfoBox("Optional: Will get the center of the object based on its collider, need to check \"useColliderBounds\"", InfoMessageType.Info)]
        [SerializeField] private Collider referenceCollider;
        [Header("Settings")]
        [SerializeField] private bool useColliderBounds = false;
        [SerializeField] private bool moveInEditor;
        [SerializeField] private float gizmoSize = 0.05f;
        [Header("Distance Data")]
        [SerializeField] private float depth = 3f;
        [SerializeField, Range(0f, 1f)] private float anchorX = 0.5f;
        [SerializeField, Range(0f, 1f)] private float anchorY = 0.5f;
        
        private Vector3 _gizmoHandlePosition;
        
        public Transform Target => target;
        public Renderer ReferenceRenderer => referenceRenderer;
        public Collider ReferenceCollider => referenceCollider;
        public Vector2 AnchorPoint => new (anchorX, anchorY);
        public float Depth => depth;
        public bool UseColliderBounds => useColliderBounds;
        
        public Camera MainCamera
        {
            get => mainCamera;
            set => mainCamera = value;
        }

        public Vector3 GizmoHandlePosition
        {
            set => _gizmoHandlePosition = value;
        }

        private void Update()
        {
            if (Application.isPlaying || moveInEditor)
            {
                Vector3 extents = GetExtents();
                Vector3 worldPos = ViewportWorldUtility.GetWorldPosition(MainCamera, AnchorPoint, Depth, extents);
                if (Target != null)
                    Target.position = worldPos;
            }
        }

        private Vector3 GetExtents()
        {
            if (UseColliderBounds && ReferenceCollider != null)
            {
                return ViewportWorldUtility.GetColliderExtents(ReferenceCollider);
            }
            else if (ReferenceRenderer != null)
            {
                return ReferenceRenderer.bounds.center - ReferenceRenderer.transform.position;
            }

            return Vector3.zero;
        }

        public void SetViewportAnchor(Vector2 newAnchor, float newDepth)
        {
            anchorX = newAnchor.x;
            anchorY = newAnchor.y;
            depth = newDepth;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_gizmoHandlePosition, gizmoSize);
        }
    }
}