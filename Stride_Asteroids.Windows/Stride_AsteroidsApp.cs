using Stride.Engine;

namespace Stride_Asteroids
{
    class Stride_AsteroidsApp
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
