using InfiniteJumper.Components;
using InfiniteJumper.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
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
        private Texture2D _playerTexture;
        private SpriteFont _font;
        private SpriteAnimationComponent _playerAnimation;
        private SpriteAnimationComponent _coinAnimation;
        private EcsContainer _ecsContainer;
        private IUnifiedEntity _player;
        private IGameStateManager _gameStateManager;
        private CustomPhysicsSystem _physics;

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
            ConvertUnits.SetDisplayUnitToSimUnitRatio(meterInPixels);

            _ecsContainer = new DefaultEcsContainer();
            //_ecsContainer = new MinEcsContainer();
            //_ecsContainer = new LeopotamEcsContainer();

            _ecsContainer.AddSystem(new SpriteAnimationSystem());
            _ecsContainer.AddSystem(new JumpSystem());
            _physics = new CustomPhysicsSystem(new Vector2(0, 111));
            _ecsContainer.AddSystem(_physics);

            _player = _ecsContainer.CreateNewEntity();
            var playerPhysics = new CustomPhysicsComponent()
            {
                Box = new Rectangle(Point.Zero, new Point(24, 48)),
                IsAffectedByGravity = true
            };
            _player.AddComponent(playerPhysics);

            _gameStateManager = new GameStateManager();

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
            _playerTexture = Content.Load<Texture2D>("player");
            _font = Content.Load<SpriteFont>("ClickToStartFont");

            _playerAnimation = new SpriteAnimationComponent()
            {
                CurrentFrameNumber = 0,
                FPS = 4,
                Frames = new List<SpriteComponent>()
                {
                    new SpriteComponent(_playerTexture,new Rectangle(0,0,24,48)),
                    new SpriteComponent(_playerTexture,new Rectangle(24,0,24,48))
                }
            };

            _coinAnimation = new SpriteAnimationComponent()
            {
                CurrentFrameNumber = 0,
                FPS = 6,
                Frames = new List<SpriteComponent>()
                {
                    new SpriteComponent(_moneta,new Rectangle(0,0,20,20)),
                    new SpriteComponent(_moneta,new Rectangle(20,0,20,20)),
                    new SpriteComponent(_moneta,new Rectangle(40,0,20,20)),
                    new SpriteComponent(_moneta,new Rectangle(60,0,20,20)),
                    new SpriteComponent(_moneta,new Rectangle(80,0,20,20)),
                    new SpriteComponent(_moneta,new Rectangle(100,0,20,20)),
                }
            };

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_startMusic);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (!_gameStateManager.IsPlaying && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _gameStateManager.IsPlaying = true;
                MediaPlayer.Play(_music);
            }
            _physics.ElapsedGameTimeTotalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _ecsContainer.Run();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            if (_gameStateManager.IsPlaying)
            {
                _spriteBatch.Draw(_niebo, _niebo.Bounds, Color.White);
                var cloudBase = (-(int)(gameTime.TotalGameTime.TotalMilliseconds / 50)) % _chmury.Width;
                _spriteBatch.Draw(_chmury, new Rectangle(
                    new Point(cloudBase + _chmury.Width, 0),
                    _chmury.Bounds.Size),
                    Color.White);
                _spriteBatch.Draw(_chmury, new Rectangle(
                    new Point(cloudBase, 0),
                    _chmury.Bounds.Size),
                    Color.White);
                var mountainBase = (-(int)(gameTime.TotalGameTime.TotalMilliseconds / 25)) % _gory.Width;
                _spriteBatch.Draw(_gory, new Rectangle(
                    new Point(mountainBase + _gory.Width, 0),
                    _gory.Bounds.Size),
                    Color.White);
                _spriteBatch.Draw(_gory, new Rectangle(
                    new Point(mountainBase, 0),
                    _gory.Bounds.Size),
                    Color.White);
                //draw player
                _playerAnimation.Update(gameTime.ElapsedGameTime.TotalSeconds);
                _spriteBatch.Draw(
                    _playerAnimation.CurrentFrame.Texture,
                    _player.GetComponent<CustomPhysicsComponent>().Box,
                    _playerAnimation.CurrentFrame.SourceRectangle,
                    Color.White);
                // draw coin
                _coinAnimation.Update(gameTime.ElapsedGameTime.TotalSeconds);
                _spriteBatch.Draw(
                    _coinAnimation.CurrentFrame.Texture,
                    new Vector2(64, 64),
                    _coinAnimation.CurrentFrame.SourceRectangle,
                    Color.White);
            }
            else
            {
                var textSize = _font.MeasureString("Click to start");
                _spriteBatch.DrawString(
                    _font,
                    "Click to start",
                    new Vector2(
                        _graphics.PreferredBackBufferWidth / 2 - textSize.X / 2,
                        _graphics.PreferredBackBufferHeight / 2),
                    Color.White);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}