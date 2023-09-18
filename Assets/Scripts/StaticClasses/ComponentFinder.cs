using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public static class ComponentFinder
{
    // returns all components in hierarchy that are of the type and a given name
    public static T FindComponent<T>(string name) where T : Component
    {
        T[] components = Resources.FindObjectsOfTypeAll<T>();

        foreach (T component in components)
        {
            if (component.gameObject.name == name)
            {
                return component;
            }
        }

        Debug.LogWarning(typeof(T) + " with name " + name + " not found!");
        return null;
    }

    // returns a component if it exists in an object's hierrarchy (self, parents, children); give it a reference as a starting point
    public static bool CheckForComponentInObjectHierarchy<T>(GameObject targetObject) where T : class
    {
        // Check if the target object itself has the specified component
        T componentInSelf = targetObject.GetComponent<T>();
        if (componentInSelf != null)
        {
            return true;
        }

        // Check if the target object or any of its parents have the specified component
        T componentInParent = targetObject.GetComponentInParent<T>();
        if (componentInParent != null)
        {
            return true;
        }

        // Check if the target object or any of its children have the specified component
        T componentInChildren = targetObject.GetComponentInChildren<T>();
        if (componentInChildren != null)
        {
            return true;
        }

        // If no component is found in the target object or its parents or children, return false
        return false;
    }

    /// <summary>
    /// Search for a component by name and type; T is the type of component, componentName is the name of the gameObject the component is on.
    /// Optional parameters include an alternative starting GameObject, includingInactive components, and if the componentName only needs to be contained in the gameObject name (vs an exact match)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="componentName"></param>
    /// <param name="rootObject"></param>
    /// <param name="includeInactive"></param>
    /// <param name="componentNameCanBeJustAPartOfGameObjectName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static T GetComponentInChildrenByNameAndType<T>(string componentName, GameObject rootObject = null, bool includeInactive = false, bool componentNameCanBeJustAPartOfGameObjectName = false) where T : Component
    {
        if (string.IsNullOrEmpty(componentName))
        {
            throw new ArgumentNullException(nameof(componentName), "Component name cannot be null or empty.");
        }

        T[] components;
        if (rootObject == null)
        {
            components = UnityEngine.Object.FindObjectsOfType<T>(includeInactive); // gets a list of objects of the type you're looking for from the entire hierarchy
        }
        else
        {
            components = rootObject.GetComponentsInChildren<T>(includeInactive);
        }

        if (componentNameCanBeJustAPartOfGameObjectName) // do a 'contains' search 
        {
            foreach (T component in components)
            {
                if (component.name.Contains(componentName))
                {
                    return component;
                }
            }

            return null;
        }
        else // do an exact match
        {
            foreach (T component in components)
            {
                if (component.name == componentName)
                {
                    return component;
                }
            }

            return null;
        }
    }

    public static T TryGetComponentInParent<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();

        if (component == null)
        {
            Transform parent = gameObject.transform.parent;

            while (parent != null && component == null)
            {
                component = parent.GetComponent<T>();
                parent = parent.parent;
            }

            if (component == null)
            {
                Debug.Log($"Component {typeof(T).Name} not found in the parent hierarchy of {gameObject.name}");
            }
        }

        return component;
    }

    public static T TryGetComponentInChildren<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponentInChildren<T>();

        if (component == null)
        {
            Debug.Log($"Component {typeof(T).Name} not found among children of {gameObject.name}");
        }

        return component;
    }


}