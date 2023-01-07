using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteJumper
{
    internal class ScoreKeeper
    {
        public ScoreKeeper()
        {
            HighScore = new List<ScoreEntry>();
        }

        public List<ScoreEntry> HighScore { get; set; }
    }

    internal class ScoreEntry
    {
        public int Score { get; set; }
    }
}