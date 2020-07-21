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

namespace Stride_Asteroids
{
    public class UFO : Actor
    {
        Entity UFOentity;
        ModelComponent UFOMesh;
        ModelComponent UFOTIMesh;
        ModelComponent UFOBIMesh;
        TimerTick vectorTimer = new TimerTick();
        float vectorAmount = 3.15f;
        TimerTick fireTimer = new TimerTick();
        float fireAmount = 2.75f;
        float adj = 0.0666f;
        float speed;
        int points;
        bool done;

        UFOType type;
        enum UFOType
        {
            Small,
            Large
        }

        public override void Start()
        {
            ModelCreation();
            Disable();
        }

        public override void Update()
        {
            if (IsActive() && !hit)
            {
                base.Update();

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
            }
        }

        public void Spawn(int spawnCount)
        {
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
            done = true;
        }

        void CheckForCollusion()
        {
            foreach(Shot shot in Main.instance.PlayerScript.Shots)
            {
                if (CirclesIntersect(shot.Position, shot.Radius))
                {
                    shot.Disable();
                    Disable();
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

        void Enable()
        {
            UFOMesh.Enabled = true;
            UFOTIMesh.Enabled = true;
            UFOBIMesh.Enabled = true;
            hit = false;
            done = false;
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
