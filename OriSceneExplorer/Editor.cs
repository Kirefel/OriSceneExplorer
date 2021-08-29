using UnityEngine;

namespace OriSceneExplorer
{
    public class Editor : MonoBehaviour
    {
        public static bool ready = false;
        bool pause = false;
        bool visible = true;

        HierarchyView hierarchyView = new HierarchyView(new Rect(10, 10, Screen.width / 3, Screen.height - 20));
        ComponentsView componentsView = new ComponentsView(new Rect(Screen.width / 3 + 20, 10, Screen.width / 3, Screen.height - 20));
        LogView logsView = new LogView(new Rect(2 * Screen.width / 3 + 30, Screen.height / 2 + 20, Screen.width / 3 - 40, Screen.height / 2 - 40));

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