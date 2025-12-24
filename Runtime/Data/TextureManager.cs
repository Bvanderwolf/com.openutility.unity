using System.Collections.Generic;
using UnityEngine;

namespace OpenUtility.Data
{
    /// <summary>
    /// Helps manage textures and sprites, including caching conversions between them.
    /// </summary>
    public static class TextureManager
    {
        private static readonly Dictionary<Texture, Sprite> _textureToSpriteCache = new Dictionary<Texture, Sprite>();
        private static readonly Dictionary<Sprite, Texture2D> _spriteToTextureCache = new Dictionary<Sprite, Texture2D>();
        private static readonly Dictionary<string, Texture2D> _namedTextureCache = new Dictionary<string, Texture2D>();

        /// <summary>
        /// Clears all cached textures and sprites.
        /// </summary>
        public static void ClearCache()
        {
            _textureToSpriteCache.Clear();
            _spriteToTextureCache.Clear();
        }
        
        /// <summary>
        /// Clears the render texture with a clear color.
        /// </summary>
        public static void ClearRenderTexture(RenderTexture texture)
        {
            RenderTexture activeTexture = RenderTexture.active;
            RenderTexture.active = texture;
            
            GL.Clear(true, true, Color.clear);
            
            RenderTexture.active = activeTexture;
        }
        
        /// <summary>
        /// Returns a sprite from a texture using the texture width/height and the default pivot.
        /// Caches the created sprite for future requests of the same texture.
        /// </summary>
        public static Sprite TextureToSprite(Texture2D texture)
        {
            if (_textureToSpriteCache.TryGetValue(texture, out Sprite sprite))
                return (sprite);

            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            _textureToSpriteCache.Add(texture, sprite);
            return (sprite);
        }

        /// <summary>
        /// returns the texture from a sprite. Accounts for sprites that are not the same size as the texture
        /// (sprites that are part of an atlas for example). Caches created textures for future requests of the same sprite.
        /// </summary>
        public static Texture2D SpriteToTexture(Sprite sprite)
        {
            if (_spriteToTextureCache.TryGetValue(sprite, out Texture2D texture))
                return (texture);
            
            if (sprite.rect.width != sprite.texture.width || sprite.rect.height != sprite.texture.height)
            {
                texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                Color[] colors = sprite.texture.GetPixels(
                    (int)sprite.rect.x, 
                    (int)sprite.rect.y, 
                    (int)sprite.rect.width, 
                    (int)sprite.rect.height);
                
                texture.SetPixels(colors);
                texture.Apply();
                
                _spriteToTextureCache.Add(sprite, texture);
                
                return texture;
            }

            return sprite.texture;
        }
        
        /// <summary>
        /// Returns a Texture2D from a byte array. Caches the texture if a name is provided to allow reuse
        /// if the same name is requested again.
        /// </summary>
        public static Texture2D BytesToTexture(byte[] bytes, string name = null)
        {
            if (bytes == null || bytes.Length == 0)
                return (null);
            
            if (!string.IsNullOrEmpty(name) && _namedTextureCache.TryGetValue(name, out Texture2D cachedTexture))
                return (cachedTexture);
            
            Texture2D texture = new Texture2D(2, 2);
            if (!texture.LoadImage(bytes))
            {
                Debug.LogWarning("Failed to load texture from bytes");
                return (null);
            }

            if (string.IsNullOrEmpty(name)) 
                return (texture);
            
            texture.name = name;
            _namedTextureCache[name] = texture;

            return (texture);
        }
        
        /// <summary>
        /// Sets the maximum dimension (width or height) of the texture, resizing it if necessary.
        /// </summary>
        public static void SetMaxDimension(this Texture2D texture, int maxDimension)
        {
            if (texture == null) return;

            int width = texture.width;
            int height = texture.height;

            if (width <= maxDimension && height <= maxDimension)
                return; // No resizing needed

            float scale = maxDimension / (float)Mathf.Max(width, height);
            int newWidth = Mathf.RoundToInt(width * scale);
            int newHeight = Mathf.RoundToInt(height * scale);

            // Resize using RenderTexture
            RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
            RenderTexture.active = rt;
            Graphics.Blit(texture, rt);

            texture.Reinitialize(newWidth, newHeight);
            texture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
        }
    }
}
