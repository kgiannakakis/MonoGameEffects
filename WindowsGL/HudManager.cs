using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

namespace Hud
{
    public class HudManager
    {
        private SpriteBatch spriteBatch;
        private GraphicsDevice graphicsDevice;
        private ContentManager content;

        private BlendState _preSpriteBlendState;
        private DepthStencilState _preSpriteDepthStencilState;
        private RasterizerState _preSpriteRasterizerState;
        private SamplerState _preSpriteSamplerState;

        private Texture2D comboBoxTexture;
        private Texture2D mousePointerTexture;
        private SpriteFont font;

        private bool isComboBoxOpen = false;
        private int comboBoxPosX = 20;
        private int comboBoxPosY = 20;
        private int comboBoxWidth = 240;
        private int comboBoxHeight = 40;

        int comboBoxOptionsCount;

        private Rectangle comboBoxRectangle;
        private Rectangle comboBoxOpenRectangle;        

        private List<String> options = new List<String> (){
            "Default",
            "Ambient",
            "Diffuse",
            "Specular",
            "Textured",
            "GrayScale",
            "BumpMap",
            "SkyBox",
            "Reflection",
            "Transparency"
        };
        private int selectedIndex = 0;

        private double eventTime = 0;

        public int SelectedEffectIndex
        {
            get
            {
                return selectedIndex;
            }
        }

        public HudManager(ContentManager content, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            this.content = content;
            this.spriteBatch = spriteBatch;
            this.graphicsDevice = graphicsDevice;

            Init();
        }

        private void Init()
        {
            comboBoxOptionsCount = options.Count();
            
            font = content.Load<SpriteFont>("Fonts/Poetsen");
            comboBoxTexture = content.Load<Texture2D>("Hud/ComboBox");
            mousePointerTexture = content.Load<Texture2D>("Hud/mouse-pointer");
            comboBoxRectangle = new Rectangle(comboBoxPosX, comboBoxPosY, comboBoxWidth, comboBoxHeight);
            comboBoxOpenRectangle = new Rectangle(comboBoxPosX, comboBoxPosY, comboBoxWidth, comboBoxHeight * (comboBoxOptionsCount + 1));
        }

        public void UnloadContent()
        {

        }

        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds < eventTime + 300)
            {
                return;
            }
            
            Vector2? touchPos = null;
            
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                
                if (gesture.GestureType == GestureType.Tap)
                {
                    touchPos = gesture.Position;
                }
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                touchPos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            }

            if (touchPos.HasValue)
            {
                if (!isComboBoxOpen && comboBoxRectangle.Contains(touchPos.Value))
                {
                    isComboBoxOpen = true;
                    eventTime = gameTime.TotalGameTime.TotalMilliseconds;
                }
                else if (isComboBoxOpen && !comboBoxOpenRectangle.Contains(touchPos.Value))
                {
                    isComboBoxOpen = false;
                    eventTime = gameTime.TotalGameTime.TotalMilliseconds;
                }
                else if (isComboBoxOpen && comboBoxOpenRectangle.Contains(touchPos.Value))
                {
                    selectedIndex = (int)((touchPos.Value.Y - comboBoxHeight - comboBoxPosY) / comboBoxHeight);
                    if (selectedIndex < 0) selectedIndex = 0;
                    if (selectedIndex >= options.Count()) selectedIndex = options.Count() - 1;
                    isComboBoxOpen = false;
                    eventTime = gameTime.TotalGameTime.TotalMilliseconds;
                }
            }

        }

        public void Draw(GameTime gameTime)
        {
            StoreStateBeforeSprites();
            spriteBatch.Begin();

            int offsetX = 10;
            int offsetY = 10;

            spriteBatch.Draw(comboBoxTexture,
                             new Rectangle(comboBoxPosX, comboBoxPosY, comboBoxWidth, comboBoxHeight),
                             new Rectangle(0, 0, comboBoxWidth, comboBoxHeight),
                             Color.White);
            spriteBatch.DrawString(font, options[selectedIndex],
                                    new Vector2(comboBoxPosX + offsetX, comboBoxPosY + offsetY), Color.Black);
            spriteBatch.Draw(comboBoxTexture,
                             new Rectangle(comboBoxPosX + 203, comboBoxPosY + 3, 31, 34),
                             new Rectangle(comboBoxWidth, 3, 31, 34),
                             Color.White);

            if (isComboBoxOpen)
            {
                spriteBatch.Draw(comboBoxTexture,
                                 new Rectangle(comboBoxPosX, comboBoxPosY + comboBoxHeight, comboBoxWidth, 2),
                                 new Rectangle(0, 0, comboBoxWidth, 2),
                                 Color.White);
                spriteBatch.Draw(comboBoxTexture,
                                 new Rectangle(comboBoxPosX, comboBoxPosY + comboBoxHeight + 2, comboBoxWidth, comboBoxHeight * comboBoxOptionsCount - 4),
                                 new Rectangle(0, 2, comboBoxWidth, 36),
                                 Color.White);
                for (int i = 0; i < comboBoxOptionsCount; i++)
                {
                    spriteBatch.DrawString(font, options[i],
                                            new Vector2(comboBoxPosX + offsetX, comboBoxPosY + offsetY + comboBoxHeight*(i+1)), 
                                            i == selectedIndex ? Color.Blue : Color.Black);
                }
                spriteBatch.Draw(comboBoxTexture,
                                 new Rectangle(comboBoxPosX, comboBoxPosY + comboBoxHeight * (comboBoxOptionsCount + 1) - 2, comboBoxWidth, 2),
                                 new Rectangle(0, 0, comboBoxWidth, 2),
                                 Color.White);
            }

            spriteBatch.Draw(mousePointerTexture, Mouse.GetState().Position.ToVector2(), Color.White);
              
            spriteBatch.End();
            RestoreStateAfterSprites();
        }

        private void StoreStateBeforeSprites()
        {
            _preSpriteBlendState = graphicsDevice.BlendState;
            _preSpriteDepthStencilState = graphicsDevice.DepthStencilState;
            _preSpriteRasterizerState = graphicsDevice.RasterizerState;
            _preSpriteSamplerState = graphicsDevice.SamplerStates[0];
        }

        /// <summary>
        /// Restore all of the graphics device state values that are modified by
        /// the sprite batch to their previous values, as saved by an earlier call to
        /// StoreStateBeforeSprites function.
        /// </summary>
        private void RestoreStateAfterSprites()
        {
            graphicsDevice.BlendState = _preSpriteBlendState;
            graphicsDevice.DepthStencilState = _preSpriteDepthStencilState;
            graphicsDevice.RasterizerState = _preSpriteRasterizerState;
            graphicsDevice.SamplerStates[0] = _preSpriteSamplerState;
        }
        
    }
}
