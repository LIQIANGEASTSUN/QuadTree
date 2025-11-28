using UnityEngine;

namespace Merge.Graphic
{
    /// <summary>
    /// 四叉树节点存储的数据接口
    /// </summary>
    public interface IQuadTreeData
    {
        // 这个数据的占用的空间 Rect 大小
        Rect Rect { get;}
        Transform Owner { get; }
        bool IsVisible { get; set; }

        bool IsValid();
        
        void SetVisible(bool visible);
        void Update(float deltaTime);
        
        bool Compare(IQuadTreeData quadTreeData);
    }
}
