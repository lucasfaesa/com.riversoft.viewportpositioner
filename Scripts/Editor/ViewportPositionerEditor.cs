#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace RiverSoft.Editor
{
    /// <summary>
    /// Custom Editor helper for the ViewportPositioner component.
    /// 
    /// Features:
    /// - Displays a position handle in the Scene view for adjusting the viewport-based world position
    /// - Computes handle position based on anchor point, depth, and optional bounds (Renderer or Collider)
    /// - Updates the ViewportPositioner anchor and depth values when handle is moved
    /// - Draws a yellow line from the camera to the handle for visual context
    /// </summary>
    [CustomEditor(typeof(ViewportPositioner))]
    public class ViewportPositionerEditor : UnityEditor.Editor 
    {
        private void OnSceneGUI()
        {
            var pos = (ViewportPositioner)target;
            if (pos == null || pos.MainCamera == null)
                return;

            Vector3 extents = Vector3.zero;
            if (pos.UseColliderBounds && pos.ReferenceCollider != null)
            {
                extents = ViewportWorldUtility.GetColliderExtents(pos.ReferenceCollider);
            }
            else if (pos.ReferenceRenderer != null)
            {
                extents = pos.ReferenceRenderer.bounds.center - pos.ReferenceRenderer.transform.position;
            }
            
            Vector3 pivotWorldPos = ViewportWorldUtility.GetWorldPosition(pos.MainCamera, pos.AnchorPoint, pos.Depth, extents);
            Vector3 handlePos = pivotWorldPos + extents;

            EditorGUI.BeginChangeCheck();
            Vector3 newWorldPos = Handles.PositionHandle(handlePos, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(pos, "Move Viewport Position");

                Vector3 adjustedWorldPos = newWorldPos - extents;
                Vector3 newViewportPoint = ViewportWorldUtility.GetViewportPoint(pos.MainCamera, adjustedWorldPos, extents);
                pos.SetViewportAnchor(new Vector2(Mathf.Clamp01(newViewportPoint.x), Mathf.Clamp01(newViewportPoint.y)), newViewportPoint.z);
                EditorUtility.SetDirty(pos);
            }

            pos.GizmoHandlePosition = handlePos;

            Handles.color = Color.yellow;
            Handles.DrawLine(pos.MainCamera.transform.position, handlePos);
        }
    }
}

#endif