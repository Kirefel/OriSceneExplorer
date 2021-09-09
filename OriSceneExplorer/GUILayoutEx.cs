using UnityEngine;

namespace OriSceneExplorer
{
    //public class EnumEditorNew : PropertyEditorNew
    //{
    //    public override bool Draw(ref object value)
    //    {

    //    }

    //    public override string FormatString(object value) => value.ToString();
    //}

    public static class GUILayoutEx
    {
        // create your style
        private static GUIStyle horizontalLine;
        static GUILayoutEx()
        {
            horizontalLine = new GUIStyle();
            //horizontalLine.margin = new RectOffset(0, 0, 4, 4);
            horizontalLine.fixedHeight = 1;
        }

        public static void HorizontalDivider()
        {
            GUILayout.Box(GUIContent.none, horizontalLine);
        }
    }
}