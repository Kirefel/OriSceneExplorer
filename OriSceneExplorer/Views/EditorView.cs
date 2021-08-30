using UnityEngine;

namespace OriSceneExplorer
{
    public abstract class EditorView
    {
        Rect windowRect;
        public string Title { get; set; }

        protected readonly int index = 0;
        static int nextIndex = 0;

        private const float GridCells = 12;
        protected EditorView(int col, int row, int width, int height, string title)
        {
            this.windowRect = new Rect(
                x: (col / GridCells) * Screen.width + 10,
                y: (row / GridCells) * Screen.height + 10,
                width: (width / GridCells) * Screen.width - 20,
                height: (height / GridCells) * Screen.height - 20
            );
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