using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;

namespace Stride_Asteroids
{
    public class Main : StartupScript
    {
        readonly Random TherandomNG = new Random(DateTime.Now.Millisecond);
        public static Main instance;
        public List<Rock> rockScriptList = new List<Rock>();
        public UFO UFOScript;
        Prefab m_PlayerPrefab;
        Player playerScript;

        int score = 0;
        int highScore = 0;
        int bonusLifeAmount = 10000;
        int bonusLifeScore = 0;
        int lives = 3;
        int wave = 0;
        bool gameOver;

        public int Score { get => score; }
        public int Wave { get => wave; }
        public Player PlayerScript { get => playerScript; }
        public Random random { get => TherandomNG; }

        public enum RockSize
        {
            Small,
            Medium,
            Large
        };

        public override void Start()
        {
            if (instance == null)
            {
                instance = this;
            }

            m_PlayerPrefab = Content.Load<Prefab>("Player");
            Entity player = m_PlayerPrefab.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(player);
            playerScript = player.Components.Get<Player>();
        }
        /// <summary>
        /// Get a random float between min and max.
        /// </summary>
        /// <param name="min">the minimum random value</param>
        /// <param name="max">the maximum random value</param>
        /// <returns>float</returns>
        public float RandomMinMax(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }
        /// <summary>
        /// Get a random int between min and max, inclusive max IE 0, 4 = 0 to 4.
        /// </summary>
        /// <param name="min">the minimum random value</param>
        /// <param name="max">the maximum random value</param>
        /// <returns>float</returns>
        public int RandomMinMax(int min, int max)
        {
            int rnd = random.Next(min, max + 1);
            return rnd;
        }
        /// <summary>
        /// Returns a velocity from rotation angle and magnitude.
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="magnitude"></param>
        /// <returns>Vector3</returns>
        public Vector3 VelocityFromAngle(float rotation, float magnitude)
        {
            return new Vector3((float)Math.Cos(rotation) * magnitude, (float)Math.Sin(rotation) * magnitude, 0);
        }
        /// <summary>
        /// Returns a random number between 0 and Pi x 2.
        /// </summary>
        /// <returns>float</returns>
        public float RandomRadian()
        {
            return RandomMinMax(0, (float)Math.PI * 2);
        }
        /// <summary>
        /// Returns a random velocity using a random angle and a set magnitude.
        /// </summary>
        /// <param name="magnitude"></param>
        /// <returns>Vector3</returns>
        public Vector3 SetVelocityFromAngle(float magnitude)
        {
            float ang = RandomRadian();
            return new Vector3((float)Math.Cos(ang) * magnitude, (float)Math.Sin(ang) * magnitude, 0);
        }

    }
}
