using UnityEngine;

namespace OriSceneExplorer
{
    public class Editor : MonoBehaviour
    {
        public static bool ready = false;
        bool pause = false;
        bool visible = true;

        HierarchyView hierarchyView = new HierarchyView(0, 0, 4, 12);
        ComponentsView componentsView = new ComponentsView(4, 0, 4, 12);
        LogView logsView = new LogView(8, 6, 4, 6);

        public void Start()
        {
            hierarchyView.OnTargetGameObject += componentsView.Load;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
                visible = !visible;

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                pause = !pause;
                if (pause)
                    SuspensionManager.SuspendAll();
                else
                    SuspensionManager.ResumeAll();
            }

            if (Input.GetKeyDown(KeyCode.Backslash))
                hierarchyView.Refresh();

            if (Input.GetKeyDown(KeyCode.Delete))
                logsView.ClearLogs();
        }

        public void OnGUI()
        {
            if (!visible)
                return;

            hierarchyView.OnGUI();
            componentsView.OnGUI();
            logsView.OnGUI();
        }
    }
}