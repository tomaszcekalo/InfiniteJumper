using System;
using System.Collections.Generic;
using System.Text;
using MonoGame;

namespace InfiniteJumper.Components
{
    public struct SpriteAnimationComponent
    {
        public int CurrentFrameNumber { get; set; }
        private int _fps;
        private float _delta;
        private float _frameTime;
        public List<SpriteComponent> Frames { get; set; }

        public SpriteComponent CurrentFrame
        {
            get
            {
                return Frames[CurrentFrameNumber];
            }
        }

        public int FPS
        {
            get
            {
                return _fps;
            }
            set
            {
                _fps = value;
                _frameTime = 1.0f / _fps;
            }
        }

        public void Update(float delta)
        {
            _delta += delta;
            if (_delta > _frameTime)
            {
                _delta -= _frameTime;
                CurrentFrameNumber = (CurrentFrameNumber + 1) % Frames.Count;
            }
        }
    }
}