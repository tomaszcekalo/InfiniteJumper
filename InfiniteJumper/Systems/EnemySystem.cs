using InfiniteJumper.Components;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Undine.Core;
using Undine.VelcroPhysics.MonoGame;

namespace InfiniteJumper.Systems
{
    internal class EnemySystem : UnifiedSystem<EnemyComponent, VelcroPhysicsComponent>
    {
        public EnemySystem(
            IGameStateManager gameStateManager,
            IGameTimeProvider gameTimeProvider,
            SoundSettings diesSoundSettings,
            SoundEffect dieSound
            , IUnifiedEntity player
            )
        {
            //player.GetComponent<VelcroPhysicsComponent>().
            GameStateManager = gameStateManager;
            GameTimeProvider = gameTimeProvider;
            DiesSoundSettings = diesSoundSettings;
            DieSound = dieSound;
            Player = player;
        }

        public IGameStateManager GameStateManager { get; }
        public IGameTimeProvider GameTimeProvider { get; }
        public SoundSettings DiesSoundSettings { get; }
        public SoundEffect DieSound { get; }
        public IUnifiedEntity Player { get; }

        public override void ProcessSingleEntity(
            int entityId,
            ref EnemyComponent a,
            ref VelcroPhysicsComponent b)
        {
            if (b.Body.ContactList != null)
            {
                if (b.Body.ContactList.Contact.FixtureA.Body
                    == Player.GetComponent<VelcroPhysicsComponent>().Body
                    && !GameStateManager.IsLosing)
                {
                    GameStateManager.IsLosing = true;
                    GameStateManager.LostTimeStamp = GameTimeProvider.GameTime.TotalGameTime;
                    DieSound.Play(DiesSoundSettings.Volume, DiesSoundSettings.Pitch, DiesSoundSettings.Pan);
                }
            }
        }
    }
}