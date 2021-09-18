using UnityEngine;
using OriSceneExplorer.Configuration;

namespace OriSceneExplorer
{
    public class Editor : MonoBehaviour
    {
        public static Editor Instance { get; private set; }
        public static bool ready = false;
        private bool pause = false;
        private bool visible = false;
        private bool autoPause = true;

        private readonly HierarchyView hierarchyView = new HierarchyView(0, 0, 4, 12);
        private readonly ComponentsView componentsView = new ComponentsView(4, 0, 5, 12);
        private readonly LogView logsView = new LogView(9, 6, 3, 6);
        private readonly HistoryView historyView = new HistoryView(9, 0, 3, 6);

        public static Settings EditorSettings { get; private set; }

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

        bool moving;
        bool rotating;
        Transform transformTarget;

        public void Awake()
        {
            Instance = this;
            gameObject.AddComponent<ContextMenuRenderer>();
        }

        public void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void Start()
        {
            allViews = new EditorView[] { hierarchyView, componentsView, logsView, historyView };

            hierarchyView.OnTargetGameObject += componentsView.Load;
            hierarchyView.OnTargetGameObject += historyView.Push;
            componentsView.OnFocusProperty += historyView.Push;
            componentsView.OnFocusProperty += obj => hierarchyView.SetSelection(obj, true);
            componentsView.OnClone += obj => hierarchyView.Refresh(obj);
            componentsView.OnStartMoving += ComponentsView_OnStartMoving;
            componentsView.OnStartRotating += ComponentsView_OnStartRotating;
            historyView.OnSelectionChange += componentsView.Load;
            historyView.OnSelectionChange += obj => hierarchyView.SetSelection(obj, true);

            EditorSettings = Settings.LoadAll();

            if (EditorSettings.EnableDebugControls)
            {
                DebugMenuB.MakeDebugMenuExist();
                DebugMenuB.DebugControlsEnabled = true;
            }
            autoPause = EditorSettings.AutoPause;
        }

        private void ComponentsView_OnStartRotating(ViewerGORef obj)
        {
            moving = false;
            rotating = true;
            transformTarget = obj.Reference.transform;
        }

        private void ComponentsView_OnStartMoving(ViewerGORef obj)
        {
            moving = true;
            rotating = false;
            transformTarget = obj.Reference.transform;
        }

        public void Update()
        {
            if (Input.GetKeyDown(EditorSettings.ToggleView))
            {
                visible = !visible;
                if (autoPause)
                    Paused = visible;

                // Refresh on first opening the window
                if (visible && hierarchyView.IsEmpty())
                    hierarchyView.Refresh();
            }

            if (Input.GetKeyDown(EditorSettings.Suspend))
                Paused = !Paused;

            if (Input.GetKeyDown(EditorSettings.RefreshHierarchy))
            {
                hierarchyView.Refresh();
                historyView.Reset();
            }

            if (Input.GetKeyDown(EditorSettings.ClearLogs))
                logsView.ClearLogs();

            if (Input.GetKeyDown(EditorSettings.PickObject))
                hierarchyView.SelectObjectUnderCursor();

            if (Input.GetMouseButtonDown(0) || !transformTarget || !transformTarget.gameObject.activeInHierarchy)
            {
                moving = false;
                rotating = false;
                transformTarget = null;
            }
            if (moving)
            {
                transformTarget.position = GetWorldPositionOnPlane(Input.mousePosition, transformTarget.position.z);
            }
            if (rotating)
            {
                var pos = GetWorldPositionOnPlane(Input.mousePosition, transformTarget.position.z);
                var delta = pos - transformTarget.position;
                float z = Mathf.Atan2(delta.y, delta.x);
                transformTarget.eulerAngles = new Vector3(transformTarget.eulerAngles.x, transformTarget.eulerAngles.y, z * Mathf.Rad2Deg);
            }
        }

        public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
            float distance;
            xy.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }

        public void OnGUI()
        {
            if (!visible || moving || rotating || ContextMenu.Open)
                return;

            GUI.depth = 2;

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

        public void NavigateToGameObject(GameObject gameObject) => componentsView.NavigateToGameObject(gameObject);
    }
}