using InfiniteJumper.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;
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
        public SpriteBatch SpriteBatch { get; }
        public IGameTimeProvider GameTimeProvider { get; }

        public SpriteAnimationSystem(SpriteBatch spriteBatch, IGameTimeProvider gameTimeProvider)
        {
            SpriteBatch = spriteBatch;
            GameTimeProvider = gameTimeProvider;
        }

        public override void ProcessSingleEntity(
            int entityId,
            ref SpriteAnimationComponent a,
            ref TransformComponent b,
            ref ColorComponent c)
        {
            a.Update(GameTimeProvider.GameTime.ElapsedGameTime.TotalSeconds);
            SpriteBatch.Draw(
                texture: a.CurrentFrame.Texture,
                position: b.Position,
                sourceRectangle: a.CurrentFrame.SourceRectangle,
                color: c.Color,
                rotation: b.Rotation,
                origin: b.Origin,
                scale: b.Scale,
                effects: SpriteEffects.None,
                layerDepth: a.LayerDepth
                );

            //display bounding box
            var position = b.Position - b.Origin * b.Scale;
            var size = new Vector2(a.CurrentFrame.SourceRectangle.Width, a.CurrentFrame.SourceRectangle.Height);
            size *= b.Scale;
            Primitives2D.DrawRectangle(SpriteBatch, position, size, Color.Red);
            
        }
    }
}