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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Undine.Core;
using Undine.DefaultEcs;
using Undine.MonoGame;
using Undine.VelcroPhysics.MonoGame;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Utilities;

namespace InfiniteJumper
{
    internal class Game3 : Game
    {
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
        private GraphicsDeviceManager _graphics;
        private SoundEffect _musicSA;
        private SoundEffect _startMusicSA;
        private GameTimeProvider _drawGameTimeProvider;
        private GameTimeProvider _updateGameTimeProvider;
        private SpriteBatch _spriteBatch;
        private GameStateManager _gameStateManager;
        private ISystem _spriteAnimationSystem;
        private SoundEffect _dieSound;
        private SoundEffect _jumpSound;
        private EcsContainer _ecsContainer;
        private VelcroPhysicsSystem _velcroPhysicsSystem;
        private Settings _settings;
        private World _physicsWorld;
        private IUnifiedEntity _player;
        private Camera2D _camera;

        public bool IsPlaying { get; set; }
        public bool IsStarted { get; set; }

        public Game3()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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
        }

        protected override void Initialize()
        {
            base.Initialize();
            InitSystems();
            InitEntitiesAndComponents();
        }

        private void InitEntitiesAndComponents()
        {
            var playerPhysics = new VelcroPhysicsComponent()
            {
                Body = VelcroPhysics.Factories.BodyFactory.CreateRectangle(
                    _physicsWorld,
                    VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(24),
                    VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(48),
                    1,
                    VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(Vector2.One),
                    0,
                    VelcroPhysics.Dynamics.BodyType.Dynamic)
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
            initialPlatform.AddComponent(new VelcroPhysicsComponent()
            {
                Body = VelcroPhysics.Factories.BodyFactory.CreateRectangle(
                    _physicsWorld,
                    VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(_settings.InitialPlatform.Box.Width),
                    VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(_settings.InitialPlatform.Box.Height),
                    0.1f,
                    VelcroPhysics.Utilities.ConvertUnits.ToSimUnits(_settings.InitialPlatform.Position.ToVector2()),
                    0,
                    VelcroPhysics.Dynamics.BodyType.Static)
            });
        }

        private void InitSystems()
        {
            _drawGameTimeProvider = new GameTimeProvider();
            _updateGameTimeProvider = new GameTimeProvider();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _ecsContainer = new DefaultEcsContainer();
            _gameStateManager = new GameStateManager();
            _spriteAnimationSystem =
            _ecsContainer.GetSystem(new SpriteAnimationSystem(_spriteBatch, _drawGameTimeProvider));

            //_ecsContainer.AddSystem(
            //    new JumpSystem(
            //        _gameStateManager,
            //        _updateGameTimeProvider,
            //        _dieSound,
            //        _jumpSound,
            //        _settings.Player.DieSound,
            //        _settings.Player.JumpSound,
            //        _settings.LostTreshold)); var physicsWorld = new VelcroPhysics.Dynamics.World(
            //     VelcroPhysics.Utilities.ConvertUnits.ToSimUnits
            //     (
            //         _settings.Gravity.ToVector2()
            //     )
            //     );
            _velcroPhysicsSystem = new VelcroPhysicsSystem();
            _ecsContainer.AddSystem(_velcroPhysicsSystem);
            _ecsContainer.AddSystem(new VelcroPhysicsTransformSystem());
            var lpp = new LastPlatformProvider();
            _ecsContainer.AddSystem(new RotationAnimationSystem()
            {
                GameTimeProvider = _updateGameTimeProvider
            });
            _player = _ecsContainer.CreateNewEntity();
            _camera = new Camera2D(this)
            {
                Focus = _player
            };
            _camera.Initialize();
            var was = new WallAddingSystem(_camera, lpp);
            _ecsContainer.AddSystem(was);
            var physicsEntity = _ecsContainer.CreateNewEntity();
            _physicsWorld = new VelcroPhysics.Dynamics.World(_settings.Gravity.ToVector2());
            physicsEntity.AddComponent(new VelcroWorldComponent()
            {
                World = _physicsWorld
            });
        }

        protected void ResetGame()
        {
            MediaPlayer.Play(_startMusic);
        }

        protected override void LoadContent()
        {
            _music = Content.Load<Song>("music");
            _startMusic = Content.Load<Song>("startMusic");
            _chmury = Content.Load<Texture2D>("chmury");
            _moneta = Content.Load<Texture2D>("moneta");
            _gory = Content.Load<Texture2D>("gory");
            _niebo = Content.Load<Texture2D>("niebo");
            _platform = Content.Load<Texture2D>("platform");
            _playerTexture = Content.Load<Texture2D>("player");
            _font = Content.Load<SpriteFont>("ClickToStartFont");
            _coinSound = Content.Load<SoundEffect>("coin");
            _dieSound = Content.Load<SoundEffect>("die");
            _jumpSound = Content.Load<SoundEffect>("jump");
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _velcroPhysicsSystem.ElapsedGameTimeTotalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _updateGameTimeProvider.GameTime = gameTime;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _ecsContainer.Run();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _drawGameTimeProvider.GameTime = gameTime;
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (/*_gameStateManager.IsPlaying*/true)
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
                var textSize = _font.MeasureString("Click to start");
                _spriteBatch.DrawString(
                   _font,
                   _player.GetComponent<VelcroPhysicsComponent>().Body.LinearVelocity.Y.ToString(),
                   new Vector2(
                       _graphics.PreferredBackBufferWidth / 2 - textSize.X / 2,
                       _graphics.PreferredBackBufferHeight / 2),
                   Color.White);
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