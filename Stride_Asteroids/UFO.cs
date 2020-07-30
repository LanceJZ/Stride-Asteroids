using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games.Time;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Audio;
using Stride.Media;
using Stride.Core.Shaders.Ast;

namespace Stride_Asteroids
{
    public class UFO : Explode
    {
        public Shot shotScript;
        Entity UFOentity;
        ModelComponent UFOMesh;
        ModelComponent UFOTIMesh;
        ModelComponent UFOBIMesh;
        SoundInstance explodeSoundInstance;
        SoundInstance largeUFOSoundInstance;
        SoundInstance smallUFOSoundInstance;
        SoundInstance fireSoundInstance;
        TimerTick vectorTimer = new TimerTick();
        TimerTick fireTimer = new TimerTick();
        float vectorAmount = 3.15f;
        float fireAmount = 2.75f;
        float adj = 0.0666f;
        float speed;
        float shotSpeed = 0.3666f;
        int points;

        UFOType type;
        enum UFOType
        {
            Small,
            Large
        }

        public override void Start()
        {
            base.Start();

            LoadSounds();
            ModelCreation();
            Disable();
            MakeShot();
        }

        public override void Update()
        {
            base.Update();

            if (IsActive() && !hit)
            {
                base.Update();

                if (!Main.instance.gameOver)
                {
                    if (type == UFOType.Large)
                    {
                        if (largeUFOSoundInstance.PlayState != PlayState.Playing)
                        {
                            largeUFOSoundInstance.Play();
                        }
                    }
                    else
                    {
                        if (smallUFOSoundInstance.PlayState != PlayState.Playing)
                        {
                            smallUFOSoundInstance.Play();
                        }
                    }
                }

                CheckForCollusion();

                if (position.X > edge.X || position.X < -edge.X)
                {
                    Disable();
                }

                CheckForEdge();
                vectorTimer.Tick();
                fireTimer.Tick();

                if (vectorTimer.TotalTime.Seconds > vectorAmount)
                {
                    vectorTimer.Reset();
                    ChangeVector();
                }

                if (fireTimer.TotalTime.Seconds > fireAmount)
                {
                    fireTimer.Reset();
                    Fire();
                }
            }
        }

        public void Spawn(int spawnCount)
        {
            fireTimer.Reset();
            float spawnPercent = (float)(Math.Pow(0.915, spawnCount / (Main.instance.Wave + 1)) * 100);

            if (Main.instance.RandomMinMax(0, 99) < spawnPercent - (Main.instance.Score / 400))
            {

                type = UFOType.Large;
                UFOentity.Transform.Scale = Vector3.One;
                points = 200;
                radius = 0.446f * adj;
            }
            else
            {

                type = UFOType.Small;
                UFOentity.Transform.Scale = Vector3.One * 0.5f;
                points = 1000;
                radius = (0.446f * adj) * 0.5f;
            }

            float posY = Main.instance.RandomMinMax(-edge.Y * 0.25f, edge.Y * 0.25f);
            float posX;

            if (Main.instance.RandomMinMax(0, 10) > 5)
            {
                posX = -edge.X;
                speed = 0.03666f;
            }
            else
            {
                posX = edge.X;
                speed = -0.03666f;
            }

            velocity.X = speed;
            position = new Vector3(posX, posY, 0);
            UpdatePR();
            Enable();
        }

        public bool IsActive()
        {
            if (UFOMesh != null)
                return UFOMesh.Enabled;
            else
                return false;
        }

        public void Disable()
        {
            UFOMesh.Enabled = false;
            UFOTIMesh.Enabled = false;
            UFOBIMesh.Enabled = false;
            hit = false;
            //done = true;
            Main.instance.UFOControlScript.ResetTimer();
        }

        public new void Hit()
        {
            if (explodeSoundInstance.PlayState != PlayState.Playing && !Main.instance.gameOver)
            {
                explodeSoundInstance.Play();
            }

            SetExplode();
            Disable();
        }

