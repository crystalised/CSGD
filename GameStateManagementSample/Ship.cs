#region File Description
//-----------------------------------------------------------------------------
// Ship.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;

#endregion

namespace CoinHunt
{
    public class Ship
    {
        #region Fields

        private const float MinimumAltitude = 370.0f;
        private const float MinX = -65000.0f;
        private const float MaxX = 65000.0f;
        private const float MinZ = -65000.0f;
        private const float MaxZ = 65000.0f;
        /// <summary>
        /// A reference to the graphics device used to access the viewport for touch input.
        /// </summary>
        private GraphicsDevice graphicsDevice;

        /// <summary>
        /// Location of ship in world space.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Direction ship is facing.
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// Ship's up vector.
        /// </summary>
        public Vector3 Up;

        private Vector3 right;
        /// <summary>
        /// Ship's right vector.
        /// </summary>
        public Vector3 Right
        {
            get { return right; }
        }

        /// <summary>
        /// Full speed at which ship can rotate; measured in radians per second.
        /// </summary>
        private const float RotationRate = 1.5f;

        /// <summary>
        /// Mass of ship.
        /// </summary>
        private const float Mass = 1.0f;

        /// <summary>
        /// Maximum force that can be applied along the ship's direction.
        /// </summary>
        private const float ThrustForce = 24000.0f;

        /// <summary>
        /// Velocity scalar to approximate drag.
        /// </summary>
        private const float DragFactor = 0.97f;

        /// <summary>
        /// Current ship velocity.
        /// </summary>
        public Vector3 Velocity;

        /// <summary>
        /// Ship world transform matrix.
        /// </summary>
        /// 
        public BoundingSphere shipBounds;

        public Model shipModel;

        public Matrix World
        {
            get { return world; }
        }
        private Matrix world;

        #endregion

        #region Initialization

        public Ship(GraphicsDevice device, Model inModel)
        {
            graphicsDevice = device;
            shipModel = inModel;
            Reset();
        }

        /// <summary>
        /// Restore the ship to its original starting state
        /// </summary>
        public void Reset()
        {
            Position = new Vector3(0, MinimumAltitude, 0);
            Direction = Vector3.Forward;
            Up = Vector3.Up;
            right = Vector3.Right;
            Velocity = Vector3.Zero;
        }

        public void setPos(Vector3 inPos)
        {
            Position = inPos;
        }

        public Vector3 getPos()
        {
            return Position;
        }

        #endregion

        bool TouchLeft()
        {
            MouseState mouseState = Mouse.GetState();
            return mouseState.LeftButton == ButtonState.Pressed &&
                mouseState.X <= graphicsDevice.Viewport.Width / 3;
        }

        bool TouchRight()
        {
            MouseState mouseState = Mouse.GetState();
            return mouseState.LeftButton == ButtonState.Pressed &&
                mouseState.X >= 2 * graphicsDevice.Viewport.Width / 3;
        }

        bool TouchDown()
        {
            MouseState mouseState = Mouse.GetState();
            return mouseState.LeftButton == ButtonState.Pressed &&
                mouseState.Y <= graphicsDevice.Viewport.Height / 3;
        }

        bool TouchUp()
        {
            MouseState mouseState = Mouse.GetState();
            return mouseState.LeftButton == ButtonState.Pressed &&
                mouseState.Y >= 2 * graphicsDevice.Viewport.Height / 3;
        }

        /// <summary>
        /// Applies a simple rotation to the ship and animates position based
        /// on simple linear motion physics.
        /// </summary>
        public void Update(GameTime gameTime, PlayerIndex playerIn)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(playerIn);
            MouseState mouseState = Mouse.GetState();

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;


            // Determine rotation amount from input
            Vector2 rotationAmount = -gamePadState.ThumbSticks.Left;

