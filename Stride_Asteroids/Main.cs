using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Audio;
using Stride.UI.Controls;
using Stride.Particles;
using Stride.Graphics.SDL;

namespace Stride_Asteroids
{
    public class Main : StartupScript
    {
        readonly Random TherandomNG = new Random(DateTime.Now.Millisecond);
        public static Main instance;
        public RockManager rockManagerScript;
        public UFOControl UFOControlScript;
        public UIText UIScript;
        public bool gameOver = true;
        Player playerScript;
        SoundInstance backgroundSoundInstance;
        FileStream fileStream;
        string fileName = "Score.sav";
        string dataRead = "";
        int score = 0;
        int highScore = 0;
        int bonusLifeAmount = 10000;
        int bonusLifeScore = 0;
        int lives = 0;
        int wave = 0;

        public int Score { get => score; }
        public int Wave { get => wave; set => wave = value; }
        public Player PlayerScript { get => playerScript; }
        public Random random { get => TherandomNG; }
        public int Lives { get => lives; }

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

            Game.Window.Title = "Asteroids Alpha 1.00";
            Prefab playerPrefab;
            playerPrefab = Content.Load<Prefab>("Prefabs/Player");
            Entity playerE = playerPrefab.Instantiate().First();
            SceneSystem.SceneInstance.RootScene.Entities.Add(playerE);
            playerScript = playerE.Components.Get<Player>();
            backgroundSoundInstance = Content.Load<Sound>("Sounds/AsteroidsBackground").CreateInstance();
            backgroundSoundInstance.IsLooping = true;
        }

        public void NewGame()
        {
            gameOver = false;
            score = 0;
            lives = 3;
            wave = 0;
            bonusLifeScore = bonusLifeAmount;
            UIScript.Score(score);
            rockManagerScript.ResetRocks();
            playerScript.ResetShip();
            playerScript.ShipLives();
            UFOControlScript.UFOScript.Disable();
            UFOControlScript.UFOScript.shotScript.Disable();
            UFOControlScript.ResetTimer();
            highScore = UIScript.hiScore;
            UIScript.gameOver.Visibility = Stride.UI.Visibility.Hidden;
            backgroundSoundInstance.Play();
        }

        public void UpdateScore(int points)
        {
            score += points;
            UIScript.Score(score);

            if (score > bonusLifeScore)
            {
                lives++;
                bonusLifeScore += bonusLifeAmount;
                playerScript.PlayBonusSound();
                playerScript.ShipLives();
            }
        }

        public void PlayerLostLife()
        {
            lives--;
            playerScript.ShipLives();

            if (lives < 0)
            {
                gameOver = true;
                UIScript.gameOver.Visibility = Stride.UI.Visibility.Visible;
                backgroundSoundInstance.Stop();

                if (score > highScore)
                {
                    highScore = score;
                    UIScript.HiScore(highScore);
                    WriteHighScore(highScore);
                }
            }
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

        bool ReadFile()
        {
            if (File.Exists(fileName))
            {
                fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                byte[] dataByte = new byte[1024];
                UTF8Encoding bufferUTF8 = new UTF8Encoding(true);

                while (fileStream.Read(dataByte, 0, dataByte.Length) > 0)
                {
                    dataRead += bufferUTF8.GetString(dataByte, 0, dataByte.Length);
                }

                Close();
            }
            else
            {
                return false;
            }

            return true;
        }

        void WriteFile(int score)
        {
            fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);

            //for (int i = 0; i < 10; i++)
            //{
            //    if (highScoreData[i].Score > 0)
            //    {
            //        byte[] name = new UTF8Encoding(true).GetBytes(highScoreData[i].Name);
            //        fileStream.Write(name, 0, name.Length);

            //        byte[] score = new UTF8Encoding(true).GetBytes(highScoreData[i].Score.ToString());
            //        fileStream.Write(score, 0, score.Length);

            //        byte[] marker = new UTF8Encoding(true).GetBytes(":");
            //        fileStream.Write(marker, 0, marker.Length);
            //    }
            //}

            byte[] scoreb = new UTF8Encoding(true).GetBytes(score.ToString());
            fileStream.Write(scoreb, 0, scoreb.Length);

            Close();
        }

        void Close()
        {
            fileStream.Flush();
            fileStream.Close();
            fileStream.Dispose();
        }

        public void WriteHighScore(int score)
        {
            WriteFile(score);
        }

        public int ReadHighScore()
        {
            if (ReadFile())
            {
                return int.Parse(dataRead);
            }
            else
            {
                return 0;
            }
        }
    }
}
