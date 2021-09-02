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
        private ViewerGORef selection;

        private readonly SearchBox filterBox = new SearchBox();

        private List<ViewerGORef> activeRendererComponents = new List<ViewerGORef>();

        public HierarchyView(int col, int row, int width, int height) : base(col, row, width, height, "Hierarchy")
        {
            filterBox.OnSearch += FilterBox_OnSearch;
        }

        protected override void Draw(int windowID)
        {
            filterBox.Draw();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh", GUILayout.Width(250)))
            {
                Refresh();
            }
            if (GUILayout.Button("Collapse All", GUILayout.Width(250)))
            {
                foreach (var goref in allRefs)
                    Collapse(goref);
            }
            GUILayout.EndHorizontal();

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
            if (selection != null && (selection == goref || (selection.Reference != null && selection.Reference == goref.Reference)))
                GUI.color = selection.Reference?.activeInHierarchy ?? true ? Color.green : new Color(0, 0.5f, 0);

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
                    else if (Event.current.button == 0)
                    {
                        // Open all parents leading to selection so filter can be cleared while preserving hierarchy
                        SetSelection(goref, expand: filtered);
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

        public bool IsEmpty() => !allRefs.Any();

        public void SelectObjectUnderCursor()
        {
            // TODO does not work for UI yet
            if (activeRendererComponents?.Count > 0)
            {
                Vector3 mousePos = Input.mousePosition;

                ViewerGORef closestObject = null;
                float closestDistance = float.PositiveInfinity;

                foreach (var obj in activeRendererComponents)
                {
                    if (obj.Reference == null)
                        continue;

                    Vector3 delta = mousePos - Camera.current.WorldToScreenPoint(obj.Reference.transform.position);
                    delta.z = 0;

                    float distance = delta.sqrMagnitude;
                    if (closestObject == null || distance < closestDistance)
                    {
                        closestObject = obj;
                        closestDistance = distance;
                    }
                }

                if (closestObject != null)
                {
                    OnTargetGameObject?.Invoke(closestObject);
                    SetSelection(closestObject, true);
                }
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
            if (current == target || current.Reference != null && current.Reference == target.Reference)
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
            var sceneGraphReader = new SceneGraphReader();
            allRefs = sceneGraphReader.GetAllGameObjectReferences();
            activeRendererComponents = sceneGraphReader.GetAllActiveRendererComponents();
            SetSelection(null, false);
        }

        public void SetSelection(ViewerGORef newSelection, bool expand)
        {
            selection = newSelection;

            if (expand)
                ExpandPathToGameObject(newSelection);
        }

        private void Collapse(ViewerGORef goref)
        {
            goref.Expanded = false;
            foreach (var child in goref.Children)
                Collapse(child);
        }
    }
}