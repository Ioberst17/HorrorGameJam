using UnityEngine;

public static class SiblingComponentUtils
{
    public static T AddSiblingComponent<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject == null || gameObject.transform.parent == null)
        {
            return null;
        }

        Transform parent = gameObject.transform.parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform sibling = parent.GetChild(i);
            if (sibling.gameObject != gameObject)
            {
                T siblingComponent = sibling.GetComponent<T>();
                if (siblingComponent != null)
                {
                    return siblingComponent;
                }
            }
        }

        GameObject newSibling = new GameObject();
        newSibling.transform.SetParent(parent);
        T newSiblingComponent = newSibling.AddComponent<T>();

        return newSiblingComponent;
    }

    public static T GetSiblingComponent<T>(this GameObject gameObject) where T : Component
    {
        if (gameObject == null || gameObject.transform.parent == null)
        {
            return null;
        }

        Transform parent = gameObject.transform.parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform sibling = parent.GetChild(i);
            if (sibling.gameObject != gameObject)
            {
                T siblingComponent = sibling.GetComponent<T>();
                if (siblingComponent != null)
                {
                    return siblingComponent;
                }
            }
        }

        return null;
    }
}