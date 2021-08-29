using System;
using System.Collections.Generic;
using UnityEngine;

namespace OriSceneExplorer
{
    public class ViewerGORef
    {
        public string Name { get; set; }
        public bool IsScene { get; set; }
        public List<ViewerGORef> Children { get; set; }
        public bool Expanded { get; set; }
        public int Depth { get; set; }

        private WeakReference reference;
        public GameObject Reference
        {
            get
            {
                if (reference == null || !reference.IsAlive)
                    return null;
                return reference.Target as GameObject;
            }
            set
            {
                reference = new WeakReference(value);
            }
        }

        public ViewerGORef()
        {
            Children = new List<ViewerGORef>();
            Name = "UNNAMED";
            IsScene = false;
            Expanded = false;
            Depth = 0;
        }

        public string Label
        {
            get
            {
                if (Children.Count == 0)
                    return Name;

                return $"{(Expanded ? "-" : "+")} {Name}";
            }
        }

        public Color Colour
        {
            get
            {
                var r = Reference;
                if (r == null)
                    return Color.white;

                if (!r.activeInHierarchy)
                    return Color.gray;

                return Color.white;
            }
        }
    }
}