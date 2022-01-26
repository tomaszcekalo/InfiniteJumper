using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfiniteJumper
{
    public class ConvertUnits
    {
        private static float _meterInPixels;

        public static void SetDisplayUnitToSimUnitRatio(float meterInPixels)
        {
            _meterInPixels = meterInPixels;
        }

        public static Vector2 ToSimUnits(Vector2 vector)
        {
            return new Vector2(vector.X / _meterInPixels, vector.Y / _meterInPixels);
        }
    }
}