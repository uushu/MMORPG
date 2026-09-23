

using UnityEngine;

namespace Assets
{
    public class Resloader
    {
        public static T Load<T>(string path) where T : UnityEngine.Object
        {
            return Resources.Load<T>(path);
        }
    }
}