using System;
using System.Collections.Generic;
using System.Text;

namespace InfiniteJumper
{
    public interface IGameStateManager
    {
        public bool IsPlaying { get; set; }
        bool IsLosing { get; set; }
        TimeSpan LostTimeStamp { get; set; }
    }

    public class GameStateManager : IGameStateManager
    {
        public bool IsPlaying { get; set; }
        public bool IsLosing { get; set; }
        public TimeSpan LostTimeStamp { get; set; }
    }
}