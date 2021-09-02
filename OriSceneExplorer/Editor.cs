using UnityEngine;

namespace OriSceneExplorer
{
    public class Editor : MonoBehaviour
    {
        public static bool ready = false;
        private bool pause = false;
        private bool visible = false;
        private bool autoPause = true;

        private readonly HierarchyView hierarchyView = new HierarchyView(0, 0, 4, 12);
        private readonly ComponentsView componentsView = new ComponentsView(4, 0, 4, 12);
        private readonly LogView logsView = new LogView(8, 6, 4, 6);
        private readonly HistoryView historyView = new HistoryView(8, 0, 4, 6);

        public bool Paused
        {
            get => pause;
            set
            {
                if (pause != value)
                {
                    pause = value;
                    if (pause)
                        SuspensionManager.SuspendAll();
                    else
                        SuspensionManager.ResumeAll();
                }
            }
        }

        EditorView[] allViews;

        public void Start()
        {
            allViews = new EditorView[] { hierarchyView, componentsView, logsView, historyView };

            hierarchyView.OnTargetGameObject += componentsView.Load;
            hierarchyView.OnTargetGameObject += historyView.Push;
            componentsView.OnFocusProperty += historyView.Push;
            componentsView.OnFocusProperty += obj => hierarchyView.SetSelection(obj, true);
            historyView.OnSelectionChange += componentsView.Load;
            historyView.OnSelectionChange += obj => hierarchyView.SetSelection(obj, true);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                visible = !visible;
                if (autoPause)
                    Paused = visible;

                // Refresh on first opening the window
                if (visible && hierarchyView.IsEmpty())
                    hierarchyView.Refresh();
            }

            if (Input.GetKeyDown(KeyCode.F2))
                Paused = !Paused;

            if (Input.GetKeyDown(KeyCode.F5))
            {
                hierarchyView.Refresh();
                historyView.Reset();
            }

            if (Input.GetKeyDown(KeyCode.F4))
                logsView.ClearLogs();

            if (Input.GetKeyDown(KeyCode.Mouse2))
                hierarchyView.SelectObjectUnderCursor();
        }

        public void OnGUI()
        {
            if (!visible)
                return;

            hierarchyView.OnGUI();
            componentsView.OnGUI();
            logsView.OnGUI();
            historyView.OnGUI();

            DrawSettingBar();

            Dispatch.Execute();
        }

        private void DrawSettingBar()
        {
            GUILayout.BeginArea(new Rect(0, Screen.height - 20, Screen.width, 20));
            GUILayout.BeginHorizontal();
            hierarchyView.Visible = GUILayout.Toggle(hierarchyView.Visible, "Hierarchy", "Button", GUILayout.Width(200));
            componentsView.Visible = GUILayout.Toggle(componentsView.Visible, "Components", "Button", GUILayout.Width(200));
            historyView.Visible = GUILayout.Toggle(historyView.Visible, "History", "Button", GUILayout.Width(200));
            logsView.Visible = GUILayout.Toggle(logsView.Visible, "Logs", "Button", GUILayout.Width(200));

            GUILayout.Space(24);

            if (GUILayout.Button("Reset Windows", GUILayout.Width(200)))
            {
                foreach (var editorView in allViews)
                    editorView.ResetPosition();
            }

            GUILayout.FlexibleSpace();
            autoPause = GUILayout.Toggle(autoPause, "Auto Pause");

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}