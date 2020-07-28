using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace Bigmonte.Entities
{
    public class BMEntitiesAutoLoad : Node
    {
        /// Singleton pattern
        protected static BMEntitiesAutoLoad _instance;

        /// Dictionary containing Nodes and its associated Entity Controllers.
        private readonly Dictionary<Node, EntityController> _entities = new Dictionary<Node, EntityController>();

        /// List of our entities in the Tree
        private readonly List<Node> _entitiesList = new List<Node>();

        /// List of nodes to remove next frame
        private readonly List<Node> _toRemove = new List<Node>();

        /// Singleton pattern
        protected BMEntitiesAutoLoad()
        {
            _instance = this;
        }

        // Singleton pattern
        public static BMEntitiesAutoLoad Instance => _instance;

        /// <summary>
        ///     On _Ready we connect the signal "node_added" and we do the Initial scan.
        /// </summary>
        public override void _Ready()
        {
            _instance = this;

            GetTree().Connect("node_added", this, nameof(OnNodeAdded));
            InitialScan();
        }

        /// <summary>
        ///     Mark the node for deletion in the next frame.
        /// </summary>
        public void DeleteNode(Node node)
        {
            if (_entities.ContainsKey(node))
            {
                MarkForDeletion(node);

                if (!_toRemove.Contains(node)) _toRemove.Add(node);

                return;
            }

            node.QueueFree();
        }

        /// <summary>
        ///     Check if the Node is active.
        ///     Return null if the node is invalid.
        /// </summary>
        public bool CheckIfNodeIsActive(Node c)
        {
            var p = c.GetType().GetProperty("Visible");
            return p.GetValue(c) is bool && (bool) p.GetValue(c);
        }


        /// <summary>
        ///     On _Process, we update every frame all of our nodes in the main thread.
        ///     In the end of the call we destroy nodes marked for deletion.
        /// </summary>
        public override void _Process(float delta)
        {
            Time.time += delta;
            Time.deltaTime = delta;


            for (var i = 0; i < _entitiesList.Count; i++)
            {
                _entities[_entitiesList[i]].Process();
                _entities[_entitiesList[i]].LateUpdate();
            }

            if (_toRemove.Count == 0) return;

            for (var i = 0; i < _toRemove.Count; i++) DestroyEntity(_toRemove[i]);
        }


        /// <summary>
        ///     On _PhysicsProcess, we run all of our physics process related nodes every physics frame
        /// </summary>
        public override void _PhysicsProcess(float delta)
        {
            try
            {
                Time.fixedDeltaTime = delta;

                for (var i = 0; i < _entitiesList.Count; i++)
                {
                    var n = _entitiesList[i];
                    _entities[n].FixedUpdate();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }


        /// <summary>
        ///     This method calls the recursive method CheckNode starting in the root node.
        /// </summary>
        private void InitialScan()
        {
            CheckNode(GetTree().Root);
        }

        /// <summary>
        ///     Check if the node and its potential child's have a controller associated to them.
        ///     If they don't have we associate one, and evoke the Awake method
        /// </summary>
        private void CheckNode(Node currentNode)
        {
            var attr = currentNode.GetType().GetCustomAttribute<Entity>();

            if (attr != null && !_entities.ContainsKey(currentNode))
            {
                _entitiesList.Add(currentNode);
                _entities[currentNode] = new EntityController(currentNode);
                _entities[currentNode].Awake();
            }


            foreach (Node n in currentNode.GetChildren()) CheckNode(n);
        }

        /// <summary>
        ///     The method associated to the signal that is triggered when a node is added to the tree.
        /// </summary>
        private void OnNodeAdded(Node node)
        {
            CheckNode(node);
        }

        /// <summary>
        ///     Get the Controller of a specific node.
        /// </summary>
        internal EntityController GetEntityController(Node node)
        {
            return _entities.ContainsKey(node) ? _entities[node] : null;
        }

        /// <summary>
        ///     Set the Visibility for node and its children's.
        /// </summary>
        public void SetActiveVisibility(Node node, bool visible)
        {
            if (!_entities.ContainsKey(node))
            {
                RefreshVisibilityAttribute(visible, node);
                UpdateChildrensVisibility(node, visible);
                return;
            }

            _entities[node].ActivateNode(visible);
            UpdateChildrensVisibility(node, visible);
        }

        /// <summary>
        ///     Set the Visibility for a solo node.
        /// </summary>
        public void SetActiveSoloVisibility(Node node, bool visible)
        {
            if (!_entities.ContainsKey(node))
            {
                RefreshVisibilityAttribute(visible, node);
                return;
            }

            _entities[node].ActivateNode(visible);
        }

        public T[] FindObjects<T>() where T : Node
        {
            var components = new List<T>();
            foreach (var node in _entitiesList)
                if (node is T)
                    components.Add(node as T);

            return components.ToArray();
        }

        /// <summary>
        ///     Recursive call that updates children's visibility and potential child's of the children's.
        /// </summary>
        private void UpdateChildrensVisibility(Node node, bool visible)
        {
            var inspect = node;

            foreach (Node c in inspect.GetChildren())
            {
                if (!_entities.ContainsKey(c))
                {
                    RefreshVisibilityAttribute(visible, c);
                    continue;
                }

                _entities[c].ActivateNode(visible);
                UpdateChildrensVisibility(c, visible);
            }
        }

        /// <summary>
        ///     Set virtual or real visibility for the node.
        ///     Note that only Control, Spatial and Canvas nodes have real visibilities.
        /// </summary>
        private static void RefreshVisibilityAttribute(bool visible, Node c)
        {
            var visibleProperty = c.GetType().GetProperty("Visible");

            if (visibleProperty != null) visibleProperty.SetValue(c, visible);
        }

        /// <summary>
        ///     Destroy a potential Entity.
        /// </summary>
        private void DestroyEntity(Node node)
        {
            _entitiesList.Remove(node);


            _entities[node].ActivateNode(false);
            _entities.Remove(node);
            _toRemove.Remove(node);
            node.Free();
        }

        /// <summary>
        ///     Mark a Entity to be deleted in the end of the Process call.
        /// </summary>
        private void MarkForDeletion(Node node)
        {
            var inspect = node;

            var cs = inspect.GetChildren();

            for (var i = 0; i < cs.Count; i++)
            {
                var c = cs[i] as Node;

                if (_entities.ContainsKey(c)) _toRemove.Add(c);
            }
        }
    }
}