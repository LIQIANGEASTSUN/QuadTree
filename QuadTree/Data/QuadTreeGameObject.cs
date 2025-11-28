using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Merge.Graphic
{
    /// <summary>
    /// 节点数据，具体实现类
    /// 存储 GameObject
    /// </summary>
    public class QuadTreeGameObject : IQuadTreeData
    {
        public Transform Owner { get; private set; }
        private Rect _rect;
        public Rect Rect => _rect;
        public bool IsVisible { get; set; }
        protected SpriteSkin spriteSkin;
        
        public QuadTreeGameObject() {}

        public virtual void Init(GameObject go, Renderer renderer)
        {
            Owner = go.transform;
            Bounds bounds = renderer.bounds;
            Vector2 min = new Vector2(bounds.min.x, bounds.min.y);
            Vector2 size = new Vector2(bounds.size.x, bounds.size.y);
            _rect = new Rect(min, size);
            
            spriteSkin = go.GetComponent<SpriteSkin>();
        }

        public bool IsValid()
        {
            return Owner;
        }

        public virtual void Update(float deltaTime)
        {
            
        }

        public virtual void SetVisible(bool visible)
        {
            IsVisible = visible;
            // if (null != spriteSkin && spriteSkin.enabled != visible)
            // {
            //     spriteSkin.enabled = visible;
            // }
        }

        public bool Compare(IQuadTreeData quadTreeData)
        {
            if (!(quadTreeData is QuadTreeGameObject obj))
                return false;

            if (!Owner || !obj.Owner)
                return false;
        
            return Owner.GetInstanceID() == obj.Owner.GetInstanceID();
        }
    }
}
