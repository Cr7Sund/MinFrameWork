using UnityEngine;

namespace Cr7Sund
{
    public static class GameObjectUtil
    {
        public static void Destroy(UnityEngine.Object gameObject)
        {
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(gameObject);
            }
            else
            {
                Object.Destroy(gameObject);
            }
        }

    }
}