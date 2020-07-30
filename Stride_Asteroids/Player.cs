using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Games.Time;
using Stride.Input;
using Stride.Engine;
using Stride.Rendering;
using Stride.Audio;
using Stride.Media;
using Stride.Graphics;

namespace Stride_Asteroids
{
    public class Player : PlayerExplode
    {
        List<Shot> shotsScriptList = new List<Shot>();
        List<Entity> displayShipsList = new List<Entity>();
        Entity ship;
        Entity shipFlame;
        Model shipModel;
        ModelComponent shipMesh;
        ModelComponent flameMesh;
        SoundInstance fireSoundInstance;
        SoundInstance thrustSoundInstance;
        SoundInstance bonusSoundInstance;
        TimerTick flameTimer;
        float spawnRadius = 0.15f; //Large rock radius is 0.0453

        public List<Shot> Shots { get => shotsScriptList; }
        public float SpawnRadius { get => spawnRadius; }

        public override void Start()
        {
            base.Start();
            radius = 0.25f * 0.0666f;
            flameTimer = new TimerTick();
            InitilizeandCreateModels();
            Entity.AddChild(ship);
            MakeShots();
            InitializeAudio();
            Disable();
        }

        public override void Update()
        {
            if (IsActive())
            {
                base.Update();
                GetInput();
                CheckForEdge();
                CheckCollusion();
            }
            else
            {
                if (Main.instance.gameOver)
                {
                    if (Input.IsKeyPressed(Keys.Return))
                    {
                        Main.instance.NewGame();
                    }

                    return;
                }
                else if (CheckExplodeDone())
                {
                    if (CheckClearToSpawn())
                    {
                        Reset();
                    }

                    PlayerDoneExploding();
                    return;
                }
                else
                {
                    Main.instance.UFOControlScript.ResetTimer();
                }
            }
        }

        public bool IsActive()
        {
            if (shipMesh != null)
                return shipMesh.Enabled;
            else
                return false;
        }

        public void GotHit()
        {
            Explode();
            Disable();
            hit = true;
            Main.instance.PlayerLostLife();
        }

        public void ResetShip()
        {
            Reset();
        }

        public void ShipLives()
        {
            foreach (Entity ship in displayShipsList)
            {
                SceneSystem.SceneInstance.RootScene.Entities.Remove(ship);
            }

            displayShipsList.Clear();

            for (int ship = 0; ship < Main.instance.Lives; ship++)
            {
                displayShipsList.Add(new Entity());
                ModelComponent shipMesh = new ModelComponent(shipModel);
                displayShipsList[ship].Add(shipMesh);
                displayShipsList[ship].Transform.Position = new Vector3((edge.X * 0.4f) + (ship * 0.025f),
                    edge.Y - 0.075f, 0);
                //displayShipsList[ship].Transform.Scale = new Vector3(0.75f);
                displayShipsList[ship].Transform.RotationEulerXYZ = new Vector3(0, 0, MathUtil.PiOverTwo);
                SceneSystem.SceneInstance.RootScene.Entities.Add(displayShipsList[ship]);
            }
        }

        public void PlayBonusSound()
        {
            bonusSoundInstance.Play();
        }

        bool CheckCollusion()
        {
            if (Main.instance.UFOControlScript.UFOScript.shotScript.IsActive())
            {
                if (CirclesIntersect(Main.instance.UFOControlScript.UFOScript.shotScript.Position,
                    Main.instance.UFOControlScript.UFOScript.shotScript.Radius))
                {
                    GotHit();
                    Main.instance.UFOControlScript.UFOScript.shotScript.Disable();
                }
            }

            return false;
        }

        bool CheckClearToSpawn()
        {
            foreach (Rock rock in Main.instance.rockManagerScript.RocksList)
            {
                if (rock.CheckPlayerNotClearToSpawn() && rock.IsActive())
                {
                    return false;
                }
            }

            return true;
        }

        void PlayerDoneExploding()
        {
            Main.instance.UFOControlScript.UFOScript.Disable();
            Main.instance.UFOControlScript.UFOScript.shotScript.Disable();
            Main.instance.UFOControlScript.ResetTimer();
        }

        void Disable()
        {
            shipMesh.Enabled = false;
            ThrustOff();
            velocity = Vector3.Zero;
        }

        void GetInput()
        {
            float turnRate = 3.25f;

            if (Input.IsKeyDown(Keys.Left))
            {
                rotationVelocity = -turnRate;
            }
            else if (Input.IsKeyDown(Keys.Right))
            {
                rotationVelocity = turnRate;
            }
            else
            {
                rotationVelocity = 0;
            }

            if (Input.IsKeyPressed(Keys.LeftCtrl) || Input.IsKeyPressed(Keys.Space))
            {
                FireShot();
            }

            if (Input.IsKeyPressed(Keys.Down) || Input.IsKeyPressed(Keys.RightShift))
            {
                HyperSpace();
            }

            if (Input.IsKeyDown(Keys.Up))
            {
                ThrustOn();
            }
            else
            {
                ThrustOff();
            }
        }

        void ThrustOn()
        {
            if (thrustSoundInstance.PlayState != PlayState.Playing)
            {
                thrustSoundInstance.Play();
            }

            float maxPerSecond = 0.5f;
            float thrustAmount = 0.002f;

            if (Math.Abs(velocity.X) + Math.Abs(velocity.Y) < maxPerSecond)
            {
                velocity += Main.instance.VelocityFromAngle(rotation, thrustAmount);
                flameTimer.Tick();

                if (flameTimer.TotalTime.Milliseconds > 18)
                {
                    flameTimer.Reset();
                    flameMesh.Enabled = !flameMesh.Enabled;
                }
            }
            else
            {
                ThrustOff();
            }

        }

