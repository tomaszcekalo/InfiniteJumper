using InfiniteJumper.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Undine.Core;
using Undine.MonoGame;

namespace InfiniteJumper.Systems
{
    public class SpriteAnimationSystem : UnifiedSystem<SpriteAnimationComponent, TransformComponent, ColorComponent>
    {
        private SpriteBatch _spriteBatch;

        public override void ProcessSingleEntity(
            int entityId,
            ref SpriteAnimationComponent a,
            ref TransformComponent b,
            ref ColorComponent c)
        {
            _spriteBatch.Draw(
                texture: a.CurrentFrame.Texture,
                position: b.Position,
                sourceRectangle: a.CurrentFrame.SourceRectangle,
                color: c.Color
                //rotation:b.Rotation,
                //origin:
                );
        }
    }
}