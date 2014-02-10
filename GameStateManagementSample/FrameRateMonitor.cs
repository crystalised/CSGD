using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace CoinHunt
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class FrameRateMonitor : Microsoft.Xna.Framework.GameComponent
    {
        #region Member variables fo performance benchmarking
        private float fps; //frames per second
        private float updateInterval = 1.0f;
        private float timeSinceLastUpdate = 0.0f;
        private float frameCounter = 0;
        public static String details = "";
        #endregion

        #region accessors
        public string Details
        {
            get
            {
                return details;
            }
        }

        public float FPS
        {
            get
            {
                return fps;
            }
        }
        #endregion


        public FrameRateMonitor(Game game)
            : this(game, false)
        {
            // TODO: Construct any child components here
        }

        public FrameRateMonitor(Game game, bool uncapFrameLimit)
            : base(game)
        {
            if (uncapFrameLimit)
            {
                GraphicsDeviceManager graphics = (GraphicsDeviceManager)Game.Services.GetService(typeof(IGraphicsDeviceManager));
                graphics.SynchronizeWithVerticalRetrace = false;
                Game.IsFixedTimeStep = false;
            }
            
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter++;
            timeSinceLastUpdate += elapsed;

            if (timeSinceLastUpdate > updateInterval)
            {
                fps = frameCounter / timeSinceLastUpdate;
                details = "FPS: " + fps.ToString() + ", GameTime: " + gameTime.TotalGameTime.TotalSeconds.ToString();
                frameCounter = 0;
                timeSinceLastUpdate -= updateInterval;

#if XBOX360
            System.Diagnostics.Debug.WriteLine(details);
#else
                Game.Window.Title = details;
#endif

                base.Update(gameTime);
            }
        }
    }
}