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
    internal class CoinSystem : UnifiedSystem<CoinComponent, VelcroPhysicsComponent>
    {
        public CoinSystem(
            LastPlatformProvider lastPlatformProvider,
            CoinCountProvider coinCountProvider,
            SoundEffect coinSound,
            IUnifiedEntity player)
        {
            LastPlatformProvider = lastPlatformProvider;
            CoinCountProvider = coinCountProvider;
            CoinSound = coinSound;
            Player = player;
        }

        public Camera2D Camera2D { get; }
        public LastPlatformProvider LastPlatformProvider { get; }
        public CoinCountProvider CoinCountProvider { get; }
        public SoundEffect CoinSound { get; }
        public IUnifiedEntity Player { get; }

        public override void ProcessSingleEntity(int entityId, ref CoinComponent a, ref VelcroPhysicsComponent b)
        {
            if (b.Body.ContactList != null)
            {
                if (b.Body.ContactList.Contact.FixtureA.Body
                    == Player.GetComponent<VelcroPhysicsComponent>().Body)
                {
                    CoinCountProvider.CointCount++;
                    b.Body.Position = LastPlatformProvider.Position - new Microsoft.Xna.Framework.Vector2(2, 2);
                    CoinSound.Play();
                }
            }
        }
    }
}