using InfiniteJumper.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Undine.Core;
using Undine.MonoGame;
using Undine.VelcroPhysics.MonoGame;

namespace InfiniteJumper.Systems
{
    internal class WallAddingSystem : UnifiedSystem<TransformComponent, VelcroPhysicsComponent, WallComponent>
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
            ref VelcroPhysicsComponent b,
            ref WallComponent c)
        {
            if ((b.Body.Position.X + VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(a.Origin.X)) < VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(Camera2D.Position.X))
            {
                var Position = VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(new Microsoft.Xna.Framework.Vector2(1500, 0));//TODO Add Magic Values To Settings

                b.Body.Position += Position;
                //LastPlatformProvider.Box = b.Body.b;

                ref var coinTransform = ref Coin.GetComponent<TransformComponent>();
                if (coinTransform.Position.X < Camera2D.Position.X)
                {
                    coinTransform.Position = new Microsoft.Xna.Framework.Vector2(a.Position.X, 452);//TODO
                }
            }
        }
    }
}