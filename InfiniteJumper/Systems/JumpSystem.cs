using System;
using System.Collections.Generic;
using System.Text;
using Undine.Core;
using Undine.MonoGame;

namespace InfiniteJumper.Systems
{
    internal class JumpSystem : UnifiedSystem<KeyboardComponent>
    {
        public override void ProcessSingleEntity(int entityId, ref KeyboardComponent t)
        {
            throw new NotImplementedException();
        }
    }
}