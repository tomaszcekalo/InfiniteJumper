using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteJumper
{
    internal class Game2 : Game
    {
        private Song _music;
        private Song _startMusic;
        private GraphicsDeviceManager _graphics;
        private SoundEffect _musicSA;
        private SoundEffect _startMusicSA;

        public bool IsPlaying { get; set; }
        public bool IsStarted { get; set; }

        public Game2()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected void ResetGame()
        {
            MediaPlayer.Play(_startMusic);
        }

        protected override void LoadContent()
        {
            //_music = Song.FromUri("music.mp3", new Uri("Content/music.mp3", UriKind.Relative));
            //_startMusic = Song.FromUri("startMusic.mp3", new Uri("Content/startMusic.mp3", UriKind.Relative));
            //_music = Song.FromUri("music.wma", new Uri("Content/music.wma", UriKind.Relative));
            //_startMusic = Song.FromUri("startMusic.wma", new Uri("Content/startMusic.wma", UriKind.Relative));
            //_music = Content.Load<Song>("music");
            //_startMusic = Content.Load<Song>("startMusic");
            //_musicSA = Content.Load<SoundEffect>("music");
            //_startMusicSA = Content.Load<SoundEffect>("startMusic");

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (!IsStarted)
            {
                IsStarted = true;
                MediaPlayer.Play(_startMusic);
                var backSong = _musicSA.CreateInstance();
                backSong.IsLooped = true;
                backSong.Play();
            }

            // TODO: Add your update logic here
            if (!IsPlaying && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                IsPlaying = true;
                MediaPlayer.Play(_music);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}