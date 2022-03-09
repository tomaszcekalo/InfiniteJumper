using InfiniteJumper.Components;
using InfiniteJumper.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using Undine.Core;
using Undine.DefaultEcs;
using Undine.MonoGame;

namespace InfiniteJumper
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private GameTimeProvider _drawGameTimeProvider;
        private GameTimeProvider _updateGameTimeProvider;
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
        private SoundEffect _coinSound;
        private SoundEffect _dieSound;
        private IUnifiedEntity _coin;
        private EcsContainer _ecsContainer;
        private ISystem _spriteAnimationSystem;
        private IUnifiedEntity _player;
        private Camera2D _camera;
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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _drawGameTimeProvider = new GameTimeProvider();
            _updateGameTimeProvider = new GameTimeProvider();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _ecsContainer = new DefaultEcsContainer();
            //_ecsContainer = new MinEcsContainer();
            //_ecsContainer = new LeopotamEcsContainer();

            _gameStateManager = new GameStateManager();
            _spriteAnimationSystem =
            _ecsContainer.GetSystem(new SpriteAnimationSystem(_spriteBatch, _drawGameTimeProvider));
            _dieSound = Content.Load<SoundEffect>("die");
            _ecsContainer.AddSystem(new JumpSystem(_gameStateManager, _updateGameTimeProvider, _dieSound, 750));//TODO: this is magic value
            _physics = new CustomPhysicsSystem(
                new Vector2(0, 333),
                _updateGameTimeProvider,
                _gameStateManager);
            _ecsContainer.AddSystem(_physics);
            var collisionSystem = new CollisionSystem();
            _ecsContainer.AddSystem(collisionSystem);

            _music = Song.FromUri("music.mp3", new Uri("Content/music.mp3", UriKind.Relative));
            _startMusic = Song.FromUri("startMusic.mp3", new Uri("Content/startMusic.mp3", UriKind.Relative));
            _chmury = Content.Load<Texture2D>("chmury");
            _moneta = Content.Load<Texture2D>("moneta");
            _gory = Content.Load<Texture2D>("gory");
            _niebo = Content.Load<Texture2D>("niebo");
            _platform = Content.Load<Texture2D>("platform");
            _playerTexture = Content.Load<Texture2D>("player");
            _font = Content.Load<SpriteFont>("ClickToStartFont");
            _coinSound = Content.Load<SoundEffect>("coin");

            _player = _ecsContainer.CreateNewEntity();
            _camera = new Camera2D(this)
            {
                Focus = _player
            };
            _camera.Initialize();

            _ecsContainer.AddSystem(new WallAddingSystem(_camera));

            _player.AddComponent(new JumpComponent()
            {
                JumpSpeed = -222
            });
            var playerPhysics = new CustomPhysicsComponent()
            {
                Box = new Rectangle(Point.Zero, new Point(24, 48)),
                IsAffectedByGravity = true,
                Speed = new Vector2(
                    155,
                    0)
            };
            _player.AddComponent(playerPhysics);
            _player.AddComponent(new ColorComponent() { Color = Color.White });
            _player.AddComponent(new TransformComponent()
            {
                Position = new Vector2(),
                Rotation = 0,
                Scale = Vector2.One
            });

            var playerAnimation = new SpriteAnimationComponent()
            {
                CurrentFrameNumber = 0,
                FPS = 4,
                Frames = new List<SpriteComponent>()
                {
                    new SpriteComponent(_playerTexture,new Rectangle(0,0,24,48)),
                    new SpriteComponent(_playerTexture,new Rectangle(24,0,24,48))
                }
            };
            _player.AddComponent(playerAnimation);
            _player.AddComponent(new CollisionComponent());

            var coinAnimation = new SpriteAnimationComponent()
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
            _coin = _ecsContainer.CreateNewEntity();
            _coin.AddComponent(coinAnimation);
            _coin.AddComponent(new ColorComponent() { Color = Color.White });
            _coin.AddComponent(new TransformComponent()
            {
                Position = new Vector2(64, 64),
                Rotation = 0,
                Scale = Vector2.One
            });
            _coin.AddComponent(new CustomPhysicsComponent()
            {
                CanColide = true,
                Box = new Rectangle(0, 0, 20, 20)
            });
            collisionSystem.Collidables.Add(_coin);

            //starting platform
            var white = new ColorComponent() { Color = Color.White };
            var initialPlatformAnimation = new SpriteAnimationComponent()
            {
                CurrentFrameNumber = 0,
                FPS = 1,
                Frames = new List<SpriteComponent>()
                    {
                        new SpriteComponent(_platform, new Rectangle(0,0,1024,32))
                    }
            };
            var initialPlatform = _ecsContainer.CreateNewEntity();
            initialPlatform.AddComponent(initialPlatformAnimation);
            initialPlatform.AddComponent(white);
            initialPlatform.AddComponent(new TransformComponent()
            {
                Position = new Vector2(0, 512),
                Rotation = 0,
                Scale = Vector2.One
            });
            initialPlatform.AddComponent(new CustomPhysicsComponent()
            {
                CanColide = true,
                Box = new Rectangle(0, 0, 1024, 32),
                IsSolid = true
            });
            collisionSystem.Collidables.Add(initialPlatform);

            for (int i = 1; i <= 6; i++)
            {
                var platform = _ecsContainer.CreateNewEntity();
                var platformAnimation = new SpriteAnimationComponent()
                {
                    CurrentFrameNumber = 0,
                    FPS = 1,
                    Frames = new List<SpriteComponent>()
                    {
                        new SpriteComponent(_platform, new Rectangle(0,0,32*6,32))
                    }
                };
                platform.AddComponent(platformAnimation);
                platform.AddComponent(white);
                platform.AddComponent(new TransformComponent()
                {
                    Position = new Vector2(900 + i * 251, 512),
                    Rotation = 0,
                    Scale = Vector2.One
                });
                platform.AddComponent(new CustomPhysicsComponent()
                {
                    CanColide = true,
                    Box = new Rectangle(0, 0, 32 * 6, 32),
                    IsSolid = true
                });
                platform.AddComponent(new WallComponent());
                collisionSystem.Collidables.Add(platform);
            }
            //add wall adding system

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_startMusic);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            _updateGameTimeProvider.GameTime = gameTime;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (!_gameStateManager.IsPlaying && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _gameStateManager.IsPlaying = true;
                _gameStateManager.IsLosing = false;
                MediaPlayer.Play(_music);
            }
            _ecsContainer.Run();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _drawGameTimeProvider.GameTime = gameTime;
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here
            if (_gameStateManager.IsPlaying)
            {
                _spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.LinearWrap);
                _camera.Update(gameTime);
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
                _spriteBatch.End();
                _spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.LinearWrap, transformMatrix: _camera.GetViewMatrix(1));
                _spriteAnimationSystem.ProcessAll();
                if (_gameStateManager.IsLosing)
                {
                    float secondsElapsed = (float)(gameTime.TotalGameTime.TotalSeconds - _gameStateManager.LostTimeStamp.TotalSeconds);
                    var animationDurationInSeconds = 2f;
                    var opacity = MathF.Max((animationDurationInSeconds - secondsElapsed) / animationDurationInSeconds, 0);
                    if (opacity == 0)
                    {
                        _gameStateManager.IsPlaying = false;
                        MediaPlayer.Play(_startMusic);
                        ref var t = ref _player.GetComponent<TransformComponent>();
                        t.Position = new Vector2(0, 0);
                    }
                    ScreenFiller.FillRectangle(
                        _spriteBatch,
                        new Rectangle(
                            _camera.Position.ToPoint(),
                            GraphicsDevice.Viewport.Bounds.Size),
                        new Color(Color.Red, opacity),
                        0);
                }
                _spriteBatch.End();

                //int width = 16;
                //_spriteBatch.Draw(_platform, new Rectangle(0, 320, 32 * width, 32), new Rectangle(0, 0, 32 * width, 32), Color.White);
            }
            else
            {
                _spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.LinearWrap);
                var textSize = _font.MeasureString("Click to start");
                _spriteBatch.DrawString(
                    _font,
                    "Click to start",
                    new Vector2(
                        _graphics.PreferredBackBufferWidth / 2 - textSize.X / 2,
                        _graphics.PreferredBackBufferHeight / 2),
                    Color.White);
                _spriteBatch.End();
            }
            //var playerCPC = _player.GetComponent<CustomPhysicsComponent>();
            //_spriteBatch.DrawString(_font, playerCPC.Speed.Y.ToString(), new Vector2(256, 256), Color.Red);

            base.Draw(gameTime);
        }
    }
}