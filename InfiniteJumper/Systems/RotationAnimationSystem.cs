using InfiniteJumper.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Undine.Core;
using Undine.MonoGame;

namespace InfiniteJumper.Systems
{
    internal class RotationAnimationSystem : UnifiedSystem<RotationAnimationComponent, TransformComponent>
    {
        public IGameTimeProvider GameTimeProvider { get; init; }

        public override void ProcessSingleEntity(int entityId, ref RotationAnimationComponent a, ref TransformComponent b)
        {
            if (a.Elapsed < a.Duration)
            {
                a.Elapsed += GameTimeProvider.GameTime.ElapsedGameTime.TotalSeconds;
                if (a.Elapsed > a.Duration)
                {
                    a.Elapsed = a.Duration;
                }
                b.Rotation = (float)(Math.PI * 2 * (a.Elapsed / a.Duration));
            }
        }
    }
}