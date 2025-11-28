using System.Collections.Generic;
using UnityEngine;

namespace Merge.Graphic
{
    public class QuadTreeStatic : QuadTreeNode, ICameraQuadTree
    {
        public static List<QuadTreeNode> VisibleNodeList = new List<QuadTreeNode>(); // 可见节点hash
        public static Dictionary<int, IQuadTreeData> VisibleDataDict = new ();
        protected override void CreateChildren()
        {
            Children = new List<QuadTreeNode>();
            for (int i = 0; i < 4; ++i)
                Children.Add(new QuadTreeStatic());
        }
        
        public virtual void CheckCamera(Rect rect)
        {
            bool isOverlaps = rect.Overlaps(Rect);
            if (!isOverlaps)
            {
                RemoveInVisibleNode(this);
                return;
            }

            if (!VisibleNodeList.Contains(this) && Objects.Count > 0)
                VisibleNodeList.Add(this);

            for (int i = Objects.Count - 1; i >= 0; --i)
            {
                IQuadTreeData data = Objects[i];
                if (data.IsValid())
                {
                    data.SetVisible(true);
                    VisibleDataDict.TryAdd(data.Owner.GetInstanceID(), data);
                }
                else
                {
                    Objects.RemoveAt(i);
                    VisibleDataDict.Remove(data.Owner.GetInstanceID());
                }
            }

            if (IsLeaf)
                return;

            foreach (var child in Children)
            {
                if (child is ICameraQuadTree cameraQuadTree)
                    cameraQuadTree.CheckCamera(rect);
            }
        }

        public void CheckOutRect(Rect rect)
        {
            // 遍历，检查原本可见的节点是否还是可见
            for (int i = VisibleNodeList.Count - 1; i >= 0; i--)
            {
                QuadTreeNode node = VisibleNodeList[i];
                bool isOverlaps = rect.Overlaps(node.Rect);
                if (!isOverlaps)
                    RemoveInVisibleNode(node);
            }
            
            // RemoveInVisibleNode(node); 注意：有些物体包含在多个节点中，移除的时候可能会有误删除
            // 如：物体 A 包含在 Node1 和 Node2，此时 Node1 可见，Node2 不可见
            // RemoveInVisibleNode(Node1); 会将 A 从 VisibleNodeList 移除，但是 Node2是可见的，所以 A 还是可见的
            // 解决方案：重新遍历一遍，把所有可见节点下的物体都添加进来
            for (int i = VisibleNodeList.Count - 1; i >= 0; i--)
            {
                QuadTreeNode node = VisibleNodeList[i];
                foreach (var data in node.Objects)
                    VisibleDataDict.TryAdd(data.Owner.GetInstanceID(), data);
            }
        }

        private void RemoveInVisibleNode(QuadTreeNode node)
        {
            if (!VisibleNodeList.Contains(node))
                return;
            
            VisibleNodeList.Remove(node);
            foreach (var data in node.Objects)
            {
                data.SetVisible(false);
                VisibleDataDict.Remove(data.Owner.GetInstanceID());
            }
        }
        
    }
}
