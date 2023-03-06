using Apos.Gui;
using InfiniteJumper.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Undine.Core;
using Undine.MonoGame;
using Undine.VelcroPhysics.MonoGame;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Dynamics.Solver;
using VelcroPhysics.Utilities;

namespace InfiniteJumper
{
    public class EntityFactory
    {
        private SpriteAnimationComponent _coinAnimation;
        private readonly SpriteAnimationComponent _bearAnimation;
        private readonly ColorComponent _white;
        private readonly SpriteAnimationComponent initialPlatformAnimation;
        private readonly EcsContainer _ecsContainer;
        private readonly World physicsWorld;
        private readonly Settings _settings;
        private readonly Texture2D _platform;

        public EntityFactory(
            EcsContainer ecsContainer,
            World physicsWorld,
            Settings settings,
            Texture2D platform,
            Texture2D moneta,
            Texture2D bear)
        {
            _coinAnimation = new SpriteAnimationComponent()
            {
                CurrentFrameNumber = 0,
                FPS = 6,
                Frames = new List<SpriteComponent>()
                {
                    new SpriteComponent(moneta,new Rectangle(0,0,20,20)),//TODO: this is magic value
                    new SpriteComponent(moneta,new Rectangle(20,0,20,20)),//TODO: this is magic value
                    new SpriteComponent(moneta,new Rectangle(40,0,20,20)),//TODO: this is magic value
                    new SpriteComponent(moneta,new Rectangle(60,0,20,20)),//TODO: this is magic value
                    new SpriteComponent(moneta,new Rectangle(80,0,20,20)),//TODO: this is magic value
                    new SpriteComponent(moneta,new Rectangle(100,0,20,20)),//TODO: this is magic value
                }
            };
            _bearAnimation = new SpriteAnimationComponent()
            {
                CurrentFrameNumber = 0,
                FPS = 2,
                Frames = new List<SpriteComponent>()
                {
                    new SpriteComponent(bear,new Rectangle(0,0,640,320)),
                    new SpriteComponent(bear,new Rectangle(640,0,640,320))
                }
            };

            _white = new ColorComponent() { Color = Color.White };
            initialPlatformAnimation = new SpriteAnimationComponent()
            {
                CurrentFrameNumber = 0,
                FPS = 1,
                Frames = new List<SpriteComponent>()
                {
                    new SpriteComponent(platform, settings.InitialPlatform.Box)
                }
            };
            _ecsContainer = ecsContainer;
            this.physicsWorld = physicsWorld;
            _settings = settings;
            this._platform = platform;
        }

        public IUnifiedEntity CreateBear()
        {
            var bear = _ecsContainer.CreateNewEntity();
            bear.AddComponent(new EnemyComponent());
            bear.AddComponent(_bearAnimation);
            bear.AddComponent(new ColorComponent() { Color = Color.White });
            var bearPosition = new Vector2(1320, -320);
            var scale = Vector2.One * 0.2f;
            var size = new Vector2(640, 320);
            var realDimensions = size * scale;
            bear.AddComponent(new TransformComponent()
            {
                Position = bearPosition,
                Rotation = 0,
                Scale = scale,
                Origin = new Vector2(320, 160)
            });
            bear.AddComponent(new EnemyComponent() { });
            bear.AddComponent(new VelcroPhysicsComponent()
            {
                Body = VelcroPhysics.Factories.BodyFactory.CreateRectangle(
                    physicsWorld,
                    ConvertUnits.ToSimUnits(realDimensions.X),
                    ConvertUnits.ToSimUnits(realDimensions.Y),
                    0.1f,
                    ConvertUnits.ToSimUnits(bearPosition),
                    0,
                    BodyType.Dynamic)
            });
            return bear;
        }

        public IUnifiedEntity CreateCoin()
        {
            var coin = _ecsContainer.CreateNewEntity();
            coin.AddComponent(new CoinComponent());
            coin.AddComponent(_coinAnimation);
            coin.AddComponent(new ColorComponent() { Color = Color.White });
            var coinPosition = new Vector2(64, 64);
            coin.AddComponent(new TransformComponent()
            {
                Position = ConvertUnits.ToSimUnits(coinPosition),//TODO: this is magic value
                Rotation = 0,
                Scale = Vector2.One,
                Origin = new Vector2(10, 10)
            });
            coin.AddComponent(new VelcroPhysicsComponent()
            {
                Body = VelcroPhysics.Factories.BodyFactory.CreateRectangle(
                    physicsWorld,
                    ConvertUnits.ToSimUnits(20),
                    ConvertUnits.ToSimUnits(20),
                    0.1f,
                    ConvertUnits.ToSimUnits(Vector2.Zero),
                    0,
                    BodyType.Static)
            });
            return coin;
        }

        public IUnifiedEntity CreateInitialPlatform()
        {
            var initialPlatform = _ecsContainer.CreateNewEntity();
            initialPlatform.AddComponent(initialPlatformAnimation);
            initialPlatform.AddComponent(_white);
            initialPlatform.AddComponent(new TransformComponent()
            {
                Position = _settings.InitialPlatform.Position.ToVector2(),
                Rotation = 0,
                Scale = Vector2.One,
                Origin = new Vector2(512, 16)
            });
            initialPlatform.AddComponent(new VelcroPhysicsComponent()
            {
                Body = VelcroPhysics.Factories.BodyFactory.CreateRectangle(
                    physicsWorld,
                    ConvertUnits.ToSimUnits(_settings.InitialPlatform.Box.Width),
                    ConvertUnits.ToSimUnits(_settings.InitialPlatform.Box.Height),
                    0.1f,
                    ConvertUnits.ToSimUnits(_settings.InitialPlatform.Position.ToVector2()),
                    0,
                    BodyType.Static)
            });
            return initialPlatform;
        }

        public IUnifiedEntity CreatePlatform(Vector2 position)
        {
            var platform = _ecsContainer.CreateNewEntity();
            var platformAnimation = new SpriteAnimationComponent()
            {
                CurrentFrameNumber = 0,
                FPS = 1,
                Frames = new List<SpriteComponent>()
                    {
                        new SpriteComponent(_platform, new Rectangle(0,0,32*6,32))//TODO: this is magic value
                    }
            };
            platform.AddComponent(platformAnimation);
            platform.AddComponent(_white);
            platform.AddComponent(new TransformComponent()
            {
                Position = position,
                Rotation = 0,
                Scale = Vector2.One,
                Origin = new Vector2(32 * 6 / 2, 16)
            });
            platform.AddComponent(new VelcroPhysicsComponent()
            {
                Body = VelcroPhysics.Factories.BodyFactory.CreateRectangle(
                physicsWorld,
                ConvertUnits.ToSimUnits(32 * 6),
                ConvertUnits.ToSimUnits(32),
                0.1f,
                ConvertUnits.ToSimUnits(position),
                0,
                BodyType.Static)
            });
            platform.AddComponent(new WallComponent());
            return platform;
        }
    }
}