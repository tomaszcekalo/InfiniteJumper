using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfiniteJumper.Components
{
    public struct CustomPhysicsComponent
    {
        public Vector2 Speed;

        public Rectangle Box;
        public bool IsAffectedByGravity { get; set; }
        public bool IsSolid { get; set; }
        public bool CanColide { get; set; }
    }
}