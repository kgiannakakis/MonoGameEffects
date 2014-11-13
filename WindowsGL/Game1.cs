using Hud;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Skyboxes;
using System;

namespace MonoGameEffects
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Matrix world = Matrix.CreateTranslation(0, 0, 0);
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);
        float angle = 0;
        float distance = 10;

        Model model;
        Model sphere;

        BasicEffect effect;
        Effect ambientEffect;
        Effect diffuseEffect;
        Effect specularEffect;
        Effect texturedEffect;
        Effect grayScaleEffect;
        Effect bumpMapEffect;
        Effect reflectionEffect;
        Effect transparencyEffect;

        Vector3 viewVector;

        Texture2D texture;
        Texture2D normalMap;

        Skybox skybox;
        Vector3 cameraLocation;

        enum EffectType
        {
            Default = 0,
            Ambient,
            Diffuse,
            Specular,
            Textured,
            GrayScale,
            BumpMap,
            SkyBox,
            Reflection,
            Transparency
        }

        private EffectType effectType = EffectType.Default;

        private HudManager hudManager;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            hudManager = new HudManager(Content, GraphicsDevice, spriteBatch);

            // TODO: use this.Content to load your game content here
            model = Content.Load<Model>("Models/Helicopter");
            sphere = Content.Load<Model>("Models/UntexturedSphere");

            // Basic Effect
            effect = new BasicEffect(GraphicsDevice);


            // Ambient
            ambientEffect = Content.Load<Effect>("Effects/Ambient");
            ambientEffect.Parameters["AmbientColor"].SetValue(Color.Yellow.ToVector4());
            ambientEffect.Parameters["AmbientIntensity"].SetValue(0.5f);

            // Diffuse
            diffuseEffect = Content.Load<Effect>("Effects/Diffuse");
            diffuseEffect.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());
            diffuseEffect.Parameters["AmbientIntensity"].SetValue(0.1f);
            diffuseEffect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(1,0,0));
            diffuseEffect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector4());
            diffuseEffect.Parameters["DiffuseIntensity"].SetValue(1.0f);

            // Specular
            specularEffect = Content.Load<Effect>("Effects/Specular");
            specularEffect.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());
            specularEffect.Parameters["AmbientIntensity"].SetValue(0.1f);
            specularEffect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(-1, 0, 0));
            specularEffect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector4());
            specularEffect.Parameters["DiffuseIntensity"].SetValue(1.0f);
            specularEffect.Parameters["Shininess"].SetValue(200.0f);
            specularEffect.Parameters["SpecularColor"].SetValue(Color.White.ToVector4());
            specularEffect.Parameters["SpecularIntensity"].SetValue(1.0f);

            // Textured
            texturedEffect = Content.Load<Effect>("Effects/Textured");
            texture = Content.Load<Texture2D>("HelicopterTexture_0");
            texturedEffect.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());
            texturedEffect.Parameters["AmbientIntensity"].SetValue(0.1f);
            texturedEffect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(-1, 0, 0));
            texturedEffect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector4());
            texturedEffect.Parameters["DiffuseIntensity"].SetValue(1.0f);
            texturedEffect.Parameters["Shininess"].SetValue(200.0f);
            texturedEffect.Parameters["SpecularColor"].SetValue(Color.White.ToVector4());
            texturedEffect.Parameters["SpecularIntensity"].SetValue(1.0f);

            // Gray scale
            grayScaleEffect = Content.Load<Effect>("Effects/GrayScale");
            grayScaleEffect.Parameters["BasicTexture"].SetValue(texture);

            // Bump map
            bumpMapEffect = Content.Load<Effect>("Effects/BumpMap");
            normalMap = Content.Load<Texture2D>("HelicopterNormalMap");
            bumpMapEffect.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());
            bumpMapEffect.Parameters["AmbientIntensity"].SetValue(0.1f);
            bumpMapEffect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(-1, 0, 0));
            bumpMapEffect.Parameters["Shininess"].SetValue(200.0f);
            bumpMapEffect.Parameters["SpecularColor"].SetValue(Color.White.ToVector4());
            bumpMapEffect.Parameters["SpecularIntensity"].SetValue(1.0f);

            // Skybox                       
            skybox = new Skybox("Skyboxes/Sunset", Content);

            // Reflection
            reflectionEffect = Content.Load<Effect>("Effects/Reflection");
            reflectionEffect.Parameters["TintColor"].SetValue(Color.White.ToVector4());

            // Transparency
            transparencyEffect = Content.Load<Effect>("Effects/Transparency");
            transparencyEffect.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());
            transparencyEffect.Parameters["AmbientIntensity"].SetValue(0.1f);
            transparencyEffect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(-1, 0, 0));
            transparencyEffect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector4());
            transparencyEffect.Parameters["DiffuseIntensity"].SetValue(1.0f);
            transparencyEffect.Parameters["Shininess"].SetValue(200.0f);
            transparencyEffect.Parameters["SpecularColor"].SetValue(Color.White.ToVector4());
            transparencyEffect.Parameters["SpecularIntensity"].SetValue(1.0f);
            transparencyEffect.Parameters["Transparency"].SetValue(0.5f);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            hudManager.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            hudManager.Update(gameTime);

            effectType = (EffectType)hudManager.SelectedEffectIndex;

            angle += 0.01f;

            switch (effectType)
            {
                case EffectType.Ambient:
                case EffectType.Diffuse:
                case EffectType.Default:
                    view = Matrix.CreateLookAt(distance * new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle)), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
                    break;
                case EffectType.Specular:
                case EffectType.Textured:
                case EffectType.GrayScale:
                case EffectType.BumpMap:
                case EffectType.SkyBox:
                case EffectType.Reflection:
                case EffectType.Transparency:
                    cameraLocation = distance * new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle));
                    Vector3 cameraTarget = new Vector3(0, 0, 0);
                    viewVector = Vector3.Transform(cameraTarget - cameraLocation, Matrix.CreateRotationY(0));
                    viewVector.Normalize();
                    view = Matrix.CreateLookAt(cameraLocation, cameraTarget, new Vector3(0, 1, 0));
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            switch (effectType)
            {
                case EffectType.Default:
                    DrawModel(model, world, view, projection);
                    break;
                case EffectType.Ambient:
                    DrawModelWithAmbientEffect(model, world, view, projection);
                    break;
                case EffectType.Diffuse:
                    DrawModelWithDiffuseEffect(model, world, view, projection);
                    break;
                case EffectType.Specular:
                    DrawModelWithSpecularEffect(model, world, view, projection);
                    break;
                case EffectType.Textured:
                    DrawModelWithTexturedEffect(model, world, view, projection);
                    break;
                case EffectType.GrayScale:
                    DrawModelWithGrayScaleEffect(model, world, view, projection);
                    break;
                case EffectType.BumpMap:
                    DrawModelWithBumpMapEffect(model, world, view, projection);
                    break;
                case EffectType.SkyBox:
                    RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState;
                    RasterizerState rasterizerState = new RasterizerState();
                    rasterizerState.CullMode = CullMode.None;
                    graphics.GraphicsDevice.RasterizerState = rasterizerState;

                    skybox.Draw(view, projection, cameraLocation);

                    graphics.GraphicsDevice.RasterizerState = originalRasterizerState;

                    DrawModelWithTexturedEffect(model, world, view, projection);
                    break;
                case EffectType.Reflection:
                    RasterizerState rOriginalRasterizerState = graphics.GraphicsDevice.RasterizerState;
                    RasterizerState rRasterizerState = new RasterizerState();
                    rRasterizerState.CullMode = CullMode.None;
                    graphics.GraphicsDevice.RasterizerState = rRasterizerState;

                    skybox.Draw(view, projection, cameraLocation);

                    graphics.GraphicsDevice.RasterizerState = rOriginalRasterizerState;

                    DrawModelWithReflectionEffect(sphere, world, view, projection);
                    break;
                case EffectType.Transparency:
                    RasterizerState tOriginalRasterizerState = graphics.GraphicsDevice.RasterizerState;
                    RasterizerState tRasterizerState = new RasterizerState();
                    tRasterizerState.CullMode = CullMode.None;
                    graphics.GraphicsDevice.RasterizerState = tRasterizerState;

                    skybox.Draw(view, projection, cameraLocation);

                    graphics.GraphicsDevice.RasterizerState = tOriginalRasterizerState;

                    graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                    DrawModelWithTransparencyEffect(model, world, view, projection);
                    graphics.GraphicsDevice.BlendState = BlendState.Opaque;
                    break;
            }

            hudManager.Draw(gameTime);

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.TextureEnabled = true;
                    effect.Texture = texture;
                    effect.World = world * mesh.ParentBone.Transform;
                    effect.View = view;
                    effect.Projection = projection;
                }

                /*foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = world * mesh.ParentBone.Transform;
                    effect.View = view;
                    effect.Projection = projection;
                }*/
                mesh.Draw();
            }
        }

        private void DrawModelWithAmbientEffect(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = ambientEffect;

                    ambientEffect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    ambientEffect.Parameters["View"].SetValue(view);
                    ambientEffect.Parameters["Projection"].SetValue(projection);
                }
                mesh.Draw();
            }
        }

        private void DrawModelWithDiffuseEffect(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = diffuseEffect;

                    diffuseEffect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    diffuseEffect.Parameters["View"].SetValue(view);
                    diffuseEffect.Parameters["Projection"].SetValue(projection);

                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    diffuseEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                }
                mesh.Draw();
            }
        }

        private void DrawModelWithSpecularEffect(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = specularEffect;
                    specularEffect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    specularEffect.Parameters["View"].SetValue(view);
                    specularEffect.Parameters["Projection"].SetValue(projection);
                    specularEffect.Parameters["ViewVector"].SetValue(viewVector);

                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    specularEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                }
                mesh.Draw();
            }
        }

        private void DrawModelWithTexturedEffect(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = texturedEffect;
                    texturedEffect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    texturedEffect.Parameters["View"].SetValue(view);
                    texturedEffect.Parameters["Projection"].SetValue(projection);
                    texturedEffect.Parameters["ViewVector"].SetValue(viewVector);
                    texturedEffect.Parameters["ModelTexture"].SetValue(texture);

                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    texturedEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                }
                mesh.Draw();
            }
        }

        private void DrawModelWithGrayScaleEffect(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = grayScaleEffect;
                    grayScaleEffect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    grayScaleEffect.Parameters["View"].SetValue(view);
                    grayScaleEffect.Parameters["Projection"].SetValue(projection);
                }
                mesh.Draw();
            }
        }

        private void DrawModelWithBumpMapEffect(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = bumpMapEffect;
                    bumpMapEffect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    bumpMapEffect.Parameters["View"].SetValue(view);
                    bumpMapEffect.Parameters["Projection"].SetValue(projection);
                    bumpMapEffect.Parameters["ViewVector"].SetValue(viewVector);

                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    bumpMapEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                }
                mesh.Draw();
            }
        }

        private void DrawModelWithReflectionEffect(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = reflectionEffect;
                    reflectionEffect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    reflectionEffect.Parameters["View"].SetValue(view);
                    reflectionEffect.Parameters["Projection"].SetValue(projection);

                    reflectionEffect.Parameters["SkyboxTexture"].SetValue(skybox.SkyBoxTextureCube);
                    reflectionEffect.Parameters["CameraPosition"].SetValue(cameraLocation);
                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    reflectionEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                }
                mesh.Draw();
            }
        }

        private void DrawModelWithTransparencyEffect(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = transparencyEffect;
                    transparencyEffect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    transparencyEffect.Parameters["View"].SetValue(view);
                    transparencyEffect.Parameters["Projection"].SetValue(projection);
                    transparencyEffect.Parameters["ViewVector"].SetValue(viewVector);
                    transparencyEffect.Parameters["ModelTexture"].SetValue(texture);

                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    transparencyEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                }
                mesh.Draw();
            }
        }

    }
}
