//using InfiniteJumper.Components;
//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Undine.Core;
//using Undine.MonoGame;

//namespace InfiniteJumper.Systems
//{
//    public class CustomPhysicsSystem : UnifiedSystem<CustomPhysicsComponent, TransformComponent>
//    {
//        public Vector2 Gravity { get; }
//        public IGameTimeProvider GameTimeProvider { get; }
//        public IGameStateManager GameStateManager { get; }

//        public CustomPhysicsSystem(
//            Vector2 gravity,
//            IGameTimeProvider gameTimeProvider,
//            IGameStateManager gameStateManager)
//        {
//            Gravity = gravity;
//            GameTimeProvider = gameTimeProvider;
//            GameStateManager = gameStateManager;
//        }

//        public override void ProcessSingleEntity(int entityId, ref CustomPhysicsComponent a, ref TransformComponent b)
//        {
//            if (!GameStateManager.IsPlaying)
//                return;
//            float time = (float)GameTimeProvider.GameTime.ElapsedGameTime.TotalSeconds;
//            if (a.IsAffectedByGravity)
//            {
//                a.Speed.X += Gravity.X * time;
//                a.Speed.Y += Gravity.Y * time;
//            }

//            b.Position.X += a.Speed.X * time;
//            b.Position.Y += a.Speed.Y * time;

//            a.Box.Location = (b.Position - b.Origin).ToPoint();
//        }
//    }
//}