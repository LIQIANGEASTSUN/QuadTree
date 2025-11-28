using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Merge.Graphic
{
    public class QuadTreeGizmosStatic : MonoBehaviour
    {
        public bool draw = false;
        private List<QuadTreeStatic> NodeList = new List<QuadTreeStatic>();
        private void OnDrawGizmos()
        {
            if (!draw) return;
            Draw1();
        }

        private void GetNode()
        {
            QuadTreeStatic rootNode = RuntimeManager.Instance.QuadTreeSystem.RootNodeStatic;
            if (NodeList.Count > 0 || null == rootNode)
                return;
            
            Queue<QuadTreeStatic> queue = new Queue<QuadTreeStatic>();
            queue.Enqueue(rootNode);
            while (queue.Count > 0)
            {
                QuadTreeStatic node = queue.Dequeue();
                if (node.Objects.Count > 0)
                    NodeList.Add(node);
                
                if (node.IsLeaf)
                    continue;
                foreach (var child in node.Children)
                    queue.Enqueue(child as QuadTreeStatic);
            }
        }
        
        private void Draw1()
        {
            GetNode();
            if (NodeList.Count <= 0) return;
            
            QuadTreeStatic rootNode = RuntimeManager.Instance.QuadTreeSystem.RootNodeStatic;
            if (null == rootNode)
                return;

            foreach (QuadTreeStatic node in NodeList)
            {
                bool isOverlaps = QuadTreeStatic.VisibleNodeList.Contains(node);
                Gizmos.color = isOverlaps ? Color.red : Color.white;
                Gizmos.DrawWireCube(node.Rect.center, new Vector3(node.Rect.width * 0.95f, node.Rect.height * 0.95f, 0.01f));
#if UNITY_EDITOR
                Handles.Label(node.Rect.center, node.ID.ToString());
#endif
            }
        }

        private void Draw2()
        {
            GetNode();
            if (NodeList.Count <= 0) return;
            
            QuadTreeNode rootNode = RuntimeManager.Instance.QuadTreeSystem.RootNodeStatic;
            if (null == rootNode)
                return;

            Rect cameraRect = RuntimeManager.Instance.QuadTreeSystem.CameraRect();
            foreach (QuadTreeNode node in NodeList)
            {
                Gizmos.color = cameraRect.Overlaps(node.Rect) ? Color.blue : Color.white;
                Gizmos.DrawWireCube(node.Rect.center, new Vector3(node.Rect.width * 0.95f, node.Rect.height * 0.95f, 0.01f));
#if UNITY_EDITOR
                Handles.Label(node.Rect.center, node.ID.ToString());
#endif
            }
        }
        
    }
}
