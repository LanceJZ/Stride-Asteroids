using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games.Time;
using Stride.Rendering;
using Stride.Audio;
using Stride.Graphics;

namespace Stride_Asteroids
{
    public class Rock : Explode
    {
        int points;
        float maxSpeed = 0.15f;
        float adj = 0.0666f;
        Main.RockSize size;
        Entity rock;
        ModelComponent rockMesh;

        public Main.RockSize Size { get => size; }

        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            base.Update();

            if (IsActive() && !hit)
            {
                base.Update();
                CheckForEdge();

                if (hit = CheckCollisions())
                {
                    Hit();
                }
            }
        }

        public void Spawn(Vector3 position, Main.RockSize size)
        {
            hit = false;
            this.size = size;
            float speed = 0.15f;
            this.position = position;
            rockMesh.Enabled = true;

            switch (size)
            {
                case Main.RockSize.Large:
                    radius = 0.68f * adj;
                    Entity.Transform.Scale = Vector3.One;
                    speed = maxSpeed / 3;
                    points = 20;
                    this.position.Y = RandomHieght();
                    velocity = SetVelocity(speed);

                    if (velocity.X < 0)
                    {
                        this.position.X = edge.X;
                    }
                    else
                    {
                        this.position.X = -edge.X;
                    }

                    break;
                case Main.RockSize.Medium:
                    radius = 0.68f * adj * 0.5f;
                    Entity.Transform.Scale = Vector3.One * 0.5f;
                    speed = maxSpeed / 2;
                    points = 50;
                    velocity = SetVelocity(speed);
                    break;
                case Main.RockSize.Small:
                    radius = 0.68f * adj * 0.25f;
                    Entity.Transform.Scale = Vector3.One * 0.25f;
                    speed = maxSpeed;
                    points = 100;
                    velocity = SetVelocity(speed);
                    break;
            }

            UpdatePR();
        }

        public void Disable()
        {
            rockMesh.Enabled = false;
        }

        public bool IsActive()
        {
            if (rockMesh == null)
                return false;

            return rockMesh.Enabled;
        }

        public void Initialize()
        {
            int type = Main.instance.RandomMinMax(1, 4);

            switch (type)
            {
                case 1:
                    RockOne();
                    break;
                case 2:
                    RockTwo();
                    break;
                case 3:
                    RockThree();
                    break;
                case 4:
                    RockFour();
                    break;
            }

            rock = new Entity();
            rock.Add(rockMesh);
            Entity.AddChild(rock);
            rockMesh.Enabled = false;
        }

        public bool CheckPlayerNotClearToSpawn()
        {
            if (CirclesIntersect(Vector3.Zero, Main.instance.PlayerScript.SpawnRadius))
            {
                return true;
            }

            return false;
        }

        bool CheckCollisions()
        {
            foreach (Shot shot in Main.instance.PlayerScript.Shots)
            {
                if (shot.IsActive())
                {
                    if (CirclesIntersect(shot.Position, shot.Radius))
                    {
                        shot.Disable();
                        Main.instance.UpdateScore(points);
                        return true;
                    }
                }
            }

            if (Main.instance.PlayerScript.IsActive())
            {
                if (CirclesIntersect(Main.instance.PlayerScript.Position, Main.instance.PlayerScript.Radius))
                {
                    Main.instance.UpdateScore(points);
                    Main.instance.PlayerScript.GotHit();
                    return true;
                }
            }

            if (Main.instance.UFOControlScript.UFOScript.IsActive())
            {
                if (CirclesIntersect(Main.instance.UFOControlScript.UFOScript.Position,
                    Main.instance.UFOControlScript.UFOScript.Radius))
                {
                    Main.instance.UFOControlScript.UFOScript.Disable();
                    return true;
                }
            }

            if (Main.instance.UFOControlScript.UFOScript.shotScript.IsActive())
            {
                if (CirclesIntersect(Main.instance.UFOControlScript.UFOScript.shotScript.Position,
                    Main.instance.UFOControlScript.UFOScript.shotScript.Radius))
                {
                    Main.instance.UFOControlScript.UFOScript.shotScript.Disable();
                    return true;
                }
            }

            return false;
        }

        new void Hit()
        {
            Spawn(position);
            hit = true;
            Main.instance.rockManagerScript.RockHit();
            Disable();

            //switch (size)
            //{
            //    case Main.RockSize.Large:
            //        radius = 0.75f;
            //        break;
            //    case Main.RockSize.Medium:
            //        radius = 0.375f;
            //        break;
            //    case Main.RockSize.Small:
            //        radius = 0.188f;
            //        break;
            //}
        }

