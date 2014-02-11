#region File Description
//-----------------------------------------------------------------------------
// LoadingScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
#endregion

namespace CoinHunt
{
    class ModelSelectionScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont hudFont;
        SpriteFont infoFont;

        Texture2D blank;

        Texture2D LTStick;

        bool playerOneReady = false;
        bool playerTwoReady = false;
        Vector2 p1TextPos, p2TextPos;

        //Viewport and matrixes for different players
        Viewport playerOneViewport;
        Viewport playerTwoViewport;
        Matrix playerOneView, playerOneProjection;
        Matrix playerTwoView, playerTwoProjection;

        KeyboardState oldState = Keyboard.GetState();

        //Values required for setting viewport
        public static int playerOneWidth, playerOneHeight, playerTwoWidth, playerTwoHeight;
        public static int playerOneX, playerOneY, playerTwoX, playerTwoY;

        //Value to identify player one model
        public static int playerOneModel = 0;
        public static int playerTwoModel = 1;

        //Array for models
        Model[] shipModels;

        #endregion

        #region Initialization

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            hudFont = content.Load<SpriteFont>("Fonts/hudfont");
            infoFont = content.Load<SpriteFont>("Fonts/infofont");

            blank = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            blank.SetData(new[] { Color.White });

            LTStick = content.Load<Texture2D>("Textures/LTStick");

            shipModels = new Model[3];

            // load content 
            shipModels[0] = content.Load<Model>("Models/ship");
            shipModels[1] = content.Load<Model>("SpaceShip");
            shipModels[2] = content.Load<Model>("Models/T-88");

            if (OptionsMenuScreen.currentSST == OptionsMenuScreen.splitScreenType.Horizontal)
            {
                playerOneWidth = ScreenManager.GraphicsDevice.Viewport.Width;
                playerOneHeight = ScreenManager.GraphicsDevice.Viewport.Height / 2;
                playerOneX = 0;
                playerOneY = 0;

                playerTwoWidth = playerOneWidth;
                playerTwoHeight = playerOneHeight;
                playerTwoX = 0;
                playerTwoY = ScreenManager.GraphicsDevice.Viewport.Height / 2;
            }
            else
            {
                playerOneWidth = ScreenManager.GraphicsDevice.Viewport.Width / 2;
                playerOneHeight = ScreenManager.GraphicsDevice.Viewport.Height;
                playerOneX = 0;
                playerOneY = 0;

                playerTwoWidth = playerOneWidth;
                playerTwoHeight = playerOneHeight;
                playerTwoX = ScreenManager.GraphicsDevice.Viewport.Width / 2;
                playerTwoY = 0;
            }

            //Create the viewport
            playerOneViewport = new Viewport
            {
                MinDepth = 0,
                MaxDepth = 1,
                X = playerOneX,
                Y = playerOneY,
                Width = playerOneWidth,
                Height = playerOneHeight,
            };
            playerTwoViewport = new Viewport
            {
                MinDepth = 0,
                MaxDepth = 1,
                X = playerTwoX,
                Y = playerTwoY,
                Width = playerTwoWidth,
                Height = playerTwoHeight,
            };

