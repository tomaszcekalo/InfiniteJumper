using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfiniteJumper.Components
{
    public struct CustomPhysicsComponent
    {
        public Vector2 Speed;
        public Vector2 Location;
        public bool IsAffectedByGravity { get; set; }

        public Rectangle Box;
    }
}