        #region RockModels
        void RockOne()
        {
            // VertexPositionNormalTexture is the layout that the engine uses in the shaders
            Buffer<VertexPositionNormalTexture> vBuffer = Stride.Graphics.Buffer.Vertex.New(GraphicsDevice,
                new VertexPositionNormalTexture[]
            {
                 new VertexPositionNormalTexture(new Vector3(-0.68f * adj, 0.352f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.352f * adj, 0.704f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.0f, 0.516f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.352f * adj, 0.704f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.63f * adj, 0.352f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.352f * adj, 0.164f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.68f * adj, -0.1f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.352f * adj, -0.7f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.164f * adj, -0.6f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.352f * adj, -0.704f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.68f * adj, -0.47f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.494f * adj, -0.0f, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.68f * adj, 0.352f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0))
            });

            MeshDraw meshDraw = new MeshDraw
            {
                PrimitiveType = PrimitiveType.LineStrip, // Tell the GPU that this is a line.
                VertexBuffers = new[] { new VertexBufferBinding(vBuffer,
                VertexPositionNormalTexture.Layout, vBuffer.ElementCount) },
                DrawCount = vBuffer.ElementCount
            };

            Mesh mesh = new Mesh();
            mesh.Draw = meshDraw;

            Model model = new Model();
            model.Add(mesh);
            rockMesh = new ModelComponent(model);
        }

        void RockTwo()
        {
            // VertexPositionNormalTexture is the layout that the engine uses in the shaders
            Buffer<VertexPositionNormalTexture> vBuffer = Stride.Graphics.Buffer.Vertex.New(GraphicsDevice,
                new VertexPositionNormalTexture[]
            {
                 new VertexPositionNormalTexture(new Vector3(-0.68f * adj, 0.352f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.328f * adj, 0.68f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.0f * adj, 0.352f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.352f * adj, 0.68f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.68f * adj, 0.352f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.516f * adj, 0.0f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.68f * adj, -0.352f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.164f * adj, -0.68f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.328f * adj, -0.68f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.68f * adj, -0.328f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.68f * adj, 0.352f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0))
            });

            MeshDraw meshDraw = new MeshDraw
            {
                PrimitiveType = PrimitiveType.LineStrip, // Tell the GPU that this is a line.
                VertexBuffers = new[] { new VertexBufferBinding(vBuffer,
                VertexPositionNormalTexture.Layout, vBuffer.ElementCount) },
                DrawCount = vBuffer.ElementCount
            };

            Mesh mesh = new Mesh();
            mesh.Draw = meshDraw;

            Model model = new Model();
            model.Add(mesh);
            rockMesh = new ModelComponent(model);
        }

        void RockThree()
        {
            // VertexPositionNormalTexture is the layout that the engine uses in the shaders
            Buffer<VertexPositionNormalTexture> vBuffer = Stride.Graphics.Buffer.Vertex.New(GraphicsDevice,
                new VertexPositionNormalTexture[]
            {
                 new VertexPositionNormalTexture(new Vector3(-0.68f * adj, 0.352f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.3f * adj, 0.352f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.376f * adj, 0.68f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.188f * adj, 0.68f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.68f * adj, 0.352f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.68f * adj, 0.188f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.188f * adj, 0.0f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.68f * adj, -0.328f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.328f * adj, -0.658f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.164f * adj, -0.48f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.352f * adj, -0.55f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.68f * adj, -0.188f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.68f * adj, 0.352f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0))
            });

            MeshDraw meshDraw = new MeshDraw
            {
                PrimitiveType = PrimitiveType.LineStrip, // Tell the GPU that this is a line.
                VertexBuffers = new[] { new VertexBufferBinding(vBuffer,
                VertexPositionNormalTexture.Layout, vBuffer.ElementCount) },
                DrawCount = vBuffer.ElementCount
            };

            Mesh mesh = new Mesh();
            mesh.Draw = meshDraw;

            Model model = new Model();
            model.Add(mesh);
            rockMesh = new ModelComponent(model);
        }

        void RockFour()
        {
            // VertexPositionNormalTexture is the layout that the engine uses in the shaders
            Buffer<VertexPositionNormalTexture> vBuffer = Stride.Graphics.Buffer.Vertex.New(GraphicsDevice,
                new VertexPositionNormalTexture[]
            {
                 new VertexPositionNormalTexture(new Vector3(-0.68f * adj, 0.188f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.26f * adj, 0.68f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.37f * adj, 0.68f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.704f * adj, 0.2f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.704f * adj, -0.164f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.376f * adj, -0.68f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.328f * adj, -0.68f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.0f * adj, -0.68f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(0.0f * adj, -0.188f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.328f * adj, -0.658f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.68f * adj, -0.164f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.352f * adj, 0.0f, 0), new Vector3(0, 1, 1), new Vector2(0, 0)),
                 new VertexPositionNormalTexture(new Vector3(-0.68f * adj, 0.188f * adj, 0), new Vector3(0, 1, 1), new Vector2(0, 0))
            });

            MeshDraw meshDraw = new MeshDraw
            {
                PrimitiveType = PrimitiveType.LineStrip, // Tell the GPU that this is a line.
                VertexBuffers = new[] { new VertexBufferBinding(vBuffer,
                VertexPositionNormalTexture.Layout, vBuffer.ElementCount) },
                DrawCount = vBuffer.ElementCount
            };

            Mesh mesh = new Mesh();
            mesh.Draw = meshDraw;

            Model model = new Model();
            model.Add(mesh);
            rockMesh = new ModelComponent(model);
        }
        #endregion
    }
}
