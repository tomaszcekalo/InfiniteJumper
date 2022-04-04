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
    internal class JumpSystem : UnifiedSystem<PlayerComponent, CustomPhysicsComponent, RotationAnimationComponent>
    {
        private KeyboardState _kbState;
        public IGameStateManager GameStateManager { get; }
        public IGameTimeProvider GameTimeProvider { get; }
        public SoundEffect DieSound { get; }
        public int LostTreshold { get; }

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

        public override void ProcessSingleEntity(
            int entityId,
            ref PlayerComponent a,
            ref CustomPhysicsComponent b,
            ref RotationAnimationComponent c)
        {
            var kbCurrent = Keyboard.GetState();
            if (b.Box.Top > LostTreshold && !GameStateManager.IsLosing)
            {
                GameStateManager.IsLosing = true;
                GameStateManager.LostTimeStamp = GameTimeProvider.GameTime.TotalGameTime;
                DieSound.Play();
            }
            else if (GameStateManager.IsPlaying
                && kbCurrent.IsKeyDown(Keys.Space)
                //&& _kbState.IsKeyUp(Keys.Space)
                && a.ColidesWithSolid)
            {
                b.SetSpeedY(a.JumpSpeed);
                //c.Elapsed = 0;
            }
            else if (GameStateManager.IsPlaying
                && kbCurrent.IsKeyDown(Keys.Space)
                && _kbState.IsKeyUp(Keys.Space)
                && !a.HasDoubleJumped
                && b.Speed.Y > 24)
            {
                //cayote time
                var diff = GameTimeProvider.GameTime.TotalGameTime.TotalSeconds - a.ColidedAt;
                if (diff < 1)
                {
                    b.SetSpeedY(a.JumpSpeed);
                    c.Elapsed = 0;
                    a.HasDoubleJumped = true;
                }
            }
            _kbState = kbCurrent;
        }
    }
}