        void ThrustOff()
        {
            float deceleration = 0.0025f;
            velocity -= velocity * deceleration;
            flameMesh.Enabled = false;
        }

        void FireShot()
        {
            foreach (Shot shot in shotsScriptList)
            {
                if (!shot.IsActive())
                {
                    if (fireSoundInstance.PlayState != PlayState.Playing)
                    {
                        fireSoundInstance.Play();
                    }

                    float speed = 0.6f;
                    Vector3 dir = Main.instance.VelocityFromAngle(rotation, speed);
                    Vector3 offset = Main.instance.VelocityFromAngle(rotation, radius);
                    shot.Spawn(position + offset, dir + (velocity * 0.75f), 1.55f);
                    break;
                }
            }
        }

        void HyperSpace()
        {
            velocity = Vector3.Zero;
            position = new Vector3(Main.instance.RandomMinMax(-edge.X, edge.X),
                Main.instance.RandomMinMax(-edge.Y, edge.Y), 0);
            UpdatePR();
        }

        void Reset()
        {
            hit = false;
            shipMesh.Enabled = true;
            flameMesh.Enabled = false;
            position = Vector3.Zero;
            velocity = Vector3.Zero;
            UpdatePR();
        }

        void MakeShots()
        {
            Prefab shotPrefab = Content.Load<Prefab>("Prefabs/Shot");

            for (int i = 0; i < 4; i++)
            {
                Entity shot;
                shot = (shotPrefab.Instantiate().First());
                SceneSystem.SceneInstance.RootScene.Entities.Add(shot);
                shotsScriptList.Add(shot.Components.Get<Shot>());
            }
        }

        void InitilizeandCreateModels()
        {
            Buffer<VertexPositionNormalTexture> vBuffer;
            MeshDraw meshDraw;
            float multi = 0.0666f;
            float shipTip = 0.25f * multi;
            float shipBWidth = 0.15f * multi;
            float shipMid = -0.16f * multi;
            float shipMidWidth = 0.125f * multi;

            // VertexPositionNormalTexture is the layout that the engine uses in the shaders
            // Ship
            vBuffer = Stride.Graphics.Buffer.Vertex.New(GraphicsDevice, new VertexPositionNormalTexture[]
            {
                 new VertexPositionNormalTexture(new Vector3(-shipTip, shipBWidth, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), //Top back tip.
                 new VertexPositionNormalTexture(new Vector3(shipTip, 0, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), //Nose pointing to the left of screen.
                 new VertexPositionNormalTexture(new Vector3(-shipTip, -shipBWidth, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), //Bottom back tip.
                 new VertexPositionNormalTexture(new Vector3(shipMid, -shipMidWidth, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), //Bottom inside back.
                 new VertexPositionNormalTexture(new Vector3(shipMid, shipMidWidth, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), //Top inside back.
                 new VertexPositionNormalTexture(new Vector3(-shipTip, shipBWidth, 0), new Vector3(0, 1, 1), new Vector2(0, 0)) //Top Back Tip.
            });

            meshDraw = new MeshDraw
            {
                PrimitiveType = PrimitiveType.LineStrip, // Tell the GPU that this is a line.
                VertexBuffers = new[] { new VertexBufferBinding(vBuffer, VertexPositionNormalTexture.Layout, vBuffer.ElementCount) },
                DrawCount = vBuffer.ElementCount
            };

            Mesh mesh = new Mesh();
            mesh.Draw = meshDraw;
            shipModel = new Model();
            shipModel.Add(mesh);
            shipMesh = new ModelComponent(shipModel);
            ship = new Entity();
            ship.Add(shipMesh);

            float flame = -0.16f * multi;
            float flameTip = -0.36f * multi;
            float flameWidth = 0.075f * multi;

            // VertexPositionNormalTexture is the layout that the engine uses in the shaders
            // Flames
            vBuffer = Stride.Graphics.Buffer.Vertex.New(GraphicsDevice, new VertexPositionNormalTexture[]
            {
                 new VertexPositionNormalTexture(new Vector3(flame, -flameWidth, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), //Bottom inside back.
                 new VertexPositionNormalTexture(new Vector3(flameTip, 0, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), //Tip of flame.
                 new VertexPositionNormalTexture(new Vector3(flame, flameWidth, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), //Top inside back.
                 new VertexPositionNormalTexture(new Vector3(flameTip, 0, 0), new Vector3(0, 1, 1), new Vector2(0, 0)), //Tip of flame.
            });

            meshDraw = new MeshDraw
            {
                PrimitiveType = PrimitiveType.LineStrip, // Tell the GPU that this is a line.
                VertexBuffers = new[] { new VertexBufferBinding(vBuffer, VertexPositionNormalTexture.Layout, vBuffer.ElementCount) },
                DrawCount = vBuffer.ElementCount
            };

            mesh = new Mesh();
            mesh.Draw = meshDraw;
            Model model = new Model();
            model.Add(mesh);
            flameMesh = new ModelComponent(model);
            shipFlame = new Entity();
            shipFlame.Add(flameMesh);
            Entity.AddChild(shipFlame);
        }

        void InitializeAudio()
        {
            fireSoundInstance = Content.Load<Sound>("Sounds/PlayerFire").CreateInstance();
            fireSoundInstance.Volume = 0.15f;
            //fireSoundInstance.Pitch = 1.75f;

            thrustSoundInstance = Content.Load<Sound>("Sounds/Thrust").CreateInstance();
            thrustSoundInstance.Volume = 1.75f;
            //thrustSoundInstance.Pitch = 0.75f;

            bonusSoundInstance = Content.Load<Sound>("Sounds/BonusShip").CreateInstance();
            bonusSoundInstance.Volume = 0.5f;
            //bonusSoundInstance.Pitch = 3;
        }
    }
}
