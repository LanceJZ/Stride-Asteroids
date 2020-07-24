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
        TextBlock score;
        public UIPage page;

        public override void Start()
        {
            var root = page.RootElement;
            score = root.FindVisualChildOfType<TextBlock>("ScoreText");
            Main.instance.UIScript = this;
        }

        public override void Update()
        {

        }

        public void Score(int points)
        {
            score.Text = points.ToString();
        }
    }
}
