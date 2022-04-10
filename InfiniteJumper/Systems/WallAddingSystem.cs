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
    internal class WallAddingSystem : UnifiedSystem<TransformComponent, CustomPhysicsComponent, WallComponent>
    {
        public WallAddingSystem(Camera2D camera2D, LastPlatformProvider lastPlatformProvider)
        {
            Camera2D = camera2D;
            LastPlatformProvider = lastPlatformProvider;
        }

        public Camera2D Camera2D { get; }
        public LastPlatformProvider LastPlatformProvider { get; }
        public IUnifiedEntity Coin { get; set; }

        public override void ProcessSingleEntity(
            int entityId,
            ref TransformComponent a,
            ref CustomPhysicsComponent b,
            ref WallComponent c)
        {
            if (b.Box.Right < Camera2D.Position.X)
            {
                a.Position = new Microsoft.Xna.Framework.Vector2(a.Position.X + 1500, 512);//TODO Add Magic Values To Settings

                b.Box.Location = a.Position.ToPoint();
                LastPlatformProvider.Box = b.Box;

                ref var coinTransform = ref Coin.GetComponent<TransformComponent>();
                if (coinTransform.Position.X < Camera2D.Position.X)
                {
                    coinTransform.Position = new Microsoft.Xna.Framework.Vector2(a.Position.X, 452);
                }
            }
        }
    }
}