            playerOneView = Matrix.CreateLookAt(
                new Vector3(0f, 800f, 800f),
                Vector3.Zero,
                Vector3.Up);
            playerOneProjection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, playerOneViewport.AspectRatio, 10f, 5000f);

            playerTwoView = Matrix.CreateLookAt(
                new Vector3(0f, 800f, 800f),
                Vector3.Zero,
                Vector3.Up);
            playerTwoProjection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, playerTwoViewport.AspectRatio, 10f, 5000f);

            Thread.Sleep(1000);

            ScreenManager.Game.ResetElapsedTime();
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update, Input and Draw


        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            float time = (float)gameTime.TotalGameTime.TotalSeconds;

            playerOneView = Matrix.CreateLookAt(
                new Vector3((float)Math.Cos(time), 1f, (float)Math.Sin(time)) * 2500f,
                Vector3.Zero,
                Vector3.Up);

            playerTwoView = Matrix.CreateLookAt(
                new Vector3((float)Math.Cos(time), 1f, (float)Math.Sin(time)) * 2500f,
                Vector3.Zero,
                Vector3.Up);
        }

        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState playerOneGamepadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !playerOneGamepadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);


                GamePad.SetVibration((PlayerIndex)playerIndex, 0, 0);
            }
            else
            {
                PlayerIndex dummy;
                // Player 1
                if (!playerOneReady)
                {
                    if (input.IsSelectLeft((PlayerIndex)playerIndex) || input.IsMenuUp((PlayerIndex)playerIndex))
                    {
                        ScreenManager.soundCue = ScreenManager.soundBank.GetCue("clickCue");
                        ScreenManager.soundCue.Play();

                        if (!oldState.IsKeyDown(Keys.Left))
                        {
                            playerOneModel--;
                            if (playerOneModel < 0)
                                playerOneModel = 2;
                        }
                    }

                    if (input.IsSelectRight((PlayerIndex)playerIndex) || input.IsMenuDown((PlayerIndex)playerIndex))
                    {
                        ScreenManager.soundCue = ScreenManager.soundBank.GetCue("clickCue");
                        ScreenManager.soundCue.Play();

                        if (!oldState.IsKeyDown(Keys.Right))
                        {
                            playerOneModel++;
                            if (playerOneModel > 2)
                                playerOneModel = 0;
                        }
                    }

                    if (input.IsMenuSelect((PlayerIndex)playerIndex, out dummy) || keyboardState.IsKeyDown(Keys.P))
                    {
                        playerOneReady = true;
                    }
                }

                // Player 2
                if (ScreenManager.PlayerTwo == -1)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (i != (int)ControllingPlayer)
                        {
                            if (input.CurrentGamePadStates[i].Buttons.Start == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Enter))
                            {
                                ScreenManager.PlayerTwo = i;
                            }
                        }
                    }
                }
                else
                {
                    GamePadState playerTwoGamepadState = input.CurrentGamePadStates[ScreenManager.PlayerTwo];
                    if (!playerTwoReady)
                    {
                        if (input.IsSelectLeft((PlayerIndex)ScreenManager.PlayerTwo) || input.IsMenuUp((PlayerIndex)ScreenManager.PlayerTwo))
                        {
                            ScreenManager.soundCue = ScreenManager.soundBank.GetCue("clickCue");
                            ScreenManager.soundCue.Play();

                            if (!oldState.IsKeyDown(Keys.A))
                            {
                                playerTwoModel--;
                                if (playerTwoModel < 0)
                                    playerTwoModel = 2;
                            }
                        }

                        if (input.IsSelectRight((PlayerIndex)ScreenManager.PlayerTwo) || input.IsMenuDown((PlayerIndex)ScreenManager.PlayerTwo))
                        {
                            ScreenManager.soundCue = ScreenManager.soundBank.GetCue("clickCue");
                            ScreenManager.soundCue.Play();

                            if (!oldState.IsKeyDown(Keys.D))
                            {
                                playerTwoModel++;
                                if (playerTwoModel > 2)
                                    playerTwoModel = 0;
                            }
                        }
                        if (input.IsMenuSelect((PlayerIndex)ScreenManager.PlayerTwo, out dummy) || keyboardState.IsKeyDown(Keys.X))
                        {
                            playerTwoReady = true;
                        }
                    }
                }

                if (playerOneReady && playerTwoReady)
                {
                    LoadingScreen.Load(ScreenManager, true, ControllingPlayer,
                               new GameplayScreen());
                }

                oldState = keyboardState;
            }
        }

        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // reset any important states 
            ScreenManager.GraphicsDevice.DepthStencilState =
            DepthStencilState.Default;
            ScreenManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            ScreenManager.GraphicsDevice.BlendState = BlendState.Opaque;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle safeArea = viewport.TitleSafeArea;

            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(Color.DarkSlateGray);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;


            String pressStart = "Press Start";
            Vector2 pressStartPos = new Vector2(safeArea.Bottom - 50, viewport.Height - 200);

            // Draw our scene with all of our viewports and their respective view/projection matrices.
            DrawScene(gameTime, playerOneViewport, playerOneView, playerOneProjection, playerOneModel);
            if (ScreenManager.PlayerTwo != -1)
                DrawScene(gameTime, playerTwoViewport, playerTwoView, playerTwoProjection, playerTwoModel);

            DrawViewportEdges(playerOneViewport);
            DrawViewportEdges(playerTwoViewport);

            Vector2 LTLocation = new Vector2(safeArea.Left + 50, safeArea.Top + 50);
            Vector2 hudText = new Vector2(safeArea.Left + 150, safeArea.Top + 75);
            Vector2 infoText = new Vector2(safeArea.Left, safeArea.Bottom - 150);
            if (OptionsMenuScreen.currentSST == OptionsMenuScreen.splitScreenType.Horizontal)
            {
                p1TextPos = new Vector2(safeArea.Right - 400, viewport.Height - 560);
                p2TextPos = new Vector2(safeArea.Right - 400, viewport.Height - 200);
            }
            else
            {
                p1TextPos = new Vector2(viewport.Width / 4 - 60, safeArea.Top + 125);
                p2TextPos = new Vector2(viewport.Width / 1 / 2 + 240, safeArea.Top + 125);
            }

            String infoStr = "Instructions: 25 Coins are \nscattered around the map \nCompete against each other \nto see who collects more!";
            String HudStr = "To Select Model";
            String ready = "Ready";

            spriteBatch.Begin();

            spriteBatch.Draw(LTStick, LTLocation, Color.White);

            spriteBatch.DrawString(infoFont, HudStr, hudText, Color.BurlyWood);
            spriteBatch.DrawString(infoFont, infoStr, infoText, Color.DeepSkyBlue);

            if (ScreenManager.PlayerTwo == -1)
                spriteBatch.DrawString(infoFont, pressStart, pressStartPos, Color.BurlyWood);

            if (playerOneReady)
            {
                spriteBatch.DrawString(hudFont, ready,
                                        p1TextPos, Color.Black);
                spriteBatch.DrawString(hudFont, ready,
                                    (p1TextPos - new Vector2(1, 1)), Color.IndianRed);
            }

            if (playerTwoReady)
            {
                spriteBatch.DrawString(hudFont, ready,
                                        p2TextPos, Color.Black);
                spriteBatch.DrawString(hudFont, ready,
                                    (p2TextPos - new Vector2(1, 1)), Color.IndianRed);
            }

            spriteBatch.End();
        }

        private void DrawScene(GameTime gameTime, Viewport viewport, Matrix view, Matrix projection, int playerIndex)
        {
            // Set our viewport. We store the old viewport so we can restore it when we're done in case
            // we want to render to the full viewport at some point.
            Viewport oldViewport = ScreenManager.GraphicsDevice.Viewport;
            ScreenManager.GraphicsDevice.Viewport = viewport;

            // Here we'd want to draw our entire scene. For this sample, that's just the tank.
            DrawModel(shipModels[playerIndex], playerOneViewport, view, projection);

            // Now that we're done, set our old viewport back on the device
            ScreenManager.GraphicsDevice.Viewport = oldViewport;
        }

        private void DrawModel(Model model, Viewport viewport, Matrix view, Matrix projection)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = transforms[mesh.ParentBone.Index];
                    // Use the matrices provided by the chase camera 
                    effect.View = view;
                    effect.Projection = projection;
                    effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                    effect.SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
                    effect.SpecularPower = 0.5f;
                }
                mesh.Draw();
            }
        }

        private void DrawViewportEdges(Viewport viewport)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            const int edgeWidth = 2;

            // We now compute four rectangles that make up our edges
            Rectangle topEdge = new Rectangle(
                viewport.X - edgeWidth / 2,
                viewport.Y - edgeWidth / 2,
                viewport.Width + edgeWidth,
                edgeWidth);
            Rectangle bottomEdge = new Rectangle(
                viewport.X - edgeWidth / 2,
                viewport.Y + viewport.Height - edgeWidth / 2,
                viewport.Width + edgeWidth,
                edgeWidth);
            Rectangle leftEdge = new Rectangle(
                viewport.X - edgeWidth / 2,
                viewport.Y - edgeWidth / 2,
                edgeWidth,
                viewport.Height + edgeWidth);
            Rectangle rightEdge = new Rectangle(
                viewport.X + viewport.Width - edgeWidth / 2,
                viewport.Y - edgeWidth / 2,
                edgeWidth,
                viewport.Height + edgeWidth);

            // We just use SpriteBatch to draw the four rectangles
            spriteBatch.Begin();
            spriteBatch.Draw(blank, topEdge, Color.Black);
            spriteBatch.Draw(blank, bottomEdge, Color.Black);
            spriteBatch.Draw(blank, leftEdge, Color.Black);
            spriteBatch.Draw(blank, rightEdge, Color.Black);
            spriteBatch.End();
        }

        #endregion
    }
}