using System;

namespace InfiniteJumper
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            //using (var game = new Game2())
            using (var game = new Game1())
                game.Run();
        }
    }
}