        void CheckForCollusion()
        {
            foreach (Shot shot in Main.instance.PlayerScript.Shots)
            {
                if (shot.IsActive())
                {
                    if (CirclesIntersect(shot.Position, shot.Radius))
                    {
                        shot.Disable();
                        Hit();
                        Main.instance.UpdateScore(points);
                    }
                }
            }

            if (Main.instance.PlayerScript.IsActive())
            {
                if (CirclesIntersect(Main.instance.PlayerScript.Position, Main.instance.PlayerScript.Radius))
                {
                    Hit();
                    Main.instance.PlayerScript.GotHit();
                    Main.instance.UpdateScore(points);
                }
            }
        }

        void ChangeVector()
        {
            if (Main.instance.RandomMinMax(0, 10) < 5)
            {
                if ((int)(velocity.Y * 100) == 0)
                {
                    if (Main.instance.RandomMinMax(0, 10) < 5)
                    {
                        velocity.Y = speed;
                    }
                    else
                    {
                        velocity.Y = -speed;
                    }
                }
                else
                {
                    velocity.Y = 0;
                }
            }

        }

        void Fire()
        {
            if (fireSoundInstance.PlayState != PlayState.Playing && !Main.instance.gameOver)
            {
                fireSoundInstance.Play();
            }

            float angel = 0;

            switch (type)
            {
                case UFOType.Large:
                    angel = AutoFire();
                    break;
                case UFOType.Small:
                    angel = AimedFire();
                    break;
            }

            Vector3 dir = Main.instance.VelocityFromAngle(angel, shotSpeed);
            Vector3 offset = Main.instance.VelocityFromAngle(angel, radius);

            if (!shotScript.IsActive())
            {
                shotScript.Spawn(position + offset, dir, 1.45f);
            }
        }

        float AutoFire()
        {
            return Main.instance.RandomRadian();
        }

        float AimedFire()
        {
            //Adjust accuracy according to score. By the time the score reaches 30,000, percent = 0.
            float percentChance = 0.25f - (Main.instance.Score * 0.00001f);

            if (percentChance < 0)
            {
                percentChance = 0;
            }

            Vector3 playerPos = Main.instance.PlayerScript.Position;

            float fireRadian = ((float)Math.Atan2(playerPos.Y - Position.Y, playerPos.X -
                Position.X) + Main.instance.RandomMinMax(-percentChance, percentChance));

            return fireRadian;
        }

        void Enable()
        {
            UFOMesh.Enabled = true;
            UFOTIMesh.Enabled = true;
            UFOBIMesh.Enabled = true;
            hit = false;
            //done = false;
        }

        void MakeShot()
        {
            Prefab shotPrefab = Content.Load<Prefab>("Prefabs/Shot");
            Entity shotE;
            shotE = (shotPrefab.Instantiate().First());
            SceneSystem.SceneInstance.RootScene.Entities.Add(shotE);
            shotScript = shotE.Components.Get<Shot>();
        }

        void LoadSounds()
        {
            explodeSoundInstance = Content.Load<Sound>("Sounds/UFOExplosion").CreateInstance();
            explodeSoundInstance.Volume = 0.50f;
            fireSoundInstance = Content.Load<Sound>("Sounds/UFOShot").CreateInstance();
            fireSoundInstance.Volume = 0.5f;
            largeUFOSoundInstance = Content.Load<Sound>("Sounds/UFOLarge").CreateInstance();
            largeUFOSoundInstance.Volume = 0.5f;
            smallUFOSoundInstance = Content.Load<Sound>("Sounds/UFOSmall").CreateInstance();
            smallUFOSoundInstance.Volume = 0.5f;
        }

