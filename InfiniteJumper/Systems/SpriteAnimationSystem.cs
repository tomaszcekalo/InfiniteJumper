using InfiniteJumper.Components;
using System;
using System.Collections.Generic;
using System.Text;
using Undine.Core;

namespace InfiniteJumper.Systems
{
    internal class SpriteAnimationSystem : UnifiedSystem<SpriteAnimationComponent>
    {
        public override void ProcessSingleEntity(int entityId, ref SpriteAnimationComponent t)
        {
            throw new NotImplementedException();
        }
    }
}