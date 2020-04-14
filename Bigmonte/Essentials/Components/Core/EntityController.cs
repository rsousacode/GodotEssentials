using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace Bigmonte.Entities
{
    public class EntityController
    {
        private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Instance |
                                                  System.Reflection.BindingFlags.Public |
                                                  System.Reflection.BindingFlags.NonPublic;

        private readonly List<IEnumerator> _coroutines = new List<IEnumerator>();
        private readonly Node _referencedNode;
        private MethodInfo _awakeMethod;
        private MethodInfo _fixedUpdateMethod;
        private MethodInfo _lateUpdateMethod;
        private MethodInfo _onDisabledMethod;
        private MethodInfo _onEnabledMethod;
        private bool? _prevVisibility;
        private MethodInfo _processMethod;


        public bool _startCalled;
        private MethodInfo _startMethod;
        private MethodInfo _startMethodCr;

        private VisibilityHandler _visibilityHandler;


        public EntityController(Node node)
        {
            _referencedNode = node;
        }

        private IEnumerator r => (IEnumerator) _startMethodCr.Invoke(_referencedNode, null);

        private MethodInfo FindMethod(string methodName, Type returnType, Type type = null)
        {
            while (true)
            {
                type = type == null ? _referencedNode.GetType() : type;
                var method = type.GetMethod(methodName, BindingFlags);

                if (method == null)
                {
                    if (type == typeof(Node) || type.BaseType == null) return null;
                    type = type.BaseType;
                    continue;
                }

                if (method.GetParameters().Length != 0)
                {
                    if (type == typeof(Node) || type.BaseType == null) return null;
                    type = type.BaseType;
                    continue;
                }

                if (method.ReturnType == returnType) return method;

                if (type == typeof(Node) || type.BaseType == null) return null;
                type = type.BaseType;
            }
        }

        /// <summary>
        ///    On Awake we initialize our Entity.
        /// </summary>
        
        public void Awake()
        {
            SetupEntity();
        }

        /// <summary>
        ///    Initial setup of the entity. Triggers Awake method if the node is visible
        ///    and triggers an visibility change event. 
        /// </summary>
        
        private void SetupEntity()
        {
            InitializeEntityController();
            

            if (_awakeMethod != null) _awakeMethod.Invoke(_referencedNode, null);

            OnVisibilityChange();

            if (!_visibilityHandler.IsVisible) return;

            if (!_startCalled)
            {
                _startCalled = true;

                if (_startMethod != null)
                {
                    var o = _startMethod.Invoke(_referencedNode, null);
                }

                if (_startMethodCr != null) StartCoroutine(r);
            }
            
        }

        /// <summary>
        ///    Register visibility handler and Entity methods.
        /// </summary>
        
        private void InitializeEntityController()
        {
            switch (_referencedNode)
            {
                case Control node:
                {
                    var control = node;
                    _visibilityHandler = new ControlItemVisibilityHandler(control);
                    break;
                }
                case Spatial node:
                {
                    var spatial = node;
                    _visibilityHandler = new SpatialVisibilityHandler(spatial);
                    break;
                }
                
                case CanvasItem item:
                {
                    var canvasItem = item;
                    _visibilityHandler = new CanvasItemVisibilityHandler(canvasItem);
                    break;
                }
                default:
                    _visibilityHandler = new VisibilityHandler();
                    break;
            }

            _awakeMethod = FindMethod("Awake", typeof(void));
            _startMethod = FindMethod("Start", typeof(void));
            _startMethodCr = FindMethod("Start", typeof(IEnumerator));
            _processMethod = FindMethod("Process", typeof(void));
            _fixedUpdateMethod = FindMethod("FixedUpdate", typeof(void));
            _lateUpdateMethod = FindMethod("LateUpdate", typeof(void));
            _onEnabledMethod = FindMethod("OnEnable", typeof(void));
            _onDisabledMethod = FindMethod("OnDisable", typeof(void));
        }


        /// <summary>
        ///    Called every frame
        /// </summary>
        public void Process()
        {
            if (!_visibilityHandler.IsVisible) return;

            if (_processMethod != null)
            {
                var o = _processMethod.Invoke(_referencedNode, null);
            }

            if (!_visibilityHandler.IsVisible) return; // Added to avoid running after deactivating node

            HandleCoroutines();
        }
        
        /// <summary>
        ///    Handles virtual Coroutines
        /// </summary>

        private void HandleCoroutines()
        {
            for (var i = 0; i < _coroutines.Count; i++)
            {
                var yielded = _coroutines[i].Current is CustomYieldInstruction yielder && yielder.MoveNext();

                if (yielded || _coroutines[i].MoveNext()) continue;
                _coroutines.RemoveAt(i);
                i--;
            }
        }

        /// <summary>
        ///    Virtual LateUpdate method
        /// </summary>
        public void LateUpdate()
        {
            if (!_visibilityHandler.IsVisible) return;

            if (_lateUpdateMethod != null) _lateUpdateMethod.Invoke(_referencedNode, null);
        }

        /// <summary>
        ///    Virtual FixedUpdate method
        /// </summary>
        public void FixedUpdate()
        {
            if (!_visibilityHandler.IsVisible) return;

            if (_fixedUpdateMethod != null) _fixedUpdateMethod.Invoke(_referencedNode, null);
        }

        /// <summary>
        ///    Virtual StartCoroutine method
        /// </summary>
        
        private Coroutine StartCoroutine(IEnumerator routine)
        {
            _coroutines.Add(routine);
            return new Coroutine(routine);
        }
        
        /// <summary>
        ///    Virtual AddCoroutine method
        /// </summary>
        /// 
        public void AddCoroutine(IEnumerator routine)
        {
            _coroutines.Add(routine);
        }
        
        /// <summary>
        ///    Used to Activate the node associated to this Entity Controller and triggers a Visibility Change event.
        /// </summary>
        public void ActivateNode(bool status)
        {
            if (status == _visibilityHandler.IsVisible) return;

            var visibleProperty = _referencedNode.GetType().GetProperty("Visible");

            if (visibleProperty != null) visibleProperty.SetValue(_referencedNode, status);

            _visibilityHandler.SetVisibility(status);

            OnVisibilityChange();

        }

        /// <summary>
        ///    Trigger OnEnable and OnDisable methods regarding the current and previous visibility. 
        /// </summary>
        private void OnVisibilityChange()
        {
            var curVisibility = _visibilityHandler.IsVisible;

            if (curVisibility == _prevVisibility) return;

            if (curVisibility && _onEnabledMethod != null) _onEnabledMethod.Invoke(_referencedNode, null);

            if (!curVisibility && _onDisabledMethod != null) _onDisabledMethod.Invoke(_referencedNode, null);

            _prevVisibility = curVisibility;
            
        }
    }
}