        void ModelCreation()
        {
            // VertexPositionNormalTexture is the layout that the engine uses in the shaders
            var vBuffer = Stride.Graphics.Buffer.Vertex.New(GraphicsDevice, new VertexPositionNormalTexture[]
            {
                 new VertexPositionNormalTexture(new Vector3(-0.07f * adj, 0.25f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Top Left
                 new VertexPositionNormalTexture(new Vector3(-0.164f * adj, 0.07f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Upper Left
                 new VertexPositionNormalTexture(new Vector3(-0.446f * adj, -0.094f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Mid Left
                 new VertexPositionNormalTexture(new Vector3(-0.188f * adj, -0.258f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Bottom Left
                 new VertexPositionNormalTexture(new Vector3(0.188f * adj, -0.258f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Bottom Right
                 new VertexPositionNormalTexture(new Vector3(0.446f * adj, -0.094f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Mid Right
                 new VertexPositionNormalTexture(new Vector3(0.164f * adj, 0.07f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Upper Right
                 new VertexPositionNormalTexture(new Vector3(0.07f * adj, 0.25f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Top Right
                 new VertexPositionNormalTexture(new Vector3(-0.07f * adj, 0.25f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Top left
            });

            MeshDraw meshDraw = new MeshDraw
            {
                PrimitiveType = PrimitiveType.LineStrip, // Tell the GPU that this is a line.
                VertexBuffers = new[] { new VertexBufferBinding(vBuffer, VertexPositionNormalTexture.Layout, vBuffer.ElementCount) },
                DrawCount = vBuffer.ElementCount
            };

            Mesh mesh = new Mesh();
            mesh.Draw = meshDraw;

            Model model = new Model();
            model.Add(mesh);
            UFOMesh = new ModelComponent(model);

            UFOentity = new Entity();
            UFOentity.Add(UFOMesh);
            Entity.AddChild(UFOentity);
            // Top inside lines for UFO
            var vBufferti = Stride.Graphics.Buffer.Vertex.New(GraphicsDevice, new VertexPositionNormalTexture[]
            {
                 new VertexPositionNormalTexture(new Vector3(0.164f * adj, 0.07f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Top inside line left
                 new VertexPositionNormalTexture(new Vector3(-0.164f * adj, 0.07f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Top inside line right
            });

            MeshDraw meshDrawti = new MeshDraw
            {
                PrimitiveType = PrimitiveType.LineStrip, // Tell the GPU that this is a line.
                VertexBuffers = new[] { new VertexBufferBinding(vBufferti, VertexPositionNormalTexture.Layout, vBufferti.ElementCount) },
                DrawCount = vBufferti.ElementCount
            };

            Mesh meshti = new Mesh();
            meshti.Draw = meshDrawti;

            Model modelti = new Model();
            modelti.Add(meshti);
            UFOTIMesh = new ModelComponent(modelti);
            Entity UFOti = new Entity();
            UFOti.Add(UFOTIMesh);
            UFOentity.AddChild(UFOti);

            // Bottom inside lines for UFO
            var vBufferbi = Stride.Graphics.Buffer.Vertex.New(GraphicsDevice, new VertexPositionNormalTexture[]
            {
                 new VertexPositionNormalTexture(new Vector3(0.446f * adj, -0.094f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), // Bottom inside line left
                 new VertexPositionNormalTexture(new Vector3(-0.446f * adj, -0.094f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)) // Bottom inside line right
            });

            MeshDraw meshDrawbi = new MeshDraw
            {
                PrimitiveType = PrimitiveType.LineStrip, // Tell the GPU that this is a line.
                VertexBuffers = new[] { new VertexBufferBinding(vBufferbi, VertexPositionNormalTexture.Layout, vBufferbi.ElementCount) },
                DrawCount = vBufferbi.ElementCount
            };

            Mesh meshbi = new Mesh();
            meshbi.Draw = meshDrawbi;

            Model modelbi = new Model();
            modelbi.Add(meshbi);
            UFOBIMesh = new ModelComponent(modelbi);
            Entity UFObi = new Entity();
            UFObi.Add(UFOBIMesh);
            UFOentity.AddChild(UFObi);
        }
    }
}
