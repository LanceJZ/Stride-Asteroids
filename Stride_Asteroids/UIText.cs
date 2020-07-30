using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.UI.Controls;
using Stride.UI;

namespace Stride_Asteroids
{
    public class UIText : SyncScript
    {
        TextBlock scoreText;
        TextBlock hiScoreText;
        public TextBlock gameOver;
        public UIPage page;
        public int hiScore;

        public override void Start()
        {
            var root = page.RootElement;
            scoreText = root.FindVisualChildOfType<TextBlock>("ScoreText");
            hiScoreText = root.FindVisualChildOfType<TextBlock>("HiScoreText");
            gameOver = root.FindVisualChildOfType<TextBlock>("GameOver");
            Main.instance.UIScript = this;

            hiScore = Main.instance.ReadHighScore();

            if (hiScore > 0)
            {
                HiScore(hiScore);
            }
        }

        public override void Update()
        {

        }

        public void Score(int points)
        {
            scoreText.Text = points.ToString();
        }

        public void HiScore(int points)
        {
            hiScoreText.Text = points.ToString();
        }
    }
}
