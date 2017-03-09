using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;


namespace OpenGL
{
    public class TextureCube : Texture
    {
        public TextureCube(string directory, string ext = ".png")
        {
            load_internal(directory, ext);
        }

        private void load_internal(string path, string ext)
        {
            GL.BindTexture(TextureTarget.TextureCubeMap, Id);

            //
            string[] paths = new string[] { path + "/px" + ext, path + "/nx" + ext, path + "/py" + ext,
            path + "/ny" + ext, path + "/pz" + ext, path + "/nz" + ext};

            for (int i = 0; i < 6; i++)
            {
                using (Bitmap img = new Bitmap(paths[i]))
                {
                    //img.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    BitmapData bits = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, img.Width, img.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bits.Scan0);
                    img.UnlockBits(bits);
                }
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            GL.BindTexture(TextureTarget.TextureCubeMap, 0);
        }

        public override void Bind(int activeTexture = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + activeTexture);
            GL.BindTexture(TextureTarget.TextureCubeMap, Id);
        }

        public override void UnBind(int activeTexture = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + activeTexture);
            GL.BindTexture(TextureTarget.TextureCubeMap, 0);
        }
    }
}
