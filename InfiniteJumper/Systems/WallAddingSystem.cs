using InfiniteJumper.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Undine.Core;
using Undine.MonoGame;
using Undine.VelcroPhysics.MonoGame;
using VelcroPhysics.Utilities;

namespace InfiniteJumper.Systems
{
    internal class WallAddingSystem : UnifiedSystem<TransformComponent, VelcroPhysicsComponent, WallComponent>
    {
        public WallAddingSystem(
            Camera2D camera2D,
            LastPlatformProvider lastPlatformProvider,
            PlatformCountProvider platformCountProvider,
            Settings settings)
        {
            Camera2D = camera2D;
            LastPlatformProvider = lastPlatformProvider;
            PlatformCountProvider = platformCountProvider;
            Settings = settings;
        }

        public Camera2D Camera2D { get; }
        public LastPlatformProvider LastPlatformProvider { get; }
        public PlatformCountProvider PlatformCountProvider { get; }
        public Settings Settings { get; }
        public IUnifiedEntity Coin { get; set; }
        public IUnifiedEntity Bear { get; set; }

        public override void ProcessSingleEntity(
            int entityId,
            ref TransformComponent a,
            ref VelcroPhysicsComponent b,
            ref WallComponent c)
        {
            if ((b.Body.Position.X + ConvertUnits.ToSimUnits(a.Origin.X)) < ConvertUnits.ToSimUnits(Camera2D.Position.X))
            {
                //var Position = VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(new Microsoft.Xna.Framework.Vector2(1500, 0));//TODO Add Magic Values To Settings
                var modifier = ConvertUnits.ToSimUnits(new Vector2(Settings.PlatformPosition.X.Multiplier, 0));
                b.Body.Position = LastPlatformProvider.Position + modifier;
                LastPlatformProvider.Position = b.Body.Position;
                PlatformCountProvider.PlatformCount++;

                ref var coinTransform = ref Coin.GetComponent<TransformComponent>();
                if (coinTransform.Position.X < Camera2D.Position.X)
                {
                    ref var coinBody = ref Coin.GetComponent<VelcroPhysicsComponent>();
                    coinBody.Body.Position = b.Body.Position - new Vector2(2, 2);
                }
                ref var bearTransform = ref Bear.GetComponent<TransformComponent>();
                if (bearTransform.Position.X < Camera2D.Position.X)
                {
                    ref var bearBody = ref Bear.GetComponent<VelcroPhysicsComponent>();
                    bearBody.Body.Position = b.Body.Position - ConvertUnits.ToSimUnits(bearTransform.Origin * bearTransform.Scale * 2 - a.Origin);
                }
            }
        }
    }
}