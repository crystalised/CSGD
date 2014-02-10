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
using Microsoft.Xna.Framework.Input;
#endregion

namespace CoinHunt
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry splitScreenMenuEntry;
        MenuEntry vibrateMenuEntry;
        MenuEntry musicMenuEntry;
        MenuEntry inMusicMenuEntry;
        MenuEntry deMusicMenuEntry;
        MenuEntry soundMenuEntry;
        MenuEntry inSoundMenuEntry;
        MenuEntry deSoundMenuEntry;

        public enum splitScreenType
        {
            Vertical,
            Horizontal,
        }

        public static splitScreenType currentSST = splitScreenType.Horizontal;

        public static bool vibrate = true;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            splitScreenMenuEntry = new MenuEntry(string.Empty);
            vibrateMenuEntry = new MenuEntry(string.Empty);
            musicMenuEntry = new MenuEntry(string.Empty);
            inMusicMenuEntry = new MenuEntry(string.Empty);
            deMusicMenuEntry = new MenuEntry(string.Empty);
            soundMenuEntry = new MenuEntry(string.Empty);
            inSoundMenuEntry = new MenuEntry(string.Empty);
            deSoundMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            splitScreenMenuEntry.Selected += splitScreenMenuEntrySelected;
            vibrateMenuEntry.Selected += vibrateScreenMenuEntrySelected;
            musicMenuEntry.Selected += musicMenuEntrySelected;
            inMusicMenuEntry.Selected += inMusicMenuEntrySelected;
            deMusicMenuEntry.Selected += deMusicMenuEntrySelected;
            soundMenuEntry.Selected += soundMenuEntrySelected;
            inSoundMenuEntry.Selected += inSoundMenuEntrySelected;
            deSoundMenuEntry.Selected += deSoundMenuEntrySelected;
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(splitScreenMenuEntry);
            MenuEntries.Add(vibrateMenuEntry);
            MenuEntries.Add(musicMenuEntry);
            MenuEntries.Add(inMusicMenuEntry);
            MenuEntries.Add(deMusicMenuEntry);
            MenuEntries.Add(soundMenuEntry);
            MenuEntries.Add(inSoundMenuEntry);
            MenuEntries.Add(deSoundMenuEntry);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            splitScreenMenuEntry.Text = "Split Screen Orientation: " + currentSST;
            vibrateMenuEntry.Text = "Vibration: " + (vibrate ? "On" : "Off");
            musicMenuEntry.Text = "Music volume: " + (int)(Settings.musicVolume * 100 + 0.5);
            inMusicMenuEntry.Text = "Increase music volume";
            deMusicMenuEntry.Text = "Decrease music volume";
            soundMenuEntry.Text = "Sound volume: " + (int)(Settings.soundVolume * 100 + 0.5);
            inSoundMenuEntry.Text = "Increase sound volume";
            deSoundMenuEntry.Text = "Decrease sound volume";
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        /// 
        void splitScreenMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.soundCue = ScreenManager.soundBank.GetCue("clickCue2");
            ScreenManager.soundCue.Play();

            currentSST++;

            if (currentSST > splitScreenType.Horizontal)
                currentSST = 0;

            SetMenuEntryText();
        }

        void vibrateScreenMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.soundCue = ScreenManager.soundBank.GetCue("clickCue2");
            ScreenManager.soundCue.Play();

            vibrate = !vibrate;
            SetMenuEntryText();
        }

        void musicMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.soundCue = ScreenManager.soundBank.GetCue("clickCue2");
            ScreenManager.soundCue.Play();

            SetMenuEntryText();
        }

        void inMusicMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.soundCue = ScreenManager.soundBank.GetCue("clickCue2");
            ScreenManager.soundCue.Play();

            Settings.musicVolume += 0.05f;
            if (Settings.musicVolume > 1.0f)
            {
                Settings.musicVolume = 1.0f;
            }
            ScreenManager.audioEngine.GetCategory("backgroundMusic").SetVolume(Settings.musicVolume);

            SetMenuEntryText();
        }

        void deMusicMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.soundCue = ScreenManager.soundBank.GetCue("clickCue2");
            ScreenManager.soundCue.Play();

            Settings.musicVolume -= 0.05f;
            if (Settings.musicVolume < 0)
            {
                Settings.musicVolume = 0;
            }
            ScreenManager.audioEngine.GetCategory("backgroundMusic").SetVolume(Settings.musicVolume);

            SetMenuEntryText();
        }

        void soundMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.soundCue = ScreenManager.soundBank.GetCue("clickCue2");
            ScreenManager.soundCue.Play();

            SetMenuEntryText();
        }

        void inSoundMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.soundCue = ScreenManager.soundBank.GetCue("clickCue2");
            ScreenManager.soundCue.Play();

            Settings.soundVolume += 0.05f;
            if (Settings.soundVolume > 1.0f)
            {
                Settings.soundVolume = 1.0f;
            }
            ScreenManager.audioEngine.GetCategory("soundEffects").SetVolume(Settings.soundVolume);

            SetMenuEntryText();
        }

        void deSoundMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.soundCue = ScreenManager.soundBank.GetCue("clickCue2");
            ScreenManager.soundCue.Play();

            Settings.soundVolume -= 0.05f;
            if (Settings.soundVolume < 0)
            {
                Settings.soundVolume = 0;
            }
            ScreenManager.audioEngine.GetCategory("soundEffects").SetVolume(Settings.soundVolume);

            SetMenuEntryText();
        }

        #endregion
    }
}
