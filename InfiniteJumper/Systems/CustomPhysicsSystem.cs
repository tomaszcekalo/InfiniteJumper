using InfiniteJumper.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Undine.Core;

namespace InfiniteJumper.Systems
{
    public class CustomPhysicsSystem : UnifiedSystem<CustomPhysicsComponent>
    {
        public Vector2 _gravity;
        public float ElapsedGameTimeTotalSeconds { get; set; }

        public CustomPhysicsSystem(Vector2 gravity)
        {
            _gravity = gravity;
        }

        public override void ProcessSingleEntity(
            int entityId,
            ref CustomPhysicsComponent t)
        {
            if (t.IsAffectedByGravity)
            {
                t.Speed.X += _gravity.X * ElapsedGameTimeTotalSeconds;
                t.Speed.Y += _gravity.Y * ElapsedGameTimeTotalSeconds;
            }

            t.Location.X += t.Speed.X * ElapsedGameTimeTotalSeconds;
            t.Location.Y += t.Speed.Y * ElapsedGameTimeTotalSeconds;

            t.Box.Location = t.Location.ToPoint();
        }
    }
}