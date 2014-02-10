using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoinHunt
{
    class Coin
    {
        private GraphicsDevice graphicsDevice;
        public Vector3 Location;
        float scale = 7.0f;
        public bool isAlive = true;
        public Matrix World
        {
            get
            { 
                return world; 
            }
            
        }

        public BoundingSphere getBoundingSphere(Model coinModel)
        {
            BoundingSphere sphere = new BoundingSphere();
            sphere = new BoundingSphere();

            foreach (ModelMesh mesh in coinModel.Meshes)
            {
                if (sphere.Radius == 0)
                    sphere = mesh.BoundingSphere;
                else
                    sphere = BoundingSphere.CreateMerged(sphere, mesh.BoundingSphere);
            }

            sphere.Center = Location;
            sphere.Radius *= 1.0f;
            return sphere;

        }
       
        private Matrix world;

        public Coin(GraphicsDevice device)
        {
            graphicsDevice = device;
            
        }

        public void Update(GameTime gameTime)
        {
            world = Matrix.CreateScale(scale) * Matrix.CreateTranslation(Location);
        }

    }
}
