using InfiniteJumper.Components;
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
        public CoinSystem(LastPlatformProvider lastPlatformProvider, CoinCountProvider coinCountProvider)
        {
            LastPlatformProvider = lastPlatformProvider;
            CoinCountProvider = coinCountProvider;
        }

        public Camera2D Camera2D { get; }
        public LastPlatformProvider LastPlatformProvider { get; }
        public CoinCountProvider CoinCountProvider { get; }

        public override void ProcessSingleEntity(int entityId, ref CoinComponent a, ref VelcroPhysicsComponent b)
        {
            if (b.Body.ContactList != null)
            {
                CoinCountProvider.CointCount++;
                b.Body.Position = LastPlatformProvider.Position - new Microsoft.Xna.Framework.Vector2(2, 2);
            }
        }
    }
}