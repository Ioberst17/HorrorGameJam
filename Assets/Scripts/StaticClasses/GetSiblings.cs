using UnityEngine;

public static class SiblingComponentUtils
{
    public static T AddSiblingComponent<T>(this Component component) where T : Component
    {
        if (component == null || component.transform.parent == null)
        {
            return null;
        }

        Transform parent = component.transform.parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform sibling = parent.GetChild(i);
            if (sibling != component.transform)
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

    public static T GetSiblingComponent<T>(this Component component) where T : Component
    {
        if (component == null || component.transform.parent == null)
        {
            return null;
        }

        Transform parent = component.transform.parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform sibling = parent.GetChild(i);
            if (sibling != component.transform)
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
