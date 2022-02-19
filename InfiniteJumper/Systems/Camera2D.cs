using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Undine.Core;
using Undine.MonoGame;

namespace InfiniteJumper.Systems
{
    public interface ICamera2D
    {
        /// <summary>
        /// Gets or sets the position of the camera
        /// </summary>
        /// <value>The position.</value>
        Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the move speed of the camera.
        /// The camera will tween to its destination.
        /// </summary>
        /// <value>The move speed.</value>
        //float MoveSpeed { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the camera.
        /// </summary>
        /// <value>The rotation.</value>
        float Rotation { get; set; }

        /// <summary>
        /// Gets the origin of the viewport (accounts for Scale)
        /// </summary>
        /// <value>The origin.</value>
        Vector2 Origin { get; }

        /// <summary>
        /// Gets or sets the scale of the Camera
        /// </summary>
        /// <value>The scale.</value>
        float Scale { get; set; }

        /// <summary>
        /// Gets the screen center (does not account for Scale)
        /// </summary>
        /// <value>The screen center.</value>
        Vector2 ScreenCenter { get; }

        /// <summary>
        /// Gets the transform that can be applied to
        /// the SpriteBatch Class.
        /// </summary>
        /// <see cref="SpriteBatch"/>
        /// <value>The transform.</value>
        Matrix Transform { get; }

        /// <summary>
        /// Gets or sets the focus of the Camera.
        /// </summary>
        /// <seealso cref="IFocusable"/>
        /// <value>The focus.</value>
        IUnifiedEntity Focus { get; set; }

        /// <summary>
        /// Determines whether the target is in view given the specified position.
        /// This can be used to increase performance by not drawing objects
        /// directly in the viewport
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="texture">The texture.</param>
        /// <returns>
        ///     <c>true</c> if the target is in view at the specified position; otherwise, <c>false</c>.
        /// </returns>
        bool IsInView(Vector2 position, Texture2D texture);
    }

    public class Camera2D : GameComponent, ICamera2D
    {
        private Vector2 _position;
        protected float _viewportHeight;
        protected float _viewportWidth;

        public Camera2D(Game game)
            : base(game)
        { }

        #region Properties

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public float Scale { get; set; }
        public Vector2 ScreenCenter { get; protected set; }
        public Matrix Transform { get; set; }
        public IUnifiedEntity Focus { get; set; }
        //public float MoveSpeed { get; set; }

        #endregion Properties

        /// <summary>
        /// Called when the GameComponent needs to be initialized.
        /// </summary>
        public override void Initialize()
        {
            _viewportWidth = Game.GraphicsDevice.Viewport.Width;
            _viewportHeight = Game.GraphicsDevice.Viewport.Height;

            ScreenCenter = new Vector2(_viewportWidth / 2, _viewportHeight / 2);
            Scale = 1;
            //MoveSpeed = 1.25f;

            base.Initialize();
        }

        public Matrix GetViewMatrix(Vector2 parallax)
        {
            // To add parallax, simply multiply it by the position
            return Matrix.CreateTranslation(new Vector3(-Position * parallax, 0.0f)) *
                // The next line has a catch. See note below.
                //Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Scale, Scale, Scale) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        public Matrix GetViewMatrix(float parallax)
        {
            // To add parallax, simply multiply it by the position
            return Matrix.CreateTranslation(new Vector3(-Position * parallax, 0.0f)) *
                // The next line has a catch. See note below.
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Scale, Scale, Scale) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        public Matrix TransformMatrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) *
                    // The next line has a catch. See note below.
                    Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Scale, Scale, Scale) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
            }
        }

        //To go from screen to world space simply. This is commonly used to get the location of the mouse in the world for object picking.
        //        Vector2.Transform(mouseLocation, Matrix.Invert(Camera.TransformMatrix));
        //To go from world to screen space simply do the opposite.

        //Vector2.Transform(mouseLocation, Camera.TransformMatrix);
        //There is no draw back to using a matrix other than it takes a little learning.

        //Its easy to get the visible area

        //public Rectangle VisibleArea {
        //    get {
        //        var inverseViewMatrix = Matrix.Invert(View);
        //        var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
        //        var tr = Vector2.Transform(new Vector2(_screenSize.X, 0), inverseViewMatrix);
        //        var bl = Vector2.Transform(new Vector2(0, _screenSize.Y), inverseViewMatrix);
        //        var br = Vector2.Transform(_screenSize, inverseViewMatrix);
        //        var min = new Vector2(
        //            MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
        //            MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
        //        var max = new Vector2(
        //            MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
        //            MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
        //        return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        //    }
        //}
        public override void Update(GameTime gameTime)
        {
            ///////////////////////////////////////////////////////Rotation += 0.001f;
            // Create the Transform used by any
            // spritebatch process
            //Transform = Matrix.Identity *
            //            Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
            //            Matrix.CreateRotationZ(Rotation) *
            //            Matrix.CreateTranslation(Origin.X, Origin.Y, 0) *
            //            Matrix.CreateScale(new Vector3(Scale, Scale, Scale));

            Origin = ScreenCenter / Scale;

            // Move the Camera to the position that it needs to go
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //var MoveSpeed = 1.0f;
            _position.X += (Focus.GetComponent<TransformComponent>().Position.X - Position.X) * delta;// * MoveSpeed;
            //_position.Y += (Focus.GetComponent<TransformComponent>().Position.Y - Position.Y) * delta;// * MoveSpeed;
            //_position = Focus.GetComponent<TransformComponent>().Position - Origin;
            //Rotation += delta;

            base.Update(gameTime);
        }

        /// <summary>
        /// Determines whether the target is in view given the specified position.
        /// This can be used to increase performance by not drawing objects
        /// directly in the viewport
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="texture">The texture.</param>
        /// <returns>
        ///     <c>true</c> if [is in view] [the specified position]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInView(Vector2 position, Texture2D texture)
        {
            // If the object is not within the horizontal bounds of the screen

            if ((position.X + texture.Width) < (Position.X - Origin.X) || (position.X) > (Position.X + Origin.X))
                return false;

            // If the object is not within the vertical bounds of the screen
            if ((position.Y + texture.Height) < (Position.Y - Origin.Y) || (position.Y) > (Position.Y + Origin.Y))
                return false;

            // In View
            return true;
        }
    }
}