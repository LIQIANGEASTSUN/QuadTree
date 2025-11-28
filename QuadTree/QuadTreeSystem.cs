using System;
using Betta.Framework;
using BettaSDK;
using UnityEngine;

namespace Merge.Graphic
{
    public partial class QuadTreeSystem : SystemBase
    {
        private Vector2 _screenSize = Vector2.zero;
        private Rect _cameraRect = Rect.zero;
        private Vector2 _cameraCenter = Vector2.one * 10000;
        private Action _timer;
        public override void Init()
        {
            base.Init();
            Register();
            
            _screenSize.x = Screen.width;
            _screenSize.y = Screen.height;
            MessageDispatch.Instance.AddEventListener(GraphicMessage.CameraChange, CameraChange);
            //MessageDispatch.Instance.AddEventListener<int>(GraphicMessage.EntityViewCreated, EntityViewCreated);
            
            CreateStaticTree();
            //CreateDynamicTree();
            CameraChange();
            //CameraChangeDynamic();
            // _timer = UtilsTimer.Instance.AddDelegate(RefreshState);
        }

        public Rect CameraRect()
        {
            return _cameraRect;
        }

        private void CameraChange()
        {
            _cameraRect = CameraTools.CalcualteCameraRect(Vector2.zero, _screenSize);
            if (   Mathf.Abs(_cameraRect.center.x - _cameraCenter.x) <= 1
                && Mathf.Abs(_cameraRect.center.y - _cameraCenter.y) <= 1)
                return;
            
            _cameraCenter = _cameraRect.center;
            CameraChangeStatic();
            //CameraChangeDynamic();
        }
        
        public override void Release()
        {
            _timer?.Invoke();
            
            base.Release();
            MessageDispatch.Instance.RemoveEventListener(GraphicMessage.CameraChange, CameraChange);
            //MessageDispatch.Instance.RemoveEventListener<int>(GraphicMessage.EntityViewCreated, EntityViewCreated);
            ReleaseStatic();
        }
    }
    
}