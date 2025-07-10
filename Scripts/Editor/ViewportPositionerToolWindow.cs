#if UNITY_EDITOR

using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace RiverSoft.Editor
{
    public class ViewportPositionerToolWindow : EditorWindow
    {
        private ViewportPositioner existingVP;

        [MenuItem("RiverSoft/Viewport Positioner/VP Tool Window")]
        public static void ShowWindow()
        {
            GetWindow<ViewportPositionerToolWindow>("Viewport Positioner Tool");
        }

        private void OnEnable()
        {
            FindExistingInstance();
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("Viewport Positioner Utility", EditorStyles.boldLabel);

            if (existingVP == null)
            {
                if (GUILayout.Button("Create Viewport Positioner Object"))
                {
                    CreateVP();
                }
            }
            else
            {
                GUILayout.Label($"Active: {existingVP.name}", EditorStyles.helpBox);

                if (GUILayout.Button("Delete Viewport Positioner"))
                {
                    DeleteVP();
                    return;
                }

                if (GUILayout.Button("Select in Hierarchy"))
                {
                    Selection.activeGameObject = existingVP.gameObject;
                }
            
                if (GUILayout.Button("Copy Anchor & Depth From Position"))
                {
                    CopyAnchorFromTransform(existingVP);
                }

                EditorGUILayout.Space();
                GUILayout.Label("Current Values:", EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField("Anchor X", existingVP.AnchorPoint.x.ToString("F3"));
                EditorGUILayout.LabelField("Anchor Y", existingVP.AnchorPoint.y.ToString("F3"));
                EditorGUILayout.LabelField("Depth", existingVP.Depth.ToString("F3"));
            }
        }

        private void FindExistingInstance()
        {
            existingVP = FindFirstObjectByType<ViewportPositioner>();
        }

        private void CreateVP()
        {
            if (existingVP != null)
            {
                Debug.LogWarning("ViewportPositioner already exists in the scene.");
                return;
            }

            GameObject go = new GameObject("ViewportPositioner");
            Undo.RegisterCreatedObjectUndo(go, "Create ViewportPositioner");

            existingVP = go.AddComponent<ViewportPositioner>();

            if (Camera.main != null)
                existingVP.MainCamera = Camera.main;

            Selection.activeGameObject = go;
        }

        private void DeleteVP()
        {
            if (existingVP != null)
            {
                Undo.DestroyObjectImmediate(existingVP.gameObject);
                existingVP = null;
            }
        }

        private void CopyAnchorFromTransform(ViewportPositioner vp)
        {
            if (vp == null)
            {
                Debug.LogWarning("ViewportPositioner is null.");
                return;
            }

            // Just copy the existing values, no recalculation
            string copyString = string.Format(CultureInfo.GetCultureInfo("en-US"), 
                "{0:F4}  {1:F4}  {2:F4}", vp.AnchorPoint.x, vp.AnchorPoint.y, vp.Depth);
            EditorGUIUtility.systemCopyBuffer = copyString;

            Debug.Log($"Copied to clipboard: {copyString}");
        }

    }
}

#endif
