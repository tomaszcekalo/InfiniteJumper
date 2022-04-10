using InfiniteJumper.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Undine.Core;
using Undine.MonoGame;

namespace InfiniteJumper.Systems
{
    public class CollisionSystem : UnifiedSystem<PlayerComponent, CustomPhysicsComponent, TransformComponent>
    {
        private int dx, dy, disX, disY, adx;
        public List<IUnifiedEntity> Collidables = new List<IUnifiedEntity>();

        public GameTimeProvider GameTimeProvider { get; set; }
        public LastPlatformProvider LastPlatformProvider { get; set; }

        public override void ProcessSingleEntity(
            int entityId,
            ref PlayerComponent a,
            ref CustomPhysicsComponent b,
            ref TransformComponent c)
        {
            a.ColidesWithSolid = false;
            //var box = b.Box;
            //box.Inflate(2, 2);

            foreach (var collidable in Collidables)
            {
                var ccpc = collidable.GetComponent<CustomPhysicsComponent>();
                if (b.Box.Intersects(ccpc.Box))
                {
                    if (ccpc.IsSolid)
                    {
                        a.ColidesWithSolid = true;
                        a.HasDoubleJumped = false;
                        a.ColidedAt = GameTimeProvider.GameTime.TotalGameTime.TotalSeconds;
                        dx = b.Box.Center.X - ccpc.Box.Center.X;
                        dy = b.Box.Center.Y - ccpc.Box.Center.Y;
                        disX = Math.Sign(dx) * (ccpc.Box.Width / 2 + b.Box.Width / 2) - dx;
                        disY = Math.Sign(dy) * (ccpc.Box.Height / 2 + b.Box.Height / 2) - dy;
                        if (disX == 0)
                            disX = int.MaxValue;
                        if (disY == 0)
                            disY = int.MaxValue;

                        adx = Math.Abs(disX);

                        if (Math.Min(adx, Math.Abs(disY)) == adx)
                        {
                            c.Position.X += disX;//adjust to origin
                            b.Box.X += disX;
                        }
                        else
                        {
                            c.Position.Y += disY;//adjust to origin
                            b.Box.Y += disY;

                            b.SetSpeedY(0);
                        }
                    }
                    else
                    {
                        ref var tc = ref collidable.GetComponent<TransformComponent>();
                        tc.Position = new Microsoft.Xna.Framework.Vector2(LastPlatformProvider.Box.Center.X, 452);
                    }
                }
            }
        }
    }
}