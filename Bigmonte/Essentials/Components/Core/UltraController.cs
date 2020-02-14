using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace Bigmonte.Essentials
{
    public class UltraController
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
        private bool _prevVisibility ;


        private bool _startCalled;
        private MethodInfo _startMethod;
        private MethodInfo _startMethodCr;
        private MethodInfo _processMethod;

        private VisibilityHandler _visibilityHandler;


        public UltraController(Node node)
        {
            _referencedNode = node;
        }

        private IEnumerator r => (IEnumerator) _startMethodCr.Invoke(_referencedNode, null);

        private MethodInfo FindMethod(string methodName, Type returnType, Type type = null)
        {
            type = type == null ? _referencedNode.GetType() : type;
            var method = type.GetMethod(methodName, BindingFlags);

            if (method == null)
                return type == typeof(Node) || type.BaseType == null
                    ? null
                    : FindMethod(methodName, returnType, type.BaseType);
            if (method.GetParameters().Length != 0)
                return type == typeof(Node) || type.BaseType == null
                    ? null
                    : FindMethod(methodName, returnType, type.BaseType);
            if (method.ReturnType == returnType) return method;

            return type == typeof(Node) || type.BaseType == null
                ? null
                : FindMethod(methodName, returnType, type.BaseType);
        }


        public void Awake()
        {
            SetupUltraController();
        }


        private void SetupUltraController()
        {
            InitUltraController();

            if (_awakeMethod != null) _awakeMethod.Invoke(_referencedNode, null);
        }


        private void InitUltraController()
        {
            switch (_referencedNode)
            {
                case Spatial node:
                {
                    Spatial spatial = node;
                    _visibilityHandler = new SpatialVisibilityHandler(spatial);
                    break;
                }
                case CanvasItem item:
                {
                    CanvasItem canvasItem = item;
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


        public void Process()
        {
            try
            {
                if (!_visibilityHandler.IsVisible) return;

                if (!_startCalled)
                {
                    OnVisibilityChange();

                    _startCalled = true;


                    if (_startMethod != null)
                    {
                        var o = _startMethod.Invoke(_referencedNode, null);
                    }

                    if (_startMethodCr != null) StartCoroutine(r);

                }
                
                if (!_visibilityHandler.IsVisible) return; // Added to avoid running after deactivating node

                if (_processMethod != null)
                {
                    var o = _processMethod.Invoke(_referencedNode, null);
                }
                
                if (!_visibilityHandler.IsVisible) return;  // Added to avoid running after deactivating node


                for (var i = 0; i < _coroutines.Count; i++)
                {
                    var yielded = _coroutines[i].Current is CustomYieldInstruction yielder && yielder.MoveNext();

                    if (yielded || _coroutines[i].MoveNext()) continue;
                    _coroutines.RemoveAt(i);
                    i--;
                }
            }
            catch (Exception e)
            {
                GD.PrintErr("[-] Update Exception: " + e.Message + "\n\n" + e.Source + "\n\n" + e.StackTrace + "\n\n"  + e.InnerException + "\n\n" + e.TargetSite);
            }
        }

        public void LateUpdate()
        {
            if (!_visibilityHandler.IsVisible) return;

            if (_fixedUpdateMethod != null) _fixedUpdateMethod.Invoke(_referencedNode, null);
        }

        public void FixedUpdate()
        {
            if (!_visibilityHandler.IsVisible) return;

            if (_fixedUpdateMethod != null) _fixedUpdateMethod.Invoke(_referencedNode, null);
        }

        private Coroutine StartCoroutine(IEnumerator routine)
        {
            _coroutines.Add(routine);
            return new Coroutine(routine);
        }

        public void AddCoroutine(IEnumerator routine)
        {
            _coroutines.Add(routine);
        }

        public void ActivateNode(bool status)
        {
            if (status == _visibilityHandler.IsVisible)
            {
                return;
            }

            var visibleProperty = _referencedNode.GetType().GetProperty("Visible");

            if (visibleProperty != null)
            {
                visibleProperty.SetValue(_referencedNode, status);
            }

            _visibilityHandler.SetVisibility(status);

            OnVisibilityChange();
        }
        

        private void OnVisibilityChange()
        {
            
            var curVisibility = _visibilityHandler.IsVisible;

            if (curVisibility == _prevVisibility)
            {
                return;
            }

            if (curVisibility && _onEnabledMethod != null)
            {
                _onEnabledMethod.Invoke(_referencedNode, null);
            }
            else if (!curVisibility && _onDisabledMethod != null)
            {
                _onDisabledMethod.Invoke(_referencedNode, null);
            }

            _prevVisibility = curVisibility;
        }
    }
}