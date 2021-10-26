using InfiniteJumper.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Undine.Core;
using Undine.MonoGame;

namespace InfiniteJumper.Systems
{
    internal class SpriteAnimationSystem : UnifiedSystem<TransformComponent, SpriteAnimationComponent, ColorComponent>
    {
        public SpriteBatch SpriteBatch { get; set; }

        public override void ProcessSingleEntity(int entityId,
            ref TransformComponent a,
            ref SpriteAnimationComponent b,
            ref ColorComponent c)
        {
            //if (Sprite != null)
            {
                //if (entity.CollisionComponent != null)
                //{
                //    var a = entity.CollisionComponent.Box.Size.ToVector2() / 2;
                //    SpriteBatch.Draw(this.Sprite.Texture,
                //    entity.Position + a,
                //    this.Sprite.SourceRectangle,
                //    Color.White,
                //    0,
                //    a,
                //    1,
                //    (Microsoft.Xna.Framework.Graphics.SpriteEffects)entity.Direction, 0);
                //}
                //else
                {
                    SpriteBatch.Draw(b.CurrentFrame.Texture, a.Position, b.CurrentFrame.SourceRectangle, c.Color);
                }
            }
        }
    }
}