            if (playerIn == PlayerIndex.One)
            {
                if (keyboardState.IsKeyDown(Keys.Left) || TouchLeft())
                    rotationAmount.X = 1.0f;
                if (keyboardState.IsKeyDown(Keys.Right) || TouchRight())
                    rotationAmount.X = -1.0f;
                if (keyboardState.IsKeyDown(Keys.Up) || TouchUp())
                    rotationAmount.Y = -1.0f;
                if (keyboardState.IsKeyDown(Keys.Down) || TouchDown())
                    rotationAmount.Y = 1.0f;
            }
            else
            {
                if (keyboardState.IsKeyDown(Keys.A) || TouchLeft())
                    rotationAmount.X = 1.0f;
                if (keyboardState.IsKeyDown(Keys.D) || TouchRight())
                    rotationAmount.X = -1.0f;
                if (keyboardState.IsKeyDown(Keys.W) || TouchUp())
                    rotationAmount.Y = -1.0f;
                if (keyboardState.IsKeyDown(Keys.S) || TouchDown())
                    rotationAmount.Y = 1.0f;
            }

            // Scale rotation amount to radians per second
            rotationAmount = rotationAmount * RotationRate * elapsed;

            // Correct the X axis steering when the ship is upside down
            if (Up.Y < 0)
                rotationAmount.X = -rotationAmount.X;


            // Create rotation matrix from rotation amount
            Matrix rotationMatrix =
                Matrix.CreateFromAxisAngle(Right, rotationAmount.Y) *
                Matrix.CreateRotationY(rotationAmount.X);

            // Rotate orientation vectors
            Direction = Vector3.TransformNormal(Direction, rotationMatrix);
            Up = Vector3.TransformNormal(Up, rotationMatrix);

            // Re-normalize orientation vectors
            // Without this, the matrix transformations may introduce small rounding
            // errors which add up over time and could destabilize the ship.
            Direction.Normalize();
            Up.Normalize();

            // Re-calculate Right
            right = Vector3.Cross(Direction, Up);

            // The same instability may cause the 3 orientation vectors may
            // also diverge. Either the Up or Direction vector needs to be
            // re-computed with a cross product to ensure orthagonality
            Up = Vector3.Cross(Right, Direction);


            // Determine thrust amount from input
            float thrustAmount = gamePadState.Triggers.Right;
            if (playerIn == PlayerIndex.One)
            {
                if (keyboardState.IsKeyDown(Keys.Space))
                    thrustAmount = 1.0f;
            }
            else 
            {
                if (keyboardState.IsKeyDown(Keys.Q))
                    thrustAmount = 1.0f;
            }
           
            if (gamePadState.Triggers.Left > 0)
            {
                thrustAmount = -gamePadState.Triggers.Left;
            }
            

          

            // Calculate force from thrust amount
            Vector3 force = Direction * thrustAmount * ThrustForce;


            // Apply acceleration
            Vector3 acceleration = force / Mass;
            Velocity += acceleration * elapsed;

            // Apply psuedo drag
            Velocity *= DragFactor;

            // Apply velocity
            Position += Velocity * elapsed;

            // Prevent ship from flying under the ground
            Position.Y = Math.Max(Position.Y, MinimumAltitude);
            Position.X = Math.Max(Position.X, MinX);
            Position.X = Math.Min(Position.X, MaxX);
            Position.Z = Math.Max(Position.Z, MinZ);
            Position.Z = Math.Min(Position.Z, MaxZ);


            // Reconstruct the ship's world matrix
            world = Matrix.Identity;
            world.Forward = Direction;
            world.Up = Up;
            world.Right = right;
            world.Translation = Position;

            shipBounds = getBoundingSphere(shipModel);
        }

        public BoundingSphere getBoundingSphere(Model inModel)
        {
            BoundingSphere outSphere = new BoundingSphere();

            foreach (ModelMesh mesh in inModel.Meshes)
            {
                if (outSphere.Radius == 0)
                    outSphere = mesh.BoundingSphere;
                else 
                    outSphere = BoundingSphere.CreateMerged(outSphere, mesh.BoundingSphere);
            }
            outSphere.Center = Position;
            outSphere.Radius *= 1.0f;

            return outSphere;
        }

        public bool checkCollision(BoundingSphere inSphere)
        {
            if (inSphere.Intersects(shipBounds))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
