using InfiniteJumper.Components;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Undine.Core;
using Undine.MonoGame;

namespace InfiniteJumper.Systems
{
    internal class JumpSystem : UnifiedSystem<CollisionComponent, JumpComponent, CustomPhysicsComponent, RotationAnimationComponent>
    {
        public JumpSystem(
            IGameStateManager gameStateManager,
            IGameTimeProvider gameTimeProvider,
            SoundEffect dieSound,
            int lostTreshold)
        {
            GameStateManager = gameStateManager;
            GameTimeProvider = gameTimeProvider;
            DieSound = dieSound;
            LostTreshold = lostTreshold;
        }

        public IGameStateManager GameStateManager { get; }
        public IGameTimeProvider GameTimeProvider { get; }
        public SoundEffect DieSound { get; }
        public int LostTreshold { get; }

        public override void ProcessSingleEntity(
            int entityId,
            ref CollisionComponent b,
            ref JumpComponent c,
            ref CustomPhysicsComponent d,
            ref RotationAnimationComponent e)
        {
            if (d.Box.Top > LostTreshold && !GameStateManager.IsLosing)
            {
                GameStateManager.IsLosing = true;
                GameStateManager.LostTimeStamp = GameTimeProvider.GameTime.TotalGameTime;
                DieSound.Play();
            }
            else if (GameStateManager.IsPlaying
                && Keyboard.GetState().IsKeyDown(Keys.Space)
                && b.ColidesWithSolid)
            {
                d.SetSpeedY(c.JumpSpeed);
                e.Elapsed = 0;
            }
        }
    }
}