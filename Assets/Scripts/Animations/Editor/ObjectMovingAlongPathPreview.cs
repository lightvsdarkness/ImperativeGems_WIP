using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace IG
{
    public class ObjectMovingAlongPathPreview
    {
        static public ObjectMovingAlongPathPreview s_Preview = null;
        static public GameObject preview;

        static protected ObjectMovingAlongPath ObjectMovingAlongPath;

        static ObjectMovingAlongPathPreview()
        {
            Selection.selectionChanged += SelectionChanged;
        }

        static void SelectionChanged()
        {
            if (ObjectMovingAlongPath != null && Selection.activeGameObject != ObjectMovingAlongPath.gameObject)
            {
                DestroyPreview();
            }
        }

        static public void DestroyPreview()
        {
            if (preview == null)
                return;

            Object.DestroyImmediate(preview);
            preview = null;
            ObjectMovingAlongPath = null;
        }

        static public void CreateNewPreview(ObjectMovingAlongPath origin)
        {
            if(preview != null)
            {
                Object.DestroyImmediate(preview);
            }

            ObjectMovingAlongPath = origin;

            preview = Object.Instantiate(origin.gameObject);
            preview.hideFlags = HideFlags.DontSave;
            ObjectMovingAlongPath plt = preview.GetComponentInChildren<ObjectMovingAlongPath>();
            Object.DestroyImmediate(plt);


            Color c = new Color(0.2f, 0.2f, 0.2f, 0.4f);
            SpriteRenderer[] rends = preview.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < rends.Length; ++i)
                rends[i].color = c;
        }
    }
}