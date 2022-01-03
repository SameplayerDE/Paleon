using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace Paleon
{
    public static class ResourceManager
    {

        private static Dictionary<string, MyTexture> sprites;
        //private static Dictionary<string, SoundEffect> soundEffects;
        private static Dictionary<string, Effect> effects;

        private static bool wasInitialized = false;

        public static void Initialize(ContentManager content)
        {
            sprites = LoadSprites(content, "Sprites", "Buildings");
            //soundEffects = LoadContent<SoundEffect>(content, "Audio");
            effects = LoadContent<Effect>(content, "Shaders");

            wasInitialized = true;
        }

        public static MyTexture GetTexture(string name)
        {
            if (!wasInitialized)
                throw new Exception("Resource Manager was not initialized!");

            try
            {
                return sprites[name];
            }
            catch(Exception e)
            {
                throw new Exception("Sprite with '" + name + "' name not found!: " + e.ToString());
            }
        }

        //public static SoundEffect GetSound(string name)
        //{
        //    if (!wasInitialized)
        //        throw new Exception("Resource Manager was not initialized!");

        //    try
        //    {
        //        return soundEffects[name];
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("Sound with '" + name + "' name not found!: " + e.ToString());
        //    }
        //}

        public static Effect GetEffect(string name)
        {
            if (!wasInitialized)
                throw new Exception("Resource Manager was not initialized!");

            try
            {
                return effects[name];
            }
            catch (Exception e)
            {
                throw new Exception("Effect with '" + name + "' name not found!: " + e.ToString());
            }
        }

        private static Dictionary<string, T> LoadContent<T>(ContentManager contentManager, string contentFolder)
        {
            DirectoryInfo dir = new DirectoryInfo(contentManager.RootDirectory + "/" + contentFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            Dictionary<string, T> result = new Dictionary<string, T>();
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                result[key] = contentManager.Load<T>(contentFolder + "/" + key);
            }
            return result;
        }

        private static Dictionary<string, MyTexture> LoadSprites(ContentManager contentManager, params string[] contentFolders)
        {
            Dictionary<string, MyTexture> result = new Dictionary<string, MyTexture>();

            for (int i = 0; i < contentFolders.Length; i++)
            {
                string contentFolder = contentFolders[i];

                DirectoryInfo dir = new DirectoryInfo(contentManager.RootDirectory + "/" + contentFolder);
                if (!dir.Exists)
                    throw new DirectoryNotFoundException();

                FileInfo[] files = dir.GetFiles("*.*");
                foreach (FileInfo file in files)
                {
                    if (file.Extension == ".xnb")
                    {
                        string key = Path.GetFileNameWithoutExtension(file.Name);
                        result[key] = new MyTexture(contentManager.Load<Texture2D>(contentFolder + "/" + key));
                    }
                }
            }

            
            return result;
        }

    }
}
