using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteJumper.Components
{
    public struct PlayerComponent
    {
        public bool ColidesWithSolid { get; internal set; }
        public double ColidedAt { get; internal set; }
        public float JumpSpeed { get; internal set; }
        public bool HasDoubleJumped { get; internal set; }
    }
}