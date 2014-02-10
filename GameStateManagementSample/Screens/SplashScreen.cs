#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace CoinHunt
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class SplashScreen : MenuScreen
    {
        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public SplashScreen()
            : base("")
        {
            MenuEntry MainMenuEntry = new MenuEntry("Press A To Start");

            MainMenuEntry.Selected += MainMenuEntrySelected;

            MenuEntries.Add(MainMenuEntry);
        }


        #endregion

        #region Handle Input

        void MainMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            PlayerIndex controllingPlayer = PlayerIndex.One;
#if XBOX360
            for (PlayerIndex index = PlayerIndex.One; index <= PlayerIndex.Four; index++)
            {
                if (GamePad.GetState(index).Buttons.Start == ButtonState.Pressed)
                {
                    controllingPlayer = index;
                    break;
                }
            }
#else
            for (PlayerIndex index = PlayerIndex.One; index <= PlayerIndex.Four; index++)
            {
                if (Keyboard.GetState(index).IsKeyDown(Keys.Enter))
                {
                    controllingPlayer = index;
                    break;
                }
            }
#endif
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
        }

        #endregion
    }
}
