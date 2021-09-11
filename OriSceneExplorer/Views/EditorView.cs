using UnityEngine;

namespace OriSceneExplorer
{
    public abstract class EditorView
    {
        protected Rect windowRect;
        private readonly Rect initialPosition;
        public string Title { get; set; }
        public bool Visible { get; set; } = true;
        public bool Draggable { get; set; } = true;

        protected readonly int index = 0;
        static int nextIndex = 0;

        private const float GridCells = 12;
        protected EditorView(int col, int row, int width, int height, string title)
        {
            windowRect = new Rect(
                x: (col / GridCells) * Screen.width + 10,
                y: (row / GridCells) * Screen.height + 10,
                width: (width / GridCells) * Screen.width - 20,
                height: (height / GridCells) * Screen.height - 20
            );

            // Give space for the bottom toolbar
            if (row + height == GridCells)
                windowRect.height -= 12;

            initialPosition = windowRect;
            index = nextIndex++;
            Title = title;
        }

        protected abstract void Draw(int windowID);

        private void WindowFunc(int windowID)
        {
            if (Draggable)
                GUI.DragWindow(new Rect(0, 0, windowRect.width, 16));

            // TODO make this darker
            var c = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0, 0, 0, 1f);
            GUI.Box(new Rect(0, 16, windowRect.width, windowRect.height - 16), "");
            GUI.backgroundColor = c;

            Draw(windowID);
        }

        public void OnGUI()
        {
            if (!Visible)
                return;

            windowRect = GUI.Window(index, windowRect, WindowFunc, Title);
        }

        public void ResetPosition()
        {
            windowRect = initialPosition;
        }
    }
}