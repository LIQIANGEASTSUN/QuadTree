using System.Collections.Generic;
using UnityEngine;

namespace Merge.Graphic
{
    /// <summary>
    /// 四叉树节点
    /// </summary>
    public class QuadTreeNode
    {
        public int ID { get; private set; }
        public Rect Rect { get; private set; }
        public int Depth { get; private set; }
        public int DepthMax { get; private set; }
        public float MinSize { get; private set; }
        public int Capacity { get; private set; }
        public readonly List<IQuadTreeData> Objects = new List<IQuadTreeData>(); // 只有叶子节点存储数据
        public List<QuadTreeNode> Children { get; protected set; }  // 四个子节点
        public bool IsLeaf => null == Children;
        private static int NewID = 0;
        public QuadTreeNode() { }

        public void Init(Rect rect, int depth, int depthMax, float minSize, int capacity = 6)
        {
            ID = ++NewID;
            Rect = rect;
            Depth = depth;
            DepthMax = depthMax;
            MinSize = minSize;
            Capacity = capacity;
        }

        public void Insert(IQuadTreeData data)
        {
            if (!Rect.Overlaps(data.Rect))
                return;

            if (IsLeaf)
            {
                if (Objects.Count < Capacity || !EnableSubdivide())
                {
                    Objects.Add(data);
                    return;
                }
                
                Subdivide();
            }

            foreach (var child in Children)
            {
                child.Insert(data);
            }

            foreach (var obj in Objects)
            {
                foreach (var child in Children)
                {
                    child.Insert(obj);
                }
            }
            Objects.Clear();
        }

        /// <summary>
        /// 能否拆分子节点
        /// </summary>
        /// <returns></returns>
        private bool EnableSubdivide()
        {
            if (Depth >= DepthMax)
                return false;
            
            float subWidth = Rect.width / 2;
            float subHeight = Rect.height / 2;
            return subWidth > MinSize && subHeight > MinSize;
        }

        /// <summary>
        /// 拆分子节点
        /// </summary>
        private void Subdivide()
        {
            // 创建四个子节点
            float subWidth = Rect.width / 2;
            float subHeight = Rect.height / 2;
            Vector2 center = Rect.center;

            CreateChildren();

            Rect rect0 = new Rect(center.x - subWidth, center.y - subHeight, subWidth, subHeight);
            Children[0].Init(rect0, Depth + 1, DepthMax, MinSize, Capacity);
            
            Rect rect1 = new Rect(center.x, center.y - subHeight, subWidth, subHeight);
            Children[1].Init(rect1, Depth + 1, DepthMax, MinSize, Capacity);
            
            Rect rect2 = new Rect(center.x - subWidth, center.y, subWidth, subHeight);
            Children[2].Init(rect2, Depth + 1, DepthMax, MinSize, Capacity);
            
            Rect rect3 = new Rect(center.x, center.y, subWidth, subHeight);
            Children[3].Init(rect3, Depth + 1, DepthMax, MinSize, Capacity);

            for (int i = Objects.Count - 1; i >= 0; --i)
            {
                var obj = Objects[i];
                bool add = false;
                foreach (var child in Children)
                {
                    if (child.Rect.Overlaps(obj.Rect))
                    {
                        child.Insert(obj);
                        add = true;
                    }
                }

                if (add)
                    Objects.RemoveAt(i);
            }
        }

        protected virtual void CreateChildren()
        {
            Children = new List<QuadTreeNode>();
            for (int i = 0; i < 4; ++i)
                Children.Add(new QuadTreeNode());
        }

        public void CheckNode()
        {
            for (int i = Objects.Count - 1; i >= 0; --i)
            {
                var obj = Objects[i];
                if (!obj.IsValid())
                    Objects.RemoveAt(i);
            }

            if (IsLeaf)
                return;

            foreach (var child in Children)
            {
                child.CheckNode();
            }
        }
        
        /// <summary>
        /// 获取跟 rect 相交的所有数据
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public void QueryData(Rect rect, List<IQuadTreeData> foundList)
        {
            if (!Overlaps(rect))
                return;
            
            foreach (var obj in Objects)
            {
                if (obj.Rect.Overlaps(rect))
                    foundList.Add(obj);
            }

            if (IsLeaf)
                return;
            
            foreach (var child in Children)
                child.QueryData(rect, foundList);
        }
        
        /// <summary>
        /// 获取跟 rect 相交的所有节点
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public void QueryNode(Rect rect, List<QuadTreeNode> foundList)
        {
            if (!Overlaps(rect))
                return;
            
            foundList.Add(this);
            if (IsLeaf)
                return;
            
            foreach (var child in Children)
                child.QueryNode(rect, foundList);
        }

        private bool Overlaps(Rect rect)
        {
            if (Rect.Overlaps(rect))
                return true;
            return rect.Overlaps(Rect);
        }
        
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="data"></param>
        public void Remove(IQuadTreeData data)
        {
            if (!Overlaps(data.Rect))
                return;

            if (Objects.Contains(data))
                Objects.Remove(data);
            
            if (IsLeaf)
                return;
            
            foreach (var child in Children)
                child.Remove(data);
        }
        
        public void Release()
        {
            Objects.Clear();
            if (IsLeaf)
                return;
            
            foreach (var child in Children)
            {
                child.Release();
            }
        }
    }
}