using InfiniteJumper.Components;
using System;
using System.Collections.Generic;
using System.Text;
using Undine.Core;
using Undine.MonoGame;

namespace InfiniteJumper.Systems
{
    public class SpriteAnimationSystem : UnifiedSystem<SpriteAnimationComponent, TransformComponent>
    {
        public override void ProcessSingleEntity(int entityId, ref SpriteAnimationComponent a, ref TransformComponent b)
        {
        }
    }
}