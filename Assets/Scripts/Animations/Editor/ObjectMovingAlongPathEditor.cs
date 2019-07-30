using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace IG
{
    [CustomEditor(typeof(ObjectMovingAlongPath))]
    public class ObjectMovingAlongPathEditor : Editor
    {
        ObjectMovingAlongPath m_ObjectMovingAlongPath;

        float m_PreviewPosition = 0;

        private void OnEnable()
        {
            m_PreviewPosition = 0;
            m_ObjectMovingAlongPath = target as ObjectMovingAlongPath;

            if(!EditorApplication.isPlayingOrWillChangePlaymode)
                ObjectMovingAlongPathPreview.CreateNewPreview(m_ObjectMovingAlongPath);
        }

        private void OnDisable()
        {
            ObjectMovingAlongPathPreview.DestroyPreview();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            m_ObjectMovingAlongPath.ObjectCatcher = EditorGUILayout.ObjectField("Object Catcher", m_ObjectMovingAlongPath.ObjectCatcher, typeof(ObjectCatcher), true) as ObjectCatcher;
            if (EditorGUI.EndChangeCheck())
                Undo.RecordObject(target, "Changed Catcher");

            EditorGUI.BeginChangeCheck();
            m_PreviewPosition = EditorGUILayout.Slider("Preview position", m_PreviewPosition, 0.0f, 1.0f);
            if (EditorGUI.EndChangeCheck())
            {
                MovePreview();
            }

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();
            bool isStartingMoving = EditorGUILayout.Toggle("Start moving", m_ObjectMovingAlongPath.StartMovingAtStart);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed move at start");
                m_ObjectMovingAlongPath.StartMovingAtStart = isStartingMoving;
            }

            if(isStartingMoving)
            {
                EditorGUI.indentLevel += 1;
                EditorGUI.BeginChangeCheck();
                bool startOnlyWhenVisible = EditorGUILayout.Toggle("When becoming visible", m_ObjectMovingAlongPath.StartMovingOnlyWhenVisible);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Changed move when visible");
                    m_ObjectMovingAlongPath.StartMovingOnlyWhenVisible = startOnlyWhenVisible;
                }
                EditorGUI.indentLevel -= 1;
            }
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            ObjectMovingAlongPath.MovingPlatformType platformType = (ObjectMovingAlongPath.MovingPlatformType)EditorGUILayout.EnumPopup("Looping", m_ObjectMovingAlongPath.PlatformType);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Moving Platform type");
                m_ObjectMovingAlongPath.PlatformType = platformType;
            }

            EditorGUI.BeginChangeCheck();
            float newSpeed = EditorGUILayout.FloatField("Speed", m_ObjectMovingAlongPath.Speed);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Speed");
                m_ObjectMovingAlongPath.Speed = newSpeed;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            if (GUILayout.Button("Add Node"))
            {
                Undo.RecordObject(target, "added node");


                Vector3 position = m_ObjectMovingAlongPath.NodesLocal[m_ObjectMovingAlongPath.NodesLocal.Length - 1] + Vector3.right;

                ArrayUtility.Add(ref m_ObjectMovingAlongPath.NodesLocal, position);
                ArrayUtility.Add(ref m_ObjectMovingAlongPath.NodesWaitTimes, 0);
            }

            EditorGUIUtility.labelWidth = 64;
            int delete = -1;
            for (int i = 0; i < m_ObjectMovingAlongPath.NodesLocal.Length; ++i)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginHorizontal();

                int size = 64;
                EditorGUILayout.BeginVertical(GUILayout.Width(size));
                EditorGUILayout.LabelField("Node " + i, GUILayout.Width(size));
                if (i != 0 && GUILayout.Button("Delete", GUILayout.Width(size)))
                {
                    delete = i;
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                Vector3 newPosition;
                if (i == 0)
                    newPosition = m_ObjectMovingAlongPath.NodesLocal[i];
                else
                    newPosition = EditorGUILayout.Vector3Field("Position", m_ObjectMovingAlongPath.NodesLocal[i]);

                float newTime = EditorGUILayout.FloatField("Wait Time", m_ObjectMovingAlongPath.NodesWaitTimes[i]);
                EditorGUILayout.EndVertical();


                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "changed time or position");
                    m_ObjectMovingAlongPath.NodesWaitTimes[i] = newTime;
                    m_ObjectMovingAlongPath.NodesLocal[i] = newPosition;
                }
            }
            EditorGUIUtility.labelWidth = 0;

            if (delete != -1)
            {
                Undo.RecordObject(target, "Removed point moving platform");

                ArrayUtility.RemoveAt(ref m_ObjectMovingAlongPath.NodesLocal, delete);
                ArrayUtility.RemoveAt(ref m_ObjectMovingAlongPath.NodesWaitTimes, delete);
            }
        }

        private void OnSceneGUI()
        {
            MovePreview();

            for (int i = 0; i < m_ObjectMovingAlongPath.NodesLocal.Length; ++i)
            {
                Vector3 worldPos;
                if (Application.isPlaying)
                {
                    worldPos = m_ObjectMovingAlongPath.NodesWorld[i];
                }
                else
                {
                    worldPos = m_ObjectMovingAlongPath.transform.TransformPoint(m_ObjectMovingAlongPath.NodesLocal[i]);
                }


                Vector3 newWorld = worldPos;
                if(i != 0)
                    newWorld = Handles.PositionHandle(worldPos, Quaternion.identity);

                Handles.color = Color.red;

                if (i == 0)
                {
                    if (m_ObjectMovingAlongPath.PlatformType != ObjectMovingAlongPath.MovingPlatformType.LOOP)
                        continue;

                    if (Application.isPlaying)
                    {
                        Handles.DrawDottedLine(worldPos, m_ObjectMovingAlongPath.NodesWorld[m_ObjectMovingAlongPath.NodesWorld.Length - 1], 10);
                    }
                    else
                    {
                        Handles.DrawDottedLine(worldPos, m_ObjectMovingAlongPath.transform.TransformPoint(m_ObjectMovingAlongPath.NodesLocal[m_ObjectMovingAlongPath.NodesLocal.Length - 1]), 10);
                    }
                }
                else
                {
                    if (Application.isPlaying)
                    {
                        Handles.DrawDottedLine(worldPos, m_ObjectMovingAlongPath.NodesWorld[i - 1], 10);
                    }
                    else
                    {
                        Handles.DrawDottedLine(worldPos, m_ObjectMovingAlongPath.transform.TransformPoint(m_ObjectMovingAlongPath.NodesLocal[i - 1]), 10);
                    }

                    if (worldPos != newWorld)
                    {
                        Undo.RecordObject(target, "moved point");
                        m_ObjectMovingAlongPath.NodesLocal[i] = m_ObjectMovingAlongPath.transform.InverseTransformPoint(newWorld);
                    }
                }
            }
        }

        void MovePreview()
        {
            //compute pos from 0-1 preview pos

            if (Application.isPlaying)
                return;

            float step = 1.0f / (m_ObjectMovingAlongPath.NodesLocal.Length - 1);

            int starting = Mathf.FloorToInt(m_PreviewPosition / step);

            if (starting > m_ObjectMovingAlongPath.NodesLocal.Length-2)
                return;

            float localRatio = (m_PreviewPosition - (step * starting)) / step;

            Vector3 localPos = Vector3.Lerp(m_ObjectMovingAlongPath.NodesLocal[starting], m_ObjectMovingAlongPath.NodesLocal[starting + 1], localRatio);

            ObjectMovingAlongPathPreview.preview.transform.position = m_ObjectMovingAlongPath.transform.TransformPoint(localPos);

            SceneView.RepaintAll();
        }
    }
}