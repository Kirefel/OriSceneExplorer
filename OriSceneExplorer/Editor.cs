using UnityEngine;

namespace OriSceneExplorer
{
    public class Editor : MonoBehaviour
    {
        public static bool ready = false;
        private bool pause = false;
        private bool visible = true;

        private readonly HierarchyView hierarchyView = new HierarchyView(0, 0, 4, 12);
        private readonly ComponentsView componentsView = new ComponentsView(4, 0, 4, 12);
        private readonly LogView logsView = new LogView(8, 6, 4, 6);
        private readonly HistoryView historyView = new HistoryView(8, 0, 4, 6);

        public void Start()
        {
            hierarchyView.OnTargetGameObject += componentsView.Load;
            hierarchyView.OnTargetGameObject += historyView.Push;
            historyView.OnSelectionChange += componentsView.Load;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                visible = !visible;

            if (Input.GetKeyDown(KeyCode.F2))
            {
                pause = !pause;
                if (pause)
                    SuspensionManager.SuspendAll();
                else
                    SuspensionManager.ResumeAll();
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                hierarchyView.Refresh();
                historyView.Reset();
            }

            if (Input.GetKeyDown(KeyCode.F4))
                logsView.ClearLogs();
        }

        public void OnGUI()
        {
            if (!visible)
                return;

            hierarchyView.OnGUI();
            componentsView.OnGUI();
            logsView.OnGUI();
            historyView.OnGUI();

            Dispatch.Execute();
        }
    }
}