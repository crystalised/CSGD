#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;

#endregion

namespace CoinHunt
{
    class GameplayScreen : GameScreen
    {
        #region Fields
        String playerName;

        ContentManager content;
        SpriteFont gameFont;
        SpriteFont hudFont;

        Texture2D blank;

        // Viewport and camera for different players
        Viewport playerOneViewport;
        Viewport playerTwoViewport;
        ChaseCamera camera0;
        ChaseCamera camera1;
        bool camera0SpringEnabled = true;
        bool camera1SpringEnabled = true;

        // Game variables for different players
        int p1Score, p2Score;
        int coinsLeft;

        SpriteFont spriteFont;
        Ship[] ship;
        Model[] shipModels;
        Model coinModel;
        Coin[] coinArray;
        Model groundModel;

        //Basic Effect for ground and coins
        BasicEffect effect;

        //Environment Map Effect for ships
        EnvironmentMapEffect envEffect;
        TextureCube texture;

        Random random = new Random((int)DateTime.Now.Ticks);

        bool gameOver = false;

        float pauseAlpha;

        #endregion

        #region Initialization

        static DateTime startVibration;
        TimeSpan timePassed;
        bool isVibrating = false;

        static DateTime startVibration2;
        TimeSpan timePassed2;
        bool isVibrating2 = false;

        //Bloom settings preset
        int bloomSettingsIndex = 0;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            #region load content
            // load content
            gameFont = content.Load<SpriteFont>("Fonts/gamefont");
            hudFont = content.Load<SpriteFont>("Fonts/hudfont");
            shipModels = new Model[3];
            spriteFont = content.Load<SpriteFont>("Fonts/gamefont");
            shipModels[0] = content.Load<Model>("Models/ship");
            shipModels[1] = content.Load<Model>("SpaceShip");
            shipModels[2] = content.Load<Model>("Models/T-88");
            groundModel = content.Load<Model>("Models/Ground");
            coinModel = content.Load<Model>("Models/TyveKrone");

            blank = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            blank.SetData(new[] { Color.White });

            #endregion

            #region Setup basic effect

