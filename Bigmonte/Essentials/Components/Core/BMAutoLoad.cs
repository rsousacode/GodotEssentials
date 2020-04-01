using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace Bigmonte.Essentials
{
    public class BMAutoLoad : Node
    {
        protected static BMAutoLoad _instance;

        private readonly Dictionary<int, Node> _monoNodes = new Dictionary<int, Node>();

        private readonly List<Node> _monoNodesToDelete = new List<Node>();
        private readonly Dictionary<Node, UltraController> _ultras = new Dictionary<Node, UltraController>();

        public BMAutoLoad()
        {
            _instance = this;
        }

        public static BMAutoLoad Instance => _instance;


        public override void _Ready()
        {
            _instance = this;

            GetTree().Connect("node_added", this, nameof(OnNodeAdded));
            InitialScan();
        }

        public bool DeleteNode(Node node)
        {
            if (_ultras.ContainsKey(node))
            {
                MarkForDeletion(node);

                if (!_monoNodesToDelete.Contains(node))
                {
                    _monoNodesToDelete.Add(node);
                }

                return true;
            }

            return false;
        }

        // Returns false if invalid
        public bool CheckIfNodeIsActive(Node c)
        {
            var p = c.GetType().GetProperty("Visible");
            if (p != null) return p.GetValue(c) is bool && (bool) p.GetValue(c);

            return false;
        }


        public void Quit()
        {
            var tree = GetTree();

            tree?.Quit();
        }

        /// <summary>
        ///    On _Process, we update every frame all of our nodes in the main thread
        /// </summary>
        public override void _Process(float delta)
        {
            Time.time += delta;
            Time.deltaTime = delta;


            for (var i = 0; i < _monoNodes.Count; i++)
            {
                _ultras[_monoNodes[i]].Process();
                _ultras[_monoNodes[i]].LateUpdate();
            }

            if (_monoNodesToDelete.Count == 0) return;

            for (var i = 0; i < _monoNodesToDelete.Count; i++) RemoveNode(_monoNodesToDelete[i]);
        }


        /// <summary>
        ///    On _PhysicsProcess, we run all of our physics process related nodes every physics frame
        /// </summary>
        public override void _PhysicsProcess(float delta)
        {
            Time.fixedDeltaTime = delta;

            for (var i = 0; i < _monoNodes.Count; i++)
            {
                var n = _monoNodes[i];
                _ultras[n].FixedUpdate();
            }
        }


        private void InitialScan()
        {
            CheckNode(GetTree().Root);
        }

        private void CheckNode(Node currentNode)
        {
            var attr = currentNode.GetType().GetCustomAttribute<Extended>();

            if (attr != null && !_ultras.ContainsKey(currentNode))
            {
                _monoNodes[_monoNodes.Count] = currentNode;
                _ultras[currentNode] = new UltraController(currentNode);
                _ultras[currentNode].Awake();
            }


            foreach (Node n in currentNode.GetChildren())
            {
                CheckNode(n);
            }
        }

        public bool InUltras(Node node)
        {
            return _ultras.ContainsKey(node);
        }


        private void OnNodeAdded(Node node)
        {
            CheckNode(node);
        }


        internal UltraController GetUltraController(Node node)
        {
            return _ultras.ContainsKey(node) ? _ultras[node] : null;
        }

        public void SetActiveVisibility(Node node, bool visible)
        {
            if (!_ultras.ContainsKey(node))
            {
                RefreshVisibilityAttribute(visible, node);
                UpdateChildrensVisibility(node, visible);
                return;
            }

            _ultras[node].ActivateNode(visible);
            UpdateChildrensVisibility(node, visible);
        }

        private void UpdateChildrensVisibility(Node node, bool visible)
        {
            var inspect = node;

            foreach (Node c in inspect.GetChildren())
            {
                if (!_ultras.ContainsKey(c))
                {
                    RefreshVisibilityAttribute(visible, c);
                    continue;
                }

                _ultras[c].ActivateNode(visible);
                UpdateChildrensVisibility(c, visible);
            }
        }

        private static void RefreshVisibilityAttribute(bool visible, Node c)
        {
            var visibleProperty = c.GetType().GetProperty("Visible");

            if (visibleProperty != null) visibleProperty.SetValue(c, visible);
        }

        private void RemoveNode(Node node)
        {
            if (_monoNodes.ContainsValue(node))
            {
                var GetKey = _monoNodes.FirstOrDefault(x => x.Value == node).Key;
                _monoNodes.Remove(GetKey);
            }

            _ultras[node].ActivateNode(false);
            _ultras.Remove(node);
            _monoNodesToDelete.Remove(node);
            node.Free();

        }

        private void MarkForDeletion(Node node)
        {
            var inspect = node;

            var cs = inspect.GetChildren();

            for (var i = 0; i < cs.Count; i++)
            {
                var c = cs[i] as Node;
                
                if (_ultras.ContainsKey(c))
                {
                    _monoNodesToDelete.Add(c);
                }
            }
        }
    }
}