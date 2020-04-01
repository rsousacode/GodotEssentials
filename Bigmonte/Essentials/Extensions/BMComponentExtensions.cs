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
        
        
        public static T GetComponentInParentFromIndex<T>(this Node node, int i) where T : Node
        {
            Node parent = node.GetParent();
            
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

            foreach (var n in node.Owner.GetComponentsInChildren<T>())
            {
               components.Add(n);
            }

            return components.ToArray();
        }
        
       // public static void SetSize(Control rectTransform, Vector2 newSize) 
        //{
            //Vector2 currSize = rectTransform.rect.size;
            //Vector2 sizeDiff = newSize - currSize;
            //rectTransform.RectPivotOffset = rectTransform.offsetMin - new Vector2(sizeDiff.x * rectTransform.pivot.x, sizeDiff.y * rectTransform.pivot.y);
            //rectTransform.offsetMax = rectTransform.offsetMax + new Vector2(sizeDiff.x * (1.0f - rectTransform.pivot.x), sizeDiff.y * (1.0f - rectTransform.pivot.y));
        //}

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
        ///     Set the visibility of the object
        ///     Is mandatory to use this method since it triggers the OnEnable and OnDisable events
        /// </summary>
        /// <param name="node"></param>
        /// <param name="status"></param>
        public static void SetActive(this Node node, bool status)
        {
            BMAutoLoad.Instance.SetActiveVisibility(node, status);
        }

        public static void SetModulateAlpha(this Control node, float alphaToSet)
        {
            Color m = node.Modulate;
            m.a = alphaToSet;
            node.Modulate = m;

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
        
        // public static T[] GetComponentsInParent<T>(this Node node) where T : Node
        // {
        //     var components = new List<T>();
        //     int treeChildCount = node.GetTree().Root.GetChildCount();
        //     int childCount = node.GetChildCount();
        //
        //     var count = treeChildCount - childCount;
        //
        //     if (node is T n1) components.Add(n1);
        //
        //     if (count > 0)
        //         for (var i = 0; i < childCount; i++)
        //             CollectChildComponents(node.GetComponentInParentFromIndex<T>(i), components);
        //     return new T[0]; // TODO FIX
        // }

        
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
         private static void CollectParentComponents<T>( Node n, List<T> components) where T : Node
        {
            var childCount = n.GetTree().Root.GetChildCount();

            if (n is T node) components.Add(node);

            if (childCount > 0)
                for (var i = 0; i < childCount; i++)
                    CollectParentComponents(n.GetComponentInParentFromIndex<T>(i) , components);
        }
        

        public static bool Destroy(this Node node)
        {
            return BMAutoLoad.Instance.DeleteNode(node);
        }
        
        public static bool InUltras(this Node node)
        {
            return BMAutoLoad.Instance.InUltras(node);
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
            var num = onNormal.Dot(onNormal);
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
   
        
        public static Quat Lerp(this Quat quaternion1, Quat quaternion2, float amount)
        {
            float num = amount;
            float num2 = 1f - num;
            Quat quaternion = new Quat();
            float num5 = (((quaternion1.x * quaternion2.x) + (quaternion1.y * quaternion2.y)) + (quaternion1.z * quaternion2.z)) + (quaternion1.w * quaternion2.w);
            if (num5 >= 0f)
            {
                quaternion.x = (num2 * quaternion1.x) + (num * quaternion2.x);
                quaternion.y = (num2 * quaternion1.y) + (num * quaternion2.y);
                quaternion.z = (num2 * quaternion1.z) + (num * quaternion2.z);
                quaternion.w = (num2 * quaternion1.w) + (num * quaternion2.w);
            }
            else
            {
                quaternion.x = (num2 * quaternion1.x) - (num * quaternion2.x);
                quaternion.y = (num2 * quaternion1.y) - (num * quaternion2.y);
                quaternion.z = (num2 * quaternion1.z) - (num * quaternion2.z);
                quaternion.w = (num2 * quaternion1.w) - (num * quaternion2.w);
            }
            float num4 = (((quaternion.x * quaternion.x) + (quaternion.y * quaternion.y)) + (quaternion.z * quaternion.z)) + (quaternion.w * quaternion.w);
            float num3 = 1f / ((float) MathTools.Sqrt((double) num4));
            quaternion.x *= num3;
            quaternion.y *= num3;
            quaternion.z *= num3;
            quaternion.w *= num3;
            return quaternion;
        }
    }
}