            //Setup Basic Effect for ground
            effect = new BasicEffect(ScreenManager.GraphicsDevice);
            effect.LightingEnabled = true;
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.DiffuseColor = Vector3.Normalize(new Vector3(7, 7, 7));
            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0, -2.5f, 0));
            effect.DirectionalLight0.SpecularColor = Color.GreenYellow.ToVector3();
            effect.SpecularColor = Color.CadetBlue.ToVector3();
            effect.SpecularPower = 20.0f;
            effect.PreferPerPixelLighting = true;
            effect.FogEnabled = true;
            effect.FogColor = Color.Black.ToVector3();
            effect.FogStart = 10000;
            effect.FogEnd = 50000;

            #endregion

            #region Setup Environment map effect

            //Setup Environment Map Effect for ship
            envEffect = new EnvironmentMapEffect(ScreenManager.GraphicsDevice);
            texture = new TextureCube(ScreenManager.GraphicsDevice, 256, false, SurfaceFormat.Color);
            Color[] facedata = new Color[256 * 256];
            for (int i = 0; i < 6; i++)
            {
                envEffect.Texture = content.Load<Texture2D>("SkyboxesTex/skybox" + i.ToString());
                envEffect.Texture.GetData<Color>(facedata);
                texture.SetData<Color>((CubeMapFace)i, facedata);
            }
            for (int i = 0; i < 3; i++)
            {
                envEffect.Texture = (shipModels[i].Meshes[0].Effects[0] as BasicEffect).Texture;
            }
            envEffect.EnvironmentMap = texture;
            envEffect.EnableDefaultLighting();
            envEffect.EnvironmentMapAmount = 1.0f;
            envEffect.FresnelFactor = 1.0f;
            envEffect.EnvironmentMapSpecular = Vector3.Zero;

            #endregion

            #region Setup the ships

            // setup the ship
            ship = new Ship[3];

            ship[0] = new Ship(ScreenManager.GraphicsDevice, shipModels[ModelSelectionScreen.playerOneModel]);
            ship[1] = new Ship(ScreenManager.GraphicsDevice, shipModels[ModelSelectionScreen.playerTwoModel]);

            ship[0].setPos(new Vector3(1000, 500, 0));
            ship[1].setPos(new Vector3(-1000, 500, 0));

            #endregion

            #region Split viewports

            // Setup player 1 and 2 viewport
            playerOneViewport = new Viewport
            {
                MinDepth = 0,
                MaxDepth = 1,
                X = ModelSelectionScreen.playerOneX,
                Y = ModelSelectionScreen.playerOneY,
                Width = ModelSelectionScreen.playerOneWidth,
                Height = ModelSelectionScreen.playerOneHeight,
            };
            playerTwoViewport = new Viewport
            {
                MinDepth = 0,
                MaxDepth = 1,
                X = ModelSelectionScreen.playerTwoX,
                Y = ModelSelectionScreen.playerTwoY,
                Width = ModelSelectionScreen.playerTwoWidth,
                Height = ModelSelectionScreen.playerTwoHeight,
            };

            // setup the camera 1 and 2
            camera0 = new ChaseCamera();
            camera0.DesiredPositionOffset = new Vector3(0.0f, 2000.0f, 3500.0f);
            camera0.LookAtOffset = new Vector3(0.0f, 150.0f, 0.0f);
            camera0.NearPlaneDistance = 10.0f;
            camera0.FarPlaneDistance = 100000.0f;
            if (OptionsMenuScreen.currentSST == OptionsMenuScreen.splitScreenType.Horizontal)
            {
                camera0.AspectRatio =
                (float)ScreenManager.GraphicsDevice.Viewport.Width * 2 /
                       ScreenManager.GraphicsDevice.Viewport.Height;
            }
            else
            {
                camera0.AspectRatio =
                (float)ScreenManager.GraphicsDevice.Viewport.Width /
                       ScreenManager.GraphicsDevice.Viewport.Height / 2;
            }

            camera1 = new ChaseCamera();
            camera1.DesiredPositionOffset = new Vector3(0.0f, 2000.0f, 3500.0f);
            camera1.LookAtOffset = new Vector3(0.0f, 150.0f, 0.0f);
            camera1.NearPlaneDistance = 10.0f;
            camera1.FarPlaneDistance = 100000.0f;
            if (OptionsMenuScreen.currentSST == OptionsMenuScreen.splitScreenType.Horizontal)
            {
                camera1.AspectRatio =
                (float)ScreenManager.GraphicsDevice.Viewport.Width * 4 /
               ScreenManager.GraphicsDevice.Viewport.Height / 2;
            }
            else
            {
                camera1.AspectRatio =
                (float)ScreenManager.GraphicsDevice.Viewport.Width /
               ScreenManager.GraphicsDevice.Viewport.Height / 2;
            }

      
            // Perform an inital reset on the camera so that it starts at the 
            // resting position. If we don't do this, the camera will start 
            // at the origin and race across the world to get behind the 
            // chased object. This is performed here because the aspect ratio 
            // is needed by Reset. 
            camera0.UpdateCameraChaseTarget(ship[0]);
            camera1.UpdateCameraChaseTarget(ship[1]);
            camera0.Reset();
            camera1.Reset();

            #endregion

            #region Setup coins

            coinArray = new Coin[25];
            coinsLeft = 25;

            for (int i = 0; i < 25; i++)
            {
                coinArray[i] = new Coin(ScreenManager.GraphicsDevice);
                coinArray[i].Location.X = random.Next(-60000, 60000);
                coinArray[i].Location.Y = random.Next(  6000, 20000);
                coinArray[i].Location.Z = random.Next(-60000, 60000);
            }

            #endregion

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw
        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
            int playerIndex = (int)ControllingPlayer.Value;
            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            #region Game Active

            if (IsActive)
            {
                //Enable bloomeffect in gameplayscreen
                ScreenManager.bloom.Enabled = true;
                ScreenManager.bloom.Visible = true;
                ScreenManager.bloom.Settings = BloomSettings.PresetSettings[bloomSettingsIndex];

                if (coinsLeft == 0)
                    gameOver = true;

                // Update the ships
                if (!gameOver)
                {
                    ship[0].Update(gameTime, (PlayerIndex)playerIndex);
                    ship[1].Update(gameTime, (PlayerIndex)ScreenManager.PlayerTwo);
                }

                //collison check for players and coin
                for (int c = 0; c < 25; c++)
                {
                    if (coinArray[c].isAlive)
                    {
                        coinArray[c].Update(gameTime);
                        if (ship[0].checkCollision(coinArray[c].getBoundingSphere(coinModel)))
                        {
                            if (OptionsMenuScreen.vibrate == true)
                            {
                                if (isVibrating == false)
                                {
                                    GamePad.SetVibration((PlayerIndex)playerIndex, 1.0f, 1.0f);
                                    isVibrating = true;
                                    startVibration = (DateTime.Now);
                                }
                            }

                            ScreenManager.soundCue = ScreenManager.soundBank.GetCue("coinCue");
                            ScreenManager.soundCue.Play();

                            coinArray[c].isAlive = false;
                            p1Score += 1;
                            coinsLeft--;
                        }

                        if (ship[1].checkCollision(coinArray[c].getBoundingSphere(coinModel)))
                        {
                            if (OptionsMenuScreen.vibrate == true)
                            {
                                if (isVibrating2 == false)
                                {
                                    GamePad.SetVibration((PlayerIndex)ScreenManager.PlayerTwo, 1.0f, 1.0f);
                                    isVibrating2 = true;
                                    startVibration2 = (DateTime.Now);
                                }
                            }

                            ScreenManager.soundCue = ScreenManager.soundBank.GetCue("coinCue");
                            ScreenManager.soundCue.Play();

                            coinArray[c].isAlive = false;
                            p2Score += 1;
                            coinsLeft--;
                        }
                    }
                }
                // Update the camera to chase the new target 
                camera0.UpdateCameraChaseTarget(ship[0]);
                camera1.UpdateCameraChaseTarget(ship[1]);
                // The chase camera's update behavior is the springs, but we
                // can use the Reset method to have a locked, spring-less 
                // camera 
                if (camera0SpringEnabled)
                    camera0.Update(gameTime);
                if (camera1SpringEnabled)
                    camera1.Update(gameTime);
                else
                {
                    camera0.Reset();
                    camera1.Reset();
                }
            }

            #endregion

            #region Game Over
            if (gameOver)
            {
                //Disable bloomeffect when not in game over
                ScreenManager.bloom.Enabled = false;
                ScreenManager.bloom.Visible = false;
            }
            #endregion

            #region Vibration control

            if (isVibrating)
            {
                timePassed = DateTime.Now - startVibration;
                if (timePassed.TotalSeconds >= 0.5f)
                {
                    GamePad.SetVibration((PlayerIndex)playerIndex, 0f, 0f);
                    isVibrating = false;
                }
            }
            if (isVibrating2)
            {
                timePassed2 = DateTime.Now - startVibration2;
                if (timePassed2.TotalSeconds >= 0.5f)
                {
                    GamePad.SetVibration((PlayerIndex)ScreenManager.PlayerTwo, 0f, 0f);
                    isVibrating2 = false;
                }
            }

            #endregion
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            SignedInGamer gamer = Gamer.SignedInGamers[playerIndex]; 
            if (gamer != null) 
                playerName = gamer.Gamertag; 
            else 
                Guide.ShowSignIn(1, false);

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(null) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), null);
                ScreenManager.bloom.Enabled = false;
                ScreenManager.bloom.Visible = false;
                GamePad.SetVibration((PlayerIndex)playerIndex, 0, 0);
            }
            else
            {
                if (keyboardState.IsKeyDown(Keys.R))
                {
                    coinsLeft = 0;
                    p1Score = 25;
                }
            }

            // Pressing the A button or key toggles the spring behavior
            // on and off 
            PlayerIndex dummy;
            if (input.IsNewKeyPress(Keys.Z, (PlayerIndex)playerIndex, out dummy) ||
            input.IsNewButtonPress(Buttons.Y, (PlayerIndex)playerIndex,
            out dummy))
            {
                camera0SpringEnabled = !camera0SpringEnabled;
            }
            if (input.IsNewKeyPress(Keys.X, (PlayerIndex)ScreenManager.PlayerTwo, out dummy) ||
            input.IsNewButtonPress(Buttons.Y, (PlayerIndex)ScreenManager.PlayerTwo,
            out dummy))
            {
                camera1SpringEnabled = !camera1SpringEnabled;
            }

            if (input.IsNewKeyPress(Keys.C, (PlayerIndex)playerIndex, out dummy) ||
            input.IsNewButtonPress(Buttons.Y, (PlayerIndex)playerIndex,
            out dummy))
            {
                bloomSettingsIndex = (bloomSettingsIndex + 1) %
                                     BloomSettings.PresetSettings.Length;

                ScreenManager.bloom.Settings = BloomSettings.PresetSettings[bloomSettingsIndex];
            }

            #region gameover

            if (gameOver)
            {
                if (input.IsMenuSelect((PlayerIndex)playerIndex, out dummy) || input.IsMenuSelect((PlayerIndex)ScreenManager.PlayerTwo, out dummy))
                {
                    LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new MainMenuScreen());
                }
            }

            #endregion
        }

        /// <summary>
        /// Draws the gameplay screen.
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

            //Start bloom effect draw
            ScreenManager.bloom.BeginDraw();

            // This game has a black background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            DrawScene(gameTime, playerOneViewport, camera0.View, camera0.Projection);
            DrawScene(gameTime, playerTwoViewport, camera1.View, camera1.Projection);

            DrawViewportEdges(playerOneViewport);
            DrawViewportEdges(playerTwoViewport);

            DrawOverlayText();
