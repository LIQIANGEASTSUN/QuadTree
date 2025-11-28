using Spine.Unity;

namespace Merge.Graphic
{
    public class QuadTreeSpine : QuadTreeGameObject
    {
        private SkeletonAnimation _skeletonAnimation;

        public void SetSpine(SkeletonAnimation skeletonAnimation)
        {
            _skeletonAnimation = skeletonAnimation;
        }

        public override void SetVisible(bool visible)
        {
            base.SetVisible(visible);
            if (!_skeletonAnimation)
                return;
            
            if (visible)
            {
                _skeletonAnimation.UpdateEnable = true;
                // _skeletonAnimation.UpdateMode = UpdateMode.FullUpdate;
                // _skeletonAnimation.UpdateTiming = UpdateTiming.InUpdate;
            }
            else
            {
                _skeletonAnimation.UpdateEnable = false;
                // _skeletonAnimation.UpdateMode = UpdateMode.Nothing;
                // _skeletonAnimation.UpdateTiming = UpdateTiming.ManualUpdate;
            }
        }
    }
}