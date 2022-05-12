using InfiniteJumper.Components;
using InfiniteJumper.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        private SoundEffect _jumpSound;
        private IUnifiedEntity _coin;
        private EcsContainer _ecsContainer;
        private ISystem _spriteAnimationSystem;
        private IUnifiedEntity _player;
        private Camera2D _camera;
        private IGameStateManager _gameStateManager;
        private CustomPhysicsSystem _physics;
        private List<IUnifiedEntity> _platforms = new List<IUnifiedEntity>();
        private Settings _settings;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            string path = "Settings.json";
            if (File.Exists(path))
            {
                string readText = File.ReadAllText(path);
                _settings = JsonConvert.DeserializeObject<Settings>(readText);
            }
            else
            {
                throw new Exception("Settings file not found.");
            }
            _graphics.PreferredBackBufferHeight = _settings.PreferredBackBuffer.Height;
            _graphics.PreferredBackBufferWidth = _settings.PreferredBackBuffer.Width;
            _graphics.ApplyChanges();
            float meterInPixels = _settings.MeterInPixels;
            ConvertUnits.SetDisplayUnitToSimUnitRatio(meterInPixels);

            base.Initialize();
        }

        protected void ResetGame()
        {
            _gameStateManager.IsPlaying = false;
            _gameStateManager.IsLosing = false;
            MediaPlayer.Play(_startMusic);
            ref var t = ref _player.GetComponent<TransformComponent>();
            t.Position = new Vector2(0, 0);
            ref var p = ref _player.GetComponent<CustomPhysicsComponent>();
            p.Box.Location = new Point(0, 0);
            p.Speed = new Vector2(155, 0);
            _camera.Position = new Vector2(0, 0);

            for (int i = 0; i < _platforms.Count; i++)
            {
                var platform = _platforms[i];
                ref var position = ref platform.GetComponent<TransformComponent>();
                position.Position = new Vector2(900 + i * 251, 512);//TODO: this is magic value
            }
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
            _jumpSound = Content.Load<SoundEffect>("jump");
            _ecsContainer.AddSystem(
                new JumpSystem(
                    _gameStateManager,
                    _updateGameTimeProvider,
                    _dieSound,
                    _jumpSound,
                    _settings.Player.DieSound,
                    _settings.Player.JumpSound,
                    _settings.LostTreshold));
            _physics = new CustomPhysicsSystem(
                _settings.Gravity.ToVector2(),
                _updateGameTimeProvider,
                _gameStateManager);
            _ecsContainer.AddSystem(_physics);
            var lpp = new LastPlatformProvider();
            var collisionSystem = new CollisionSystem()
            {
                GameTimeProvider = _updateGameTimeProvider,
                LastPlatformProvider = lpp
            };
            _ecsContainer.AddSystem(collisionSystem);
            _ecsContainer.AddSystem(new RotationAnimationSystem()
            {
                GameTimeProvider = _updateGameTimeProvider
            });

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
            var was = new WallAddingSystem(_camera, lpp);
            _ecsContainer.AddSystem(was);

            var playerPhysics = new CustomPhysicsComponent()
            {
                Box = new Rectangle(Point.Zero, new Point(24, 48)),//TODO: this is magic value
                IsAffectedByGravity = true,
                Speed = _settings.Player.Speed.ToVector2()
            };
            _player.AddComponent(playerPhysics);
            _player.AddComponent(new ColorComponent() { Color = Color.White });
            _player.AddComponent(new TransformComponent()
            {
                Position = new Vector2(),
                Rotation = 0,
                Scale = Vector2.One,
                Origin = new Vector2(12, 24)
            });

            var playerAnimation = new SpriteAnimationComponent()
            {
                CurrentFrameNumber = 0,
                FPS = 4,
                Frames = new List<SpriteComponent>()
                {
                    new SpriteComponent(_playerTexture,new Rectangle(0,0,24,48)),//TODO: this is magic value
                    new SpriteComponent(_playerTexture,new Rectangle(24,0,24,48))//TODO: this is magic value
                }
            };
            _player.AddComponent(playerAnimation);
            _player.AddComponent(new PlayerComponent()
            {
                HasDoubleJumped = true,
                JumpSpeed = _settings.Player.JumpSpeed
            });
            _player.AddComponent(new RotationAnimationComponent() { Duration = 1 });

            var coinAnimation = new SpriteAnimationComponent()
            {
                CurrentFrameNumber = 0,
                FPS = 6,
                Frames = new List<SpriteComponent>()
                {
                    new SpriteComponent(_moneta,new Rectangle(0,0,20,20)),//TODO: this is magic value
                    new SpriteComponent(_moneta,new Rectangle(20,0,20,20)),//TODO: this is magic value
                    new SpriteComponent(_moneta,new Rectangle(40,0,20,20)),//TODO: this is magic value
                    new SpriteComponent(_moneta,new Rectangle(60,0,20,20)),//TODO: this is magic value
                    new SpriteComponent(_moneta,new Rectangle(80,0,20,20)),//TODO: this is magic value
                    new SpriteComponent(_moneta,new Rectangle(100,0,20,20)),//TODO: this is magic value
                }
            };
            _coin = _ecsContainer.CreateNewEntity();
            _coin.AddComponent(coinAnimation);
            _coin.AddComponent(new ColorComponent() { Color = Color.White });
            _coin.AddComponent(new TransformComponent()
            {
                Position = new Vector2(64, 64),//TODO: this is magic value
                Rotation = 0,
                Scale = Vector2.One
            });
            _coin.AddComponent(new CustomPhysicsComponent()
            {
                CanColide = true,
                Box = new Rectangle(0, 0, 20, 20)//TODO: this is magic value
            });

            collisionSystem.Collidables.Add(_coin);
            was.Coin = _coin;

            //starting platform
            var white = new ColorComponent() { Color = Color.White };
            var initialPlatformAnimation = new SpriteAnimationComponent()
            {
                CurrentFrameNumber = 0,
                FPS = 1,
                Frames = new List<SpriteComponent>()
                {
                    new SpriteComponent(_platform, _settings.InitialPlatform.Box)
                }
            };
            var initialPlatform = _ecsContainer.CreateNewEntity();
            initialPlatform.AddComponent(initialPlatformAnimation);
            initialPlatform.AddComponent(white);
            initialPlatform.AddComponent(new TransformComponent()
            {
                Position = _settings.InitialPlatform.Position.ToVector2(),
                Rotation = 0,
                Scale = Vector2.One
            });
            initialPlatform.AddComponent(new CustomPhysicsComponent()
            {
                CanColide = true,
                Box = _settings.InitialPlatform.Box,
                IsSolid = true
            });
            collisionSystem.Collidables.Add(initialPlatform);

            for (int i = 1; i <= 6; i++)
            {
                var platform = _ecsContainer.CreateNewEntity();
                _platforms.Add(platform);
                var platformAnimation = new SpriteAnimationComponent()
                {
                    CurrentFrameNumber = 0,
                    FPS = 1,
                    Frames = new List<SpriteComponent>()
                    {
                        new SpriteComponent(_platform, new Rectangle(0,0,32*6,32))//TODO: this is magic value
                    }
                };
                platform.AddComponent(platformAnimation);
                platform.AddComponent(white);
                platform.AddComponent(new TransformComponent()
                {
                    Position = new Vector2(
                        _settings.PlatformPosition.X.Offset + i * _settings.PlatformPosition.X.Multiplier,
                        _settings.PlatformPosition.Y),
                    Rotation = 0,
                    Scale = Vector2.One
                });
                platform.AddComponent(new CustomPhysicsComponent()
                {
                    CanColide = true,
                    Box = new Rectangle(0, 0, 32 * 6, 32),//TODO: this is magic value
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
                MediaPlayer.Play(_music);
            }
            _ecsContainer.Run();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _drawGameTimeProvider.GameTime = gameTime;
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (_gameStateManager.IsPlaying)
            {
                _spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, samplerState: SamplerState.LinearWrap);
                _camera.Update(gameTime);
                _spriteBatch.Draw(_niebo, _niebo.Bounds, Color.White);
                var cloudBase = (-(int)(gameTime.TotalGameTime.TotalMilliseconds / 50)) % _chmury.Width;//TODO: 50 is magic constant
                _spriteBatch.Draw(_chmury, new Rectangle(
                    new Point(cloudBase + _chmury.Width, 0),
                    _chmury.Bounds.Size),
                    Color.White);
                _spriteBatch.Draw(_chmury, new Rectangle(
                    new Point(cloudBase, 0),
                    _chmury.Bounds.Size),
                    Color.White);
                var mountainBase = (-(int)(gameTime.TotalGameTime.TotalMilliseconds / 25)) % _gory.Width;//TODO: 25 is magic constant
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
                    var animationDurationInSeconds = 2f;//TODO: magic constant
                    var opacity = MathF.Max((animationDurationInSeconds - secondsElapsed) / animationDurationInSeconds, 0);
                    if (opacity == 0)
                    {
                        ResetGame();
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
            base.Draw(gameTime);
        }
    }
}