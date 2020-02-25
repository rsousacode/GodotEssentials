using System.Collections;
using System.Collections.Generic;
using Godot;

namespace Bigmonte.Essentials
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
            Node currentNode = node.GetParent();
            do
            {
                if (currentNode is T)
                {
                    return currentNode as T;
                }
                currentNode = currentNode.GetComponentInChildren<T>();
                
                if (currentNode != null)
                {
                    return currentNode as T;
                }

                currentNode = currentNode.GetParent();

            } while (currentNode != null);

            return null;
        }
        
        public static T[] GetComponentsInParent<T>(this Node node) where T : Node
        {
            var components = new List<T>();

            do
            {
                if (node is T toAdd) components.Add(toAdd);

                node = node.GetParent();
            } while (node != null);

            return components.ToArray();
        }

        public static T[] FindObjectsOfType<T>(this Node node) where T : Node
        {
            var components = new List<T>();

            var cs = node.GetComponentsInChildren<T>();

            var ps = GetComponentsInParent<T>(node);

            for (var i = 0; i < cs.Length; i++)
            {
                var c = cs[i];
                components.Add(c);
            }

            for (var i = 0; i < ps.Length; i++)
            {
                var p = ps[i];
                components.Add(p);
            }

            return components.ToArray();
        }
        
        public static T GetComponentInChildren<T>(this Node node) where T : Node
        {
            return FindChild<T>(node);
        }

        private static T FindChild<T>(Node parent) where T : Node
        {
            var childCount = parent.GetChildCount();

            if (parent is T) return parent as T;

            if (childCount <= 0) return null;
            for (var i = 0; i < childCount; i++)
            {
                var node = FindChild<T>(parent.GetChild(i));

                if (node != null) return node;
            }

            return null;
        }


        /// <summary>
        ///     Set the visibility of the object
        ///     Is mandatory to use this method since it triggers the OnEnable and OnDisable events
        /// </summary>
        /// <param name="node"></param>
        /// <param name="status"></param>
        public static void SetActive(this Node node, bool status)
        {
            BMAutoLoad.Instance.UpdateNodeVisibility(node, status);
        }

        public static bool IsActive(this Node node)
        {
            return BMAutoLoad.Instance.CheckIfNodeIsActive(node);
        }


        /// <summary>
        ///     Godot usually instantiates objects at the left origin
        ///     This function makes the node appearing in the center of its borders.
        /// </summary>
        /// <param name="relativePosition">A position to be added</param>
        public static void FixPosition(this Panel panel, Vector2 relativePosition)
        {
            var size = panel.RectSize;
            var newPos = new Vector2(panel.RectPosition.x - size.x / 2, panel.RectPosition.y - size.y / 2) +
                         relativePosition;
            panel.SetPosition(newPos);
        }

        public static Node PackedSceneInstance(string path)
        {
            var p = GD.Load(path) as PackedScene;
            return p?.Instance();
        }


        public static void FixPosition(this Sprite panel, Vector2 relativePosition)
        {
            var size = panel.Texture.GetSize();
            var newPos = new Vector2(panel.Position.x - size.x / 2, panel.Position.y - size.y / 2) +
                         relativePosition;
            panel.Position = newPos;
        }

        public static T[] GetComponentsInChildren<T>(this Node node) where T : Node
        {
            var components = new List<T>();
            CollectChildComponents(node, components);
            return components.ToArray();
        }


        private static void CollectChildComponents<T>(Node parent, List<T> components) where T : Node
        {
            var childCount = parent.GetChildCount();

            if (parent is T) components.Add(parent as T);

            if (childCount > 0)
                for (var i = 0; i < childCount; i++)
                    CollectChildComponents(parent.GetChild(i), components);
        }

        public static void Destroy(this Node node)
        {
            BMAutoLoad.Instance.DeleteNode(node);
        }


        public static Coroutine StartCoroutine(this Node node, IEnumerator routine)
        {
            BMAutoLoad.Instance.GetUltraController(node)?.AddCoroutine(routine);
            return new Coroutine(routine);
        }


        public static void RotateEuler(this Spatial spatial, Vector3 eulerAngles)
        {
            spatial.Rotate(eulerAngles.Normalized(), eulerAngles.Length() * MathTools.Deg2Rad);
        }

        /// <summary>
        ///   <para>Projects a vector onto another vector.</para>
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="onNormal"></param>
        ///
                
        public static Vector3 ProjectOnNormal(Vector3 vector, Vector3 onNormal)
        {
            float num = onNormal.Dot(onNormal);
            return (double) num < (double) Mathf.Epsilon ? Vector3.Zero : onNormal * vector.Dot( onNormal) / num;
        }

        public static float Center(this CapsuleShape capsuleShape)
        {
            return capsuleShape.Height * 0.5f;
        }
        
        public static float Center(this BoxShape boxShape)
        {
            return boxShape.Extents.x * 0.5f;
        }
        


    }
}