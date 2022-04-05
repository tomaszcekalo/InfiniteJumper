﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteJumper
{
    internal class Settings
    {
        public WidthHeight WidthHeight { get; set; }
        public int MeterInPixels { get; set; }
        public PlatformPosition PlatformPosition { get; set; }
        public int LostTreshold { get; set; }
        public Point Gravity { get; set; }
        public Player Player { get; set; }
        public InitialPlatform InitialPlatform { get; set; }
    }

    public class InitialPlatform
    {
        public Point Position { get; set; }
        public Rectangle Box { get; set; }
    }

    public class Player
    {
        public Point Speed { get; set; }
        public int JumpSpeed { get; set; }
    }

    internal class WidthHeight
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public class PlatformPosition
    {
        public PlatformPositionX X { get; set; }
        public int Y { get; set; }

        public class PlatformPositionX
        {
            public int Multiplier { get; set; }
            public int Offset { get; set; }
        }
    }
}