using UnityEngine;

namespace OriSceneExplorer.Inspector.PropertyEditors.Fields
{
    public class Vector3Field
    {
        FloatField x, y, z;
        public Vector3Field(Vector3 value)
        {
            x = new FloatField(value.x);
            y = new FloatField(value.y);
            z = new FloatField(value.z);
        }

        public Vector3 Draw(Vector3 value)
        {
            Vector3 v;
            v.x = x.Draw(value.x);
            v.y = y.Draw(value.y);
            v.z = z.Draw(value.z);
            return v;
        }
    }
}
