using InfiniteJumper.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Undine.Core;

namespace InfiniteJumper.Systems
{
    public class CustomPhysics : UnifiedSystem<CustomPhysicsComponent>
    {
        public Vector2 _gravity;

        public CustomPhysics(Vector2 gravity)
        {
            _gravity = gravity;
        }

        public override void ProcessSingleEntity(int entityId, ref CustomPhysicsComponent t)
        {
            throw new NotImplementedException();
        }
    }
}