#if DEBUG
            DrawOverlayTextDebug(); 
#endif

            #region Draw Sprite

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            String gameOver = "Game Over!";
            String gameWin = "You win!";
            String gameLose = "You lose!";
            Texture2D buttonA = content.Load<Texture2D>("Textures/ButtonA");
            Vector2 aButtonPos = new Vector2(580, 345);
            Vector2 exitTxtPos = new Vector2(625, 335);

            Vector2 p1TextPos, p2TextPos, gameOverTextPos;

            if (OptionsMenuScreen.currentSST == OptionsMenuScreen.splitScreenType.Horizontal)
            {
                gameOverTextPos = new Vector2(viewport.Width / 2 - 100, safeArea.Top);
                p1TextPos = new Vector2(viewport.Width / 2 - 100, viewport.Height - 560);
                p2TextPos = new Vector2(viewport.Width / 2 - 100, viewport.Height - 200);
            }
            else
            {
                gameOverTextPos = new Vector2(viewport.Width / 2 - 100, safeArea.Top);
                p1TextPos = new Vector2(viewport.Width / 4 - 80, viewport.Height - 420);
                p2TextPos = new Vector2(viewport.Width / 1/2 + 240, viewport.Height - 420);
            }

            if (coinsLeft == 0)
            {
                if (p1Score > p2Score)
                {
                    spriteBatch.DrawString(hudFont, gameOver, gameOverTextPos, Color.Black);
                    spriteBatch.DrawString(hudFont, gameOver, (gameOverTextPos - new Vector2(1, 1)), Color.IndianRed);

                    spriteBatch.DrawString(hudFont, gameWin, p1TextPos, Color.Black);
                    spriteBatch.DrawString(hudFont, gameWin, (p1TextPos - new Vector2(1, 1)), Color.IndianRed);

                    spriteBatch.DrawString(hudFont, gameLose,
                                        p2TextPos, Color.Black);
                    spriteBatch.DrawString(hudFont, gameLose,
                                        (p2TextPos - new Vector2(1, 1)), Color.IndianRed);

                    spriteBatch.Draw(buttonA, aButtonPos, Color.White);
                    spriteBatch.DrawString(hudFont, "Exit", exitTxtPos, Color.LawnGreen);
                }
                else
                {
                    spriteBatch.DrawString(hudFont, gameOver, gameOverTextPos, Color.Black);
                    spriteBatch.DrawString(hudFont, gameOver, (gameOverTextPos - new Vector2(1, 1)), Color.IndianRed);

                    spriteBatch.DrawString(hudFont, gameLose, p1TextPos, Color.Black);
                    spriteBatch.DrawString(hudFont, gameLose, (p1TextPos - new Vector2(1, 1)), Color.IndianRed);

                    spriteBatch.DrawString(hudFont, gameWin,
                                        p2TextPos, Color.Black);
                    spriteBatch.DrawString(hudFont, gameWin,
                                        (p2TextPos - new Vector2(1, 1)), Color.IndianRed);

                    spriteBatch.Draw(buttonA, aButtonPos, Color.White);
                    spriteBatch.DrawString(hudFont, "Exit", exitTxtPos, Color.LawnGreen);
                }
            }

            spriteBatch.End();

            #endregion

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        private void DrawScene(GameTime gameTime, Viewport viewport, Matrix view, Matrix projection)
        {
            // Set our viewport. We store the old viewport so we can restore it when we're done in case
            // we want to render to the full viewport at some point.
            Viewport oldViewport = ScreenManager.GraphicsDevice.Viewport;
            ScreenManager.GraphicsDevice.Viewport = viewport;

            // Here we'd want to draw our entire scene. For this sample, that's just the tank.
            DrawShip(ship[0].shipModel, ship[0].World, view, projection, envEffect);
            DrawShip(ship[1].shipModel, ship[1].World, view, projection, envEffect);

            //Draw the coins
            for (int m = 0; m < 25; m++)
            {
                if (coinArray[m].isAlive)
                {
                    DrawModel(coinModel, coinArray[m].World, view, projection);
                }
            }

            //Draw the ground
            DrawModel(groundModel, Matrix.Identity, view, projection, effect);
            // Now that we're done, set our old viewport back on the device
            ScreenManager.GraphicsDevice.Viewport = oldViewport;
        }

        //Draw ship which uses Environment Map Effect
        private void DrawShip(Model m, Matrix world, Matrix view, Matrix projection, EnvironmentMapEffect be)
        {
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms); 
            foreach (ModelMesh mm in m.Meshes)
            {
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    be.World = transforms[mm.ParentBone.Index] * world;
                    be.View = view;
                    be.Projection = projection;
                    ScreenManager.GraphicsDevice.SetVertexBuffer(mmp.VertexBuffer, mmp.VertexOffset);
                    ScreenManager.GraphicsDevice.Indices = mmp.IndexBuffer;
                    be.CurrentTechnique.Passes[0].Apply();
                    ScreenManager.GraphicsDevice.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList, 0, 0,
                        mmp.NumVertices, mmp.StartIndex, mmp.PrimitiveCount);
                }
            }
        }

        //Draw model which uses basic effect (coins/ground)
        private void DrawModel(Model m, Matrix world, Matrix view, Matrix projection, BasicEffect be)
        {
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mm in m.Meshes)
            {
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    be.World = transforms[mm.ParentBone.Index] * world;
                    be.View = view;
                    be.Projection = projection;
                    ScreenManager.GraphicsDevice.SetVertexBuffer(mmp.VertexBuffer, mmp.VertexOffset);
                    ScreenManager.GraphicsDevice.Indices = mmp.IndexBuffer;
                    be.CurrentTechnique.Passes[0].Apply();
                    ScreenManager.GraphicsDevice.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList, 0, 0,
                        mmp.NumVertices, mmp.StartIndex, mmp.PrimitiveCount);
                }
            }
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    // Use the matrices provided by the chase camera
                    effect.View = view;
                    effect.Projection = projection;
                    effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
                    effect.SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
                    effect.SpecularPower = 0.5f;

                    effect.FogEnabled = true;
                    effect.FogColor = Color.Black.ToVector3();
                    effect.FogStart = 10000;
                    effect.FogEnd = 50000;
                }
                mesh.Draw();
            }
        }
 
        private void DrawOverlayText()
        {
            ScreenManager.SpriteBatch.Begin();
#if XBOX360
            if (PlayerIndex controllingPlayer == PlayerIndex.One) //check if first controller is player1
            {
                string p1Hud = playerName + "\nCoins Collected: " + p1Score;
                string p2Hud = "Player2\nCoins Collected: " + p2Score;
            }
            else
            {
                string p1Hud = "Player1\nCoins Collected: " + p1Score;
                string p2Hud = playerName + "\nCoins Collected: " + p2Score;
            }
#else
            string p1Hud = "Player 1\nCoins Collected: " + p1Score;
            string p2Hud = "Player2\nCoins Collected: " + p2Score;
#endif

            Viewport viewport = ScreenManager.Game.GraphicsDevice.Viewport;

            Rectangle safeArea = viewport.TitleSafeArea;

            Vector2 p1HudPos, p2HudPos;

            if (OptionsMenuScreen.currentSST == OptionsMenuScreen.splitScreenType.Horizontal)
            {
                p1HudPos = new Vector2(safeArea.Left, safeArea.Top);
                p2HudPos = new Vector2(safeArea.Left, safeArea.Top + 360);
            }
            else
            {
                p1HudPos = new Vector2(safeArea.Left, safeArea.Top);
                p2HudPos = new Vector2(safeArea.Left + 640, safeArea.Top);
            }
            
            // Draw the string twice to create a drop shadow, first colored 
            // black and offset one pixel to the bottom right, then again in 
            // white at the intended position. This makes text easier to read 
            // over the background. 
            ScreenManager.SpriteBatch.DrawString(spriteFont, p1Hud,
            p1HudPos, Color.Black);
            ScreenManager.SpriteBatch.DrawString(spriteFont, p1Hud,
            (p1HudPos - new Vector2(1, 1)), Color.White);

            ScreenManager.SpriteBatch.DrawString(spriteFont, p2Hud,
            p2HudPos, Color.Black);
            ScreenManager.SpriteBatch.DrawString(spriteFont, p2Hud,
            (p2HudPos - new Vector2(1, 1)), Color.White);
            ScreenManager.SpriteBatch.End();
        } 

        private void DrawOverlayTextDebug() 
        {
            ScreenManager.SpriteBatch.Begin();
            string text = FrameRateMonitor.details;

            // Draw the string twice to create a drop shadow, first colored 
            // black and offset one pixel to the bottom right, then again in 
            // white at the intended position. This makes text easier to read 
            // over the background. 
            ScreenManager.SpriteBatch.DrawString(spriteFont, text,
            new Vector2(65, 85), Color.Black);
            ScreenManager.SpriteBatch.DrawString(spriteFont, text,
            new Vector2(64, 84), Color.White);
            ScreenManager.SpriteBatch.End();
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
            spriteBatch.Draw(blank, topEdge, Color.White);
            spriteBatch.Draw(blank, bottomEdge, Color.White);
            spriteBatch.Draw(blank, leftEdge, Color.White);
            spriteBatch.Draw(blank, rightEdge, Color.White);
            spriteBatch.End();
        }

        #endregion
    }
}
