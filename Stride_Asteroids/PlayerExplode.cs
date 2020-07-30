using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Audio;
using Stride.Media;
using Stride.Particles.Spawners;

namespace Stride_Asteroids
{
    public class PlayerExplode : Actor
    {
        Line[] shipLines = new Line[6];
        SoundInstance explodeSoundInstance;

        public override void Start()
        {
            base.Start();
            Prefab myLinePrefab = Content.Load<Prefab>("Prefabs/Line");

            for (int i = 0; i < 6; i++)
            {
                Entity line = myLinePrefab.Instantiate().First();
                SceneSystem.SceneInstance.RootScene.Entities.Add(line);
                shipLines[i] = line.Components.Get<Line>();
            }

            explodeSoundInstance = Content.Load<Sound>("Sounds/PlayerExplosion").CreateInstance();
            explodeSoundInstance.Volume = 0.5f;
            explodeSoundInstance.Pitch = 0.5f;
        }

        public override void Update()
        {
            base.Update();

        }

        public void Explode()
        {
            if (explodeSoundInstance.PlayState != PlayState.Playing)
            {
                explodeSoundInstance.Play();
            }

            foreach (Line line in shipLines)
            {
                line.Spawn(position + new Vector3(Main.instance.RandomMinMax(-Radius, Radius),
                    Main.instance.RandomMinMax(-Radius, Radius), 0));
            }
        }

        protected bool CheckExplodeDone()
        {
            foreach(Line line in shipLines)
            {
                if (line.IsActive())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
