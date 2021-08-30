using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OriSceneExplorer
{
    public class HierarchyView : EditorView
    {
        Vector2 hierarchyscroll = Vector2.zero;
        const int indentSize = 20;
        List<ViewerGORef> allRefs = new List<ViewerGORef>();
        private bool filtered = false;

        public event Action<ViewerGORef> OnTargetGameObject;

        private readonly SearchBox filterBox = new SearchBox();

        public HierarchyView(int col, int row, int width, int height) : base(col, row, width, height, "Hierarchy")
        {
            filterBox.OnSearch += FilterBox_OnSearch;
        }

        protected override void Draw(int windowID)
        {
            filterBox.Draw();

            hierarchyscroll = GUILayout.BeginScrollView(hierarchyscroll);
            foreach (var goref in allRefs)
            {
                DrawRef(goref);
            }
            GUILayout.EndScrollView();
        }

        private void DrawRef(ViewerGORef goref)
        {
            if (filtered && !goref.MatchesFilter)
                return;

            GUILayout.BeginHorizontal();
            GUILayout.Space(goref.Depth * indentSize);

            GUI.color = goref.Colour;
            if (GUILayout.Button(goref.Label, "Label"))
            {
                if (Event.current.button == 1)
                {
                    goref.Expanded = !goref.Expanded;
                }
                else
                {
                    // TODO improve interaction
                    if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && goref.Reference != null)
                    {
                        goref.Reference.SetActive(!goref.Reference.activeInHierarchy);
                    }
                    else
                    {
                        // Open all parents leading to selection so filter can be cleared while preserving hierarchy
                        if (filtered)
                            ExpandPathToGameObject(goref);

                        OnTargetGameObject?.Invoke(goref);
                    }
                }
            }
            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            // Auto expand everything if the filter is enabled
            if (goref.Expanded || filtered)
            {
                foreach (var c in goref.Children)
                    DrawRef(c);
            }
        }

        private void FilterBox_OnSearch(string filterText)
        {
            if (string.IsNullOrEmpty(filterText))
            {
                filtered = false;
                return;
            }

            foreach (var sceneRef in allRefs)
            {
                int count = SendFilter(sceneRef, filterText.ToUpper());
                sceneRef.MatchesFilter = count > 0;
            }

            filtered = true;
        }

        private int SendFilter(ViewerGORef goref, string filterText)
        {
            int count = goref.Children.Aggregate(0, (acc, child) => acc + SendFilter(child, filterText));

            goref.MatchesFilter = count > 0 || goref.Name.ToUpper().Contains(filterText);

            if (goref.MatchesFilter)
                count++;

            return count;
        }

        // TODO remove these double loops by giving all scenes one root
        // TODO improve search performance? store parents?
        private void ExpandPathToGameObject(ViewerGORef goref)
        {
            foreach (var sceneRef in allRefs)
            {
                if (ExpandPathToGameObject(sceneRef, goref))
                {
                    sceneRef.Expanded = true;
                    break;
                }
            }
        }

        private bool ExpandPathToGameObject(ViewerGORef current, ViewerGORef target)
        {
            if (current == target)
                return true;

            foreach (var child in current.Children)
            {
                if (ExpandPathToGameObject(child, target))
                {
                    current.Expanded = true;
                    return true;
                }
            }

            return false;
        }

        public void Refresh()
        {
            allRefs = new SceneGraphReader().GetAllGameObjectReferences();
        }
    }
}