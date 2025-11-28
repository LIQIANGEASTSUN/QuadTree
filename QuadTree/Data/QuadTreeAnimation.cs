using UnityEngine;

namespace Merge.Graphic
{
    public class QuadTreeAnimation : QuadTreeGameObject
    {
        public Animator _animator;
        public void SetAnimator(Animator animator)
        {
            _animator = animator;
            _animator.cullingMode = AnimatorCullingMode.CullCompletely;
            
            // _animator.Play(_animator.GetNextAnimatorStateInfo(0).shortNameHash);
            // _animator.enabled = false;
        }
        
        public override void Update(float deltaTime)
        {
            if (null != _animator && _animator.gameObject.activeInHierarchy)
            {
                _animator.Update(deltaTime);
            }
        }
    }
}