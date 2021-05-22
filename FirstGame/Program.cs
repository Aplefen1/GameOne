using System;

namespace FirstGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new TrexRunner())
                game.Run();
        }
    }
}
