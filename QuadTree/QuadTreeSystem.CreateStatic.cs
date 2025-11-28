using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using Betta.Framework;

namespace Merge.Graphic
{
    /// <summary>
    /// 场景中静态物体四叉树管理
    /// </summary>
    public partial class QuadTreeSystem
    {
        public QuadTreeStatic RootNodeStatic { get; set; }
        private ClassCfgController _cfg = new ClassCfgController();
        private HashSet<string> _excludedHash = new HashSet<string>()
        {
            "ElementLayer", "BubbleLayer", "Main Camera", "Input"
        };
        
        private void Register()
        {
            _cfg.Register<QuadTreeGameObject>(QuadTreeDataType.GameObject);
            _cfg.Register<QuadTreeAnimation>(QuadTreeDataType.Animation);
            _cfg.Register<QuadTreeSpine>(QuadTreeDataType.Spine);
        }
        
        private void UpdateStatic(float deltaTime)
        {
            foreach (var kv in QuadTreeStatic.VisibleDataDict)
                kv.Value.Update(deltaTime);
        }

        private void CameraChangeStatic()
        {
            RootNodeStatic.CheckCamera(_cameraRect);
            RootNodeStatic.CheckOutRect(_cameraRect);
        }
        
        private void CreateStaticTree()
        {
            Transform root = Logic.RuntimeManager.Instance.MapSystem.LandBuilder;
#if UNITY_EDITOR
            root.gameObject.AddComponent<QuadTreeGizmosStatic>();
#endif
            
            float size = 200;
            Vector2 rectMin = new Vector2(size, size) * -0.5f;
            Rect rect = new Rect(rectMin, Vector2.one * size);
            RootNodeStatic = new QuadTreeStatic();
            RootNodeStatic.Init(rect, 0, 10, 2, 6);
            
            Queue<Transform> queue = new Queue<Transform>();
            foreach (Transform child in root.transform)
            {
                if (!_excludedHash.Contains(child.name))
                {
                    queue.Enqueue(child);
                }
            }
            
            while (queue.Count > 0)
            {
                Transform current = queue.Dequeue();
                foreach (Transform child in current)
                    queue.Enqueue(child);
                
                SortingLayerRuntime layer = current.gameObject.GetComponent<SortingLayerRuntime>();
                if (null != layer)
                    layer.autoUpdate = false;
                
                QuadTreeGameObject obj = Create(current.gameObject);
                if (null != obj)
                    RootNodeStatic.Insert(obj);
            }
        }
        
        private QuadTreeGameObject Create(GameObject go)
        {
            Renderer render = go.GetComponent<Renderer>();
            if (null == render)
                return null;

            Animator animator = go.GetComponent<Animator>();
            if (null != animator)
            {
                QuadTreeAnimation quadTreeAnimation = _cfg.Create<QuadTreeAnimation>(QuadTreeDataType.Animation);
                quadTreeAnimation.SetAnimator(animator);
                quadTreeAnimation.Init(go, render);
                return quadTreeAnimation;
            }
            
            SkeletonAnimation skeletonAnimation = go.GetComponent<SkeletonAnimation>();
            if (null != skeletonAnimation)
            {
                QuadTreeSpine quadTreeSpine = _cfg.Create<QuadTreeSpine>(QuadTreeDataType.Spine);
                quadTreeSpine.SetSpine(skeletonAnimation);
                quadTreeSpine.Init(go, render);
                return quadTreeSpine;
            }

            // QuadTreeGameObject quadTreeGameObject = _cfg.Create<QuadTreeGameObject>(QuadTreeDataType.GameObject);
            // quadTreeGameObject.Init(go, render);
            // return quadTreeGameObject;
            return null;
        }

        private void ReleaseStatic()
        {
            RootNodeStatic.Release();
        }
    }
}