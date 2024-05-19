using UnityEngine;

namespace Cr7Sund
{
    public static class GameObjectUtil
    {
        public static void Destroy(GameObject gameObject)
        {
            if (!Application.isPlaying)
            {
                GameObject.DestroyImmediate(gameObject);
            }
            else
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}