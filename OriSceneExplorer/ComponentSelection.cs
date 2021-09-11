using UnityEngine;

namespace OriSceneExplorer
{
    public class ComponentSelection
    {
        private static Component selection;
        public static Component Selection
        {
            get
            {
                if (!selection)
                    return null;
                return selection;
            }
            set
            {
                selection = value;
            }
        }
    }
}
