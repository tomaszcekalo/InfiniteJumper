using InfiniteJumper.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using Undine.Core;
using Undine.DefaultEcs;

namespace InfiniteJumper
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Song _music;
        private Song _startMusic;
        private Texture2D _chmury;
        private Texture2D _moneta;
        private Texture2D _gory;
        private Texture2D _niebo;
        private Texture2D _platform;
        private Texture2D _player;
        private EcsContainer _ecsContainer;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            //<canvas style="width: 1746px; height: 981.634px;" width="1334" height="750"></canvas>
            _graphics.PreferredBackBufferHeight = 750;
            _graphics.PreferredBackBufferWidth = 1334;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            float meterInPixels = 16;

            _ecsContainer = new DefaultEcsContainer();
            //_ecsContainer = new MinEcsContainer();
            //_ecsContainer = new LeopotamEcsContainer();

            _ecsContainer.AddSystem(new SpriteAnimationSystem());
            _ecsContainer.AddSystem(new JumpSystem());

            _ecsContainer.CreateNewEntity();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _music = Song.FromUri("music.mp3", new Uri("Content/music.mp3", UriKind.Relative));
            _startMusic = Song.FromUri("startMusic.mp3", new Uri("Content/startMusic.mp3", UriKind.Relative));
            _chmury = Content.Load<Texture2D>("chmury");
            _moneta = Content.Load<Texture2D>("moneta");
            _gory = Content.Load<Texture2D>("gory");
            _niebo = Content.Load<Texture2D>("niebo");
            _platform = Content.Load<Texture2D>("platform");
            _player = Content.Load<Texture2D>("player");

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_startMusic);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(_niebo, _niebo.Bounds, Color.White);
            _spriteBatch.Draw(_chmury, new Rectangle(
                new Point(-(int)(gameTime.TotalGameTime.TotalMilliseconds / 50), 0),
                _chmury.Bounds.Size),
                Color.White);
            _spriteBatch.Draw(_gory, new Rectangle(
                new Point(-(int)(gameTime.TotalGameTime.TotalMilliseconds / 25), 0),
                _gory.Bounds.Size),
                Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}