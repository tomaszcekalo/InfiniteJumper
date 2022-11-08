using InfiniteJumper.Components;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Undine.Core;
using Undine.MonoGame;
using Undine.VelcroPhysics.MonoGame;

namespace InfiniteJumper.Systems
{
    internal class JumpSystem : UnifiedSystem<PlayerComponent, VelcroPhysicsComponent, RotationAnimationComponent>
    {
        private KeyboardState _kbState;
        public IGameStateManager GameStateManager { get; }
        public IGameTimeProvider GameTimeProvider { get; }
        public SoundEffect DieSound { get; }
        public SoundEffect JumpSound { get; }
        public SoundSettings DiesSoundSettings { get; }
        public SoundSettings JumpSoundSettings { get; }
        public int LostTreshold { get; }

        public JumpSystem(
            IGameStateManager gameStateManager,
            IGameTimeProvider gameTimeProvider,
            SoundEffect dieSound,
            SoundEffect jumpSound,
            SoundSettings diesSoundSettings,
            SoundSettings jumpSoundSettings,
            int lostTreshold)
        {
            GameStateManager = gameStateManager;
            GameTimeProvider = gameTimeProvider;
            DieSound = dieSound;
            JumpSound = jumpSound;
            DiesSoundSettings = diesSoundSettings;
            JumpSoundSettings = jumpSoundSettings;
            LostTreshold = lostTreshold;
        }

        public override void ProcessSingleEntity(
            int entityId,
            ref PlayerComponent a,
            ref VelcroPhysicsComponent b,
            ref RotationAnimationComponent c)
        {
            var kbCurrent = Keyboard.GetState();
            if (b.Body.Position.Y > LostTreshold && !GameStateManager.IsLosing)
            {
                GameStateManager.IsLosing = true;
                GameStateManager.LostTimeStamp = GameTimeProvider.GameTime.TotalGameTime;
                DieSound.Play(DiesSoundSettings.Volume, DiesSoundSettings.Pitch, DiesSoundSettings.Pan);
            }
            else if (GameStateManager.IsPlaying
                && kbCurrent.IsKeyDown(Keys.Space)
                && _kbState.IsKeyUp(Keys.Space)
                //&& a.ColidesWithSolid
                && b.Body.ContactList != null
                )
            {
                b.Body.LinearVelocity = new Microsoft.Xna.Framework.Vector2(b.Body.LinearVelocity.X, VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(a.JumpSpeed));
                JumpSound.Play(JumpSoundSettings.Volume, JumpSoundSettings.Pitch, JumpSoundSettings.Pan);
                a.HasDoubleJumped = false;
                a.ColidedAt = GameTimeProvider.GameTime.TotalGameTime.TotalSeconds;
                //c.Elapsed = 0;
            }
            else if (GameStateManager.IsPlaying
                && kbCurrent.IsKeyDown(Keys.Space)
                && _kbState.IsKeyUp(Keys.Space)
                && !a.HasDoubleJumped
                && b.Body.LinearVelocity.Y > 0)
            {
                //cayote time
                var diff = GameTimeProvider.GameTime.TotalGameTime.TotalSeconds - a.ColidedAt;
                //if (diff < 1)
                {
                    b.Body.LinearVelocity = new Microsoft.Xna.Framework.Vector2(b.Body.LinearVelocity.X, VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(a.JumpSpeed));
                    c.Elapsed = 0;
                    a.HasDoubleJumped = true;
                    JumpSound.Play(JumpSoundSettings.Volume, JumpSoundSettings.Pitch, JumpSoundSettings.Pan);
                }
            }
            _kbState = kbCurrent;
        }
    }
}