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
using Undine.Core;
using Undine.DefaultEcs;
using Undine.MonoGame;
using Undine.VelcroPhysics.MonoGame;
using VelcroPhysics.Utilities;

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
        private Texture2D _polar_bear;
        private Texture2D _snowflake;
        private Texture2D _snowman_32x32;
        private Texture2D _player_x32_21x31;
        private Texture2D _gory;
        private Texture2D _niebo;
        private Texture2D _platform;
        private Texture2D _playerTexture;
        private SpriteFont _font;
        private SoundEffect _coinSound;
        private SoundEffect _dieSound;
        private SoundEffect _jumpSound;
        private IUnifiedEntity _coin;
        private IUnifiedEntity _bear;
        private EcsContainer _ecsContainer;
        private ISystem _spriteAnimationSystem;
        private Texture2D _textStroke;
        private IUnifiedEntity _player;
        private Camera2D _camera;
        private IGameStateManager _gameStateManager;

        private List<IUnifiedEntity> _platforms = new List<IUnifiedEntity>();

        private Settings _settings;
        private VelcroPhysicsSystem _velcroPhysicsSystem;
        private VelcroPhysicsComponent _playerPhysics;
        private LastPlatformProvider _lastPlatformProvider;
        private PlatformCountProvider _platformCountProvider;
        private CoinCountProvider _coinCountProvider;
        private ScoreKeeper _scoreKeeper;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
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
            ref var p = ref _player.GetComponent<VelcroPhysicsComponent>();
            p.Body.Position = new Vector2(0, 0);
            _camera.Position = new Vector2(0, 0);
            Vector2 position = new Vector2(0, 0);

            for (int i = 0; i < _platforms.Count; i++)
            {
                var platform = _platforms[i];
                position = ConvertUnits.ToSimUnits(
                    new Vector2(
                        _settings.PlatformPosition.X.Offset + i * _settings.PlatformPosition.X.Multiplier,
                        _settings.PlatformPosition.Y)
                    );
                var physicsComponent = platform.GetComponent<VelcroPhysicsComponent>();
                physicsComponent.Body.Position = position;
            }
            _coin.GetComponent<VelcroPhysicsComponent>().Body.Position = new Vector2(-10, -10);
            _bear.GetComponent<VelcroPhysicsComponent>().Body.Position = ConvertUnits.ToSimUnits(new Vector2(1320, 320));
            _lastPlatformProvider.Position = position;
            _coinCountProvider.CointCount = 0;
            _scoreKeeper.HighScore.Add(new ScoreEntry()
            {
                Score = _platformCountProvider.PlatformCount
            });
            _scoreKeeper.HighScore = _scoreKeeper.HighScore.OrderBy(x => x.Score).ToList();
            _platformCountProvider.PlatformCount = 0;
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
            _scoreKeeper = new ScoreKeeper();
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

            var physicsWorld = new VelcroPhysics.Dynamics.World(
                ConvertUnits.ToSimUnits
                (
                    _settings.Gravity.ToVector2()
                ));
            var physicsEntity = _ecsContainer.CreateNewEntity();
            physicsEntity.AddComponent(new VelcroWorldComponent()
            {
                World = physicsWorld
            });
            _velcroPhysicsSystem = new VelcroPhysicsSystem();
            _ecsContainer.AddSystem(_velcroPhysicsSystem);
            _ecsContainer.AddSystem(new VelcroPhysicsTransformSystem());
            _lastPlatformProvider = new LastPlatformProvider();

            _ecsContainer.AddSystem(new RotationAnimationSystem()
            {
                GameTimeProvider = _updateGameTimeProvider
            });
            _coinCountProvider = new CoinCountProvider();

            //_music = Song.FromUri("music.mp3", new Uri("Content/music.mp3", UriKind.Relative));
            //_startMusic = Song.FromUri("startMusic.mp3", new Uri("Content/startMusic.mp3", UriKind.Relative));
            _music = Content.Load<Song>("music");
            _startMusic = Content.Load<Song>("startMusic");
            _chmury = Content.Load<Texture2D>("chmury");
            _moneta = Content.Load<Texture2D>("moneta");
            _polar_bear = Content.Load<Texture2D>("polar_bear");
            _snowflake = Content.Load<Texture2D>("snowflake");
            _snowman_32x32 = Content.Load<Texture2D>("snowman_32x32");
            _player_x32_21x31 = Content.Load<Texture2D>("player_x32_21x31");
            _gory = Content.Load<Texture2D>("gory");
            _niebo = Content.Load<Texture2D>("niebo");
            _platform = Content.Load<Texture2D>("platform");
            _playerTexture = Content.Load<Texture2D>("player");
            _font = Content.Load<SpriteFont>("ClickToStartFont");
            _coinSound = Content.Load<SoundEffect>("coin");

            _player = _ecsContainer.CreateNewEntity();
            _ecsContainer.AddSystem(new CoinSystem(_lastPlatformProvider, _coinCountProvider, _coinSound, _player));

            StrokeEffect.strokeEffectCache = Content.Load<Effect>("StrokeEffect");
            Color textColor = Color.White;
            Vector2 scale = Vector2.One;
            int strokeSize = 3;
            Color strokeColor = Color.Black;
            StrokeType strokeType = StrokeType.OutlineAndTexture;
            _textStroke = StrokeEffect.CreateStrokeSpriteFont(_font, "Press SPACE", textColor, scale, strokeSize, strokeColor, GraphicsDevice, strokeType);

            _ecsContainer.AddSystem(
                new EnemySystem(
                    _gameStateManager,
                    _updateGameTimeProvider,
                    _settings.Player.DieSound,
                    _dieSound, _player));
            _camera = new Camera2D(this)
            {
                Focus = _player
            };
            _camera.Initialize();
            _platformCountProvider = new PlatformCountProvider();
            var was = new WallAddingSystem(_camera, _lastPlatformProvider, _platformCountProvider, _settings);
            _ecsContainer.AddSystem(was);

            _playerPhysics = new VelcroPhysicsComponent()
            {
                Body = VelcroPhysics.Factories.BodyFactory.CreateRectangle(
                    physicsWorld,
                    ConvertUnits.ToSimUnits(24),
                    ConvertUnits.ToSimUnits(48),
                    1111f,
                    ConvertUnits.ToSimUnits(Vector2.One),
                    0,
                    VelcroPhysics.Dynamics.BodyType.Dynamic)
            };
            _playerPhysics.Body.FixedRotation = true;
            _playerPhysics.Body.LinearVelocity = new Vector2(0, 0);
            _player.AddComponent(_playerPhysics);
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
            _playerPhysics.Body.Friction = 0;

            var entityFactory = new EntityFactory(
                _ecsContainer,
                physicsWorld,
                _settings,
                _platform,
                _moneta,
                _polar_bear);
            _coin = entityFactory.CreateCoin();
            was.Coin = _coin;

            var initialPlatform = entityFactory.CreateInitialPlatform();

            _bear = entityFactory.CreateBear();
            was.Bear = _bear;

            Vector2 position = new Vector2();
            for (int i = 0; i <= 8; i++)
            {
                position = new Vector2(
                       _settings.PlatformPosition.X.Offset + i * _settings.PlatformPosition.X.Multiplier,
                       _settings.PlatformPosition.Y);
                var platform = entityFactory.CreatePlatform(position);
                _platforms.Add(platform);
            }
            _lastPlatformProvider.Position = ConvertUnits.ToSimUnits(position);

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_startMusic);
        }

        protected override void Update(GameTime gameTime)
        {
            _velcroPhysicsSystem.ElapsedGameTimeTotalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _updateGameTimeProvider.GameTime = gameTime;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (!_gameStateManager.IsPlaying && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _gameStateManager.IsPlaying = true;
                MediaPlayer.Play(_music);
            }
            if (_gameStateManager.IsPlaying)
            {
                _playerPhysics.Body.LinearVelocity = new Vector2(11, _playerPhysics.Body.LinearVelocity.Y);
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

                _spriteBatch.DrawString(
                    _font,
                    "Platforms: " + _platformCountProvider.PlatformCount + Environment.NewLine
                    + "Coins: " + _coinCountProvider.CointCount + Environment.NewLine
                    + "Timer: " + gameTime.TotalGameTime.ToString(),
                    _camera.Position + new Vector2(10, 10),
                    Color.White);
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
                //var text = "Press SPACE";
                //foreach (var score in _scoreKeeper.HighScore)
                //{
                //    text += Environment.NewLine + score.Score.ToString();
                //}
                var textSize = _font.MeasureString("Press SPACE");
                //_spriteBatch.DrawString(
                //    _font,
                //    text,
                //    new Vector2(
                //        _graphics.PreferredBackBufferWidth / 2 - textSize.X / 2,
                //        _graphics.PreferredBackBufferHeight / 2),
                //        Color.White);

                _spriteBatch.Draw(_textStroke,
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