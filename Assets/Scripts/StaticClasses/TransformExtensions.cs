using UnityEngine;

public static class TransformExtensions
{
    public static Transform GetSibling(this Transform transform, string name)
    {
        if (transform.parent == null)
        {
            return null;
        }

        Transform parent = transform.parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == name)
            {
                return child;
            }
        }

        return null;
    }
}