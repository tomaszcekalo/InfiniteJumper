using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteJumper
{
    public interface IGameTimeProvider
    {
        public GameTime GameTime { get; set; }
    }

    public class GameTimeProvider : IGameTimeProvider
    {
        public GameTime GameTime { get; set; }
    }
}