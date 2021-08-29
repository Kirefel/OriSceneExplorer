using UnityEngine;

namespace OriSceneExplorer
{
    public abstract class EditorView
    {
        Rect windowRect;
        public string Title { get; set; }

        protected readonly int index = 0;
        static int nextIndex = 0;

        protected EditorView(Rect windowRect, string title)
        {
            this.windowRect = windowRect;
            this.index = nextIndex++;
            Title = title;
        }

        protected abstract void Draw(int windowID);

        public void OnGUI()
        {
            windowRect = GUI.Window(index, windowRect, Draw, Title);
        }
    }
}