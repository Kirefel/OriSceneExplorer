using System;
using System.Collections.Generic;
using UnityEngine;

namespace OriSceneExplorer
{
    public class HierarchyView : EditorView
    {
        Vector2 hierarchyscroll = Vector2.zero;
        const int indentSize = 20;
        List<ViewerGORef> allRefs = new List<ViewerGORef>();

        public event Action<ViewerGORef> OnTargetGameObject;

        public HierarchyView(Rect windowRect) : base(windowRect, "Hierarchy")
        {
        }

        protected override void Draw(int windowID)
        {
            hierarchyscroll = GUILayout.BeginScrollView(hierarchyscroll);
            foreach (var goref in allRefs)
            {
                DrawRef(goref);
            }
            GUILayout.EndScrollView();
        }

        private void DrawRef(ViewerGORef goref)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(goref.Depth * indentSize);

            GUI.color = goref.Colour;
            if (GUILayout.Button(goref.Label, "Label"))
            {
                if (Event.current.button == 1)
                    goref.Expanded = !goref.Expanded;
                else
                {
                    if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && goref.Reference != null)
                        goref.Reference.SetActive(!goref.Reference.activeInHierarchy);
                    else
                        OnTargetGameObject?.Invoke(goref);
                }
            }
            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            if (goref.Expanded)
            {
                foreach (var c in goref.Children)
                    DrawRef(c);
            }
        }

        public void Refresh()
        {
            allRefs = new SceneGraphReader().GetAllGameObjectReferences();
        }
    }
}