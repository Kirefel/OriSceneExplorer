using System;
using System.Collections.Generic;
using UnityEngine;

namespace OriSceneExplorer
{
    public class HistoryView : EditorView
    {
        private readonly List<ViewerGORef> history = new List<ViewerGORef>();
        private int current = -1;

        public event Action<ViewerGORef> OnSelectionChange;
        Vector2 scroll = Vector2.zero;

        public HistoryView(int col, int row, int width, int height) : base(col, row, width, height, "History")
        {

        }

        protected override void Draw(int windowID)
        {
            if (GUILayout.Button("Clear", GUILayout.Width(150)))
                Reset();

            scroll = GUILayout.BeginScrollView(scroll);

            // for (int i = history.Count - 1; i >= 0; i--)
            for (int i = 0; i < history.Count; i++)
            {
                string label = $"{(i == current ? "> " : "")}{history[i].Name}";
                if (GUILayout.Button(label, "Label") && i != current)
                {
                    Pick(i);
                }
            }

            GUILayout.EndScrollView();
        }

        private void Pick(int index)
        {
            current = index;
            OnSelectionChange?.Invoke(history[index]);
        }

        public void Push(ViewerGORef goref)
        {
            int index = history.FindIndex(x => x.Reference != null && x.Reference == goref.Reference);
            if (index >= 0)
            {
                current = index;
            }
            else
            {
                history.Add(goref);
                current = history.Count - 1;
            }
        }

        public void Reset()
        {
            current = -1;
            history.Clear();
        }
    }
}
