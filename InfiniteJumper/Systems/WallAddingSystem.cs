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
    internal class WallAddingSystem : UnifiedSystem<TransformComponent, CustomPhysicsComponent, WallComponent>
    {
        public WallAddingSystem(Camera2D camera2D)
        {
            Camera2D = camera2D;
        }

        public Camera2D Camera2D { get; }

        public override void ProcessSingleEntity(int entityId, ref TransformComponent a, ref CustomPhysicsComponent b, ref WallComponent c)
        {
            if (Camera2D.IsInView(a.Position, b.Box))
            {
                return;
            }
            a.Position = new Microsoft.Xna.Framework.Vector2(
                Camera2D.ScreenCenter.X + Camera2D.Game.GraphicsDevice.Viewport.Width,
                Camera2D.ScreenCenter.Y);
        }
    }
}