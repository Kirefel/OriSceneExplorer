using UnityEngine;

namespace OriSceneExplorer
{
    public class ContextMenuRenderer : MonoBehaviour
    {
        public void OnGUI()
        {
            ContextMenu.OnGUI();
        }

        public void Update()
        {
            ContextMenu.Update();
        }
    }
}
