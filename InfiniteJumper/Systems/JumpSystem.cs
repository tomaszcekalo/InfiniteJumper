using InfiniteJumper.Components;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Undine.Core;
using Undine.MonoGame;

namespace InfiniteJumper.Systems
{
    internal class JumpSystem : UnifiedSystem<CollisionComponent, JumpComponent, CustomPhysicsComponent>
    {
        public JumpSystem(IGameStateManager gameStateManager)
        {
            GameStateManager = gameStateManager;
        }

        public IGameStateManager GameStateManager { get; }

        public override void ProcessSingleEntity(
            int entityId,
            ref CollisionComponent b,
            ref JumpComponent c,
            ref CustomPhysicsComponent d)
        {
            if (GameStateManager.IsPlaying
                && Keyboard.GetState().IsKeyDown(Keys.Space)
                && b.ColidesWithSolid)
            {
                d.SetSpeedY(c.JumpSpeed);
            }
        }
    }
}