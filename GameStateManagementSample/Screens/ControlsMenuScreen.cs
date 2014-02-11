#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace CoinHunt
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class ControlsMenuScreen : MenuScreen
    {
        #region Fields
        Texture2D ControllerPic;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public ControlsMenuScreen()
            : base("Controls")
        {
            MenuEntry back = new MenuEntry("Back");

            back.Selected += OnCancel;

            MenuEntries.Add(back);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            ControllerPic = content.Load<Texture2D>("Textures/controller");
        }   

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Vector2 ControllerPos = new Vector2(400, 143);
            spriteBatch.Begin();
            Color color = Color.White * TransitionAlpha;
            // Draw the background rectangle.
            spriteBatch.Draw(ControllerPic, ControllerPos, color);
        
       

            spriteBatch.End();
        }
    }
}






        








