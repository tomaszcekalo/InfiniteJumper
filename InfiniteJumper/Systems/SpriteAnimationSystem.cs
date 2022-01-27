using InfiniteJumper.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Undine.Core;
using Undine.MonoGame;

namespace InfiniteJumper.Systems
{
    public class SpriteAnimationSystem
        : UnifiedSystem<SpriteAnimationComponent, TransformComponent, ColorComponent>
    {
        public SpriteBatch SpriteBatch { get; set; }
        public float ElapsedGameTimeTotalSeconds { get; set; }

        public override void ProcessSingleEntity(
            int entityId,
            ref SpriteAnimationComponent a,
            ref TransformComponent b,
            ref ColorComponent c)
        {
            a.Update(ElapsedGameTimeTotalSeconds);
            SpriteBatch.Draw(
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