#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
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
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace CoinHunt
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class CreditsMenuScreen : MenuScreen
    {
        #region Fields

        SpriteFont gameFont;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public CreditsMenuScreen()
            : base("Credits")
        {
            MenuEntry back = new MenuEntry("Back");

            back.Selected += OnCancel;

            MenuEntries.Add(back);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            gameFont = content.Load<SpriteFont>("Fonts/gamefont");
        }   

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            String credits1 = "Songs\nDrops of H2O ( The Filtered Water Treatment ) by J.Lang\nhttp://ccmixter.org/files/djlang59/37792" + "\n" +
                "is licensed under a Creative Commons license:\nhttp://creativecommons.org/licenses/by/3.0/";

            String credits2 = "cafe connection by morgantj\nhttp://ccmixter.org/files/morgantj/18947" + "\n" + 
                "is licensed under a Creative Commons license:\nhttp://creativecommons.org/licenses/by/3.0/";

            String credits3 = "Models" + "\n" + "http://www.turbosquid.com/FullPreview/Index.cfm/ID/293282" + "\n" +
                "http://www.turbosquid.com/FullPreview/Index.cfm/ID/691570" + "\n" + "http://www.turbosquid.com/3d-models/free-lwo-model-gold-coin/567715";

            String credits4 = "Textures\nhttp://www.mayang.com/textures/Architectural/html/Tiles/index.html" + "\n" + "http://www.dreamstime.com";

            Vector2 textPos1 = new Vector2(viewport.Width / 2 - 250, viewport.Height - 560);
            Vector2 textPos11 = new Vector2(viewport.Width / 2 - 249, viewport.Height - 559);
            Vector2 textPos2 = new Vector2(viewport.Width / 2 - 250, viewport.Height - 450);
            Vector2 textPos22 = new Vector2(viewport.Width / 2 - 249, viewport.Height - 449);
            Vector2 textPos3 = new Vector2(viewport.Width / 2 - 250, viewport.Height - 340);
            Vector2 textPos33 = new Vector2(viewport.Width / 2 - 249, viewport.Height - 339);
            Vector2 textPos4 = new Vector2(viewport.Width / 2 - 250, viewport.Height - 230);
            Vector2 textPos44 = new Vector2(viewport.Width / 2 - 249, viewport.Height - 229);

            spriteBatch.Begin();

            spriteBatch.DrawString(gameFont, credits1, textPos1, Color.Black);
            spriteBatch.DrawString(gameFont, credits1, textPos11, Color.White);
            spriteBatch.DrawString(gameFont, credits2, textPos2, Color.Black);
            spriteBatch.DrawString(gameFont, credits2, textPos22, Color.White);
            spriteBatch.DrawString(gameFont, credits3, textPos3, Color.Black);
            spriteBatch.DrawString(gameFont, credits3, textPos33, Color.White);
            spriteBatch.DrawString(gameFont, credits4, textPos4, Color.Black);
            spriteBatch.DrawString(gameFont, credits4, textPos44, Color.White);

            spriteBatch.End();
        }

        #endregion

        #region Handle Input

        #endregion
    }
}
