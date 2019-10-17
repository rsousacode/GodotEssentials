using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace Bigmonte.Essentials
{
    public class BMAutoLoad : Node
    {
        protected static BMAutoLoad _instance;

        public BMAutoLoad()
        {
            _instance = this;
        }

        public static BMAutoLoad Instance
        {
            get
            {
                return _instance;
            }
        }

        private readonly List<Node> _monoNodes = new List<Node>();

        private readonly List<Node> _monoNodesToDelete = new List<Node>();
        private readonly Dictionary<Node, UltraController> _ultras = new Dictionary<Node, UltraController>();


        public override void _Ready()
        {
            _instance = this;

            GetTree().Connect("node_added", this, "_node_added");
            InitialScan();
        }
        
        public void DeleteNode(Node node)
        {
            MarkForDeletion(node);
            if (!_ultras.ContainsKey(node)) return;
            _monoNodesToDelete.Add(node);
        }


        public void Quit()
        {
            var tree = GetTree();

            tree?.Quit();
        }

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

            _monoNodesToDelete.Clear();
        }


        public override void _PhysicsProcess(float delta)
        {
            Time.fixedDeltaTime = delta;


            foreach (var node in _monoNodes) _ultras[node].FixedUpdate();
        }


        private void InitialScan()
        {
            CheckNode(GetTree().GetRoot());
        }

        private void CheckNode(Node currentNode)
        {
            var attr = currentNode.GetType().GetCustomAttribute<Extended>();

            if (attr != null)
            {
                _monoNodes.Add(currentNode);
                _ultras[currentNode] = new UltraController(currentNode);
                _ultras[currentNode].Awake();
            }

            for (var i = 0; i < currentNode.GetChildCount(); i++) CheckNode(currentNode.GetChild(i));
        }


        private void _node_added(Node node)
        {
            CheckNode(node);
        }


        internal UltraController GetUltraController(Node node)
        {
            return _ultras.ContainsKey(node) ? _ultras[node] : null;
        }

        public void UpdateNodeVisibility(Node node, bool visible)
        {
            if (!_ultras.ContainsKey(node) )
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

            if (visibleProperty != null)
            {
                visibleProperty.SetValue(c, visible);
                
            }
        }

        private void RemoveNode(Node node)
        {
            _monoNodes.Remove(node);
            _ultras[node].ActivateNode(false);
            _ultras.Remove(node);
            node.QueueFree();
        }

        private void MarkForDeletion(Node node)
        {
            var inspect = node;

            var cs = inspect.GetChildren();
            for (var i = 0; i < cs.Count; i++)
            {
                var c = cs[i] as Node;
                if (!_ultras.ContainsKey(c)) continue;
                _monoNodesToDelete.Add(c);
                MarkForDeletion(c);
            }
        }
    }
}