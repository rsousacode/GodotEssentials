using System.Collections;
using System.Collections.Generic;
using Godot;

namespace Bigmonte.Entities
{
    public static class ComponentExtensions
    {
        public static T AddComponent<T>(this Node node) where T : Node, new()
        {
            var newNode = new T();

            node.AddChild(newNode);

            return newNode;
        }

        public static T GetComponent<T>(this Node node) where T : Node
        {
            return node as T;
        }

        public static T GetComponentInParent<T>(this Node node) where T : Node
        {
            var currentNode = node.GetParent();
            do
            {
                if (currentNode is T node1) return node1;

                var child = currentNode.GetComponentInChildren<T>();
                if (child is T c) return c;
                currentNode = currentNode.GetParent();
            } while (currentNode != null);

            return null;
        }


        private static T GetComponentInParentFromIndex<T>(this Node node, int i) where T : Node
        {
            var parent = node.GetParent();

            while (i > 0)
            {
                i--;
                parent = parent.GetParent();
                if (parent is T returnNode && i == 0) return returnNode;
            }

            if (parent is T returnParent) return returnParent;
            return null;
        }


        public static T[] GetComponentsInParent<T>(this Node node) where T : Node
        {
            var components = new List<T>();

            foreach (var n in node.Owner.GetComponentsInChildren<T>()) components.Add(n);

            return components.ToArray();
        }

        public static T[] FindObjectsOfType<T>(this Node node) where T : Node
        {
            return BMEntitiesAutoLoad.Instance.FindObjects<T>();
        }

        public static T GetComponentInChildren<T>(this Node node) where T : Node
        {
            return FindChild<T>(node);
        }

        private static T FindChild<T>(Node parent) where T : Node
        {
            var childCount = parent.GetChildCount();

            if (parent is T node1) return node1;

            if (childCount <= 0) return null;
            for (var i = 0; i < childCount; i++)
            {
                var node = FindChild<T>(parent.GetChild(i));

                if (node != null) return node;
            }

            return null;
        }


        /// <summary>
        ///     Set the visibility of the object and its children's
        /// </summary>
        /// <param name="node"></param>
        /// <param name="status"></param>
        public static void SetActive(this Node node, bool status)
        {
            BMEntitiesAutoLoad.Instance.SetActiveVisibility(node, status);
        }

        /// <summary>
        ///     Set the visibility of the object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="status"></param>
        public static void SetActiveSolo(this Node node, bool status)
        {
            BMEntitiesAutoLoad.Instance.SetActiveSoloVisibility(node, status);
        }

        public static void SetModulateAlpha(this CanvasItem node, float alphaToSet)
        {
            var m = node.Modulate;
            m.a = alphaToSet;
            node.Modulate = m;
        }


        public static bool IsActive(this Node node)
        {
            return BMEntitiesAutoLoad.Instance.CheckIfNodeIsActive(node);
        }


        public static T[] GetComponentsInChildren<T>(this Node node) where T : Node
        {
            var components = new List<T>();
            CollectChildComponents(node, components);
            return components.ToArray();
        }

        public static T[] GetComponentsInChildren<T>(this Viewport viewport) where T : Node
        {
            var components = new List<T>();
            CollectChildComponents(viewport, components);
            return components.ToArray();
        }

        private static void CollectChildComponents<T>(Node parent, List<T> components) where T : Node
        {
            var childCount = parent.GetChildCount();

            if (parent is T node) components.Add(node);

            if (childCount > 0)
                for (var i = 0; i < childCount; i++)
                    CollectChildComponents(parent.GetChild(i), components);
        }

        private static void CollectParentComponents<T>(Node n, List<T> components) where T : Node
        {
            var childCount = n.GetTree().Root.GetChildCount();

            if (n is T node) components.Add(node);

            if (childCount > 0)
                for (var i = 0; i < childCount; i++)
                    CollectParentComponents(n.GetComponentInParentFromIndex<T>(i), components);
        }


        public static void Destroy(this Node node)
        {
            BMEntitiesAutoLoad.Instance.DeleteNode(node);
        }


        public static void StartCoroutine(this Node node, IEnumerator routine)
        {
            BMEntitiesAutoLoad.Instance.GetEntityController(node)?.AddCoroutine(routine);
        }
    }
}