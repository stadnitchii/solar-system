using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OpenGL
{
    public class Framebuffer : IDisposable
    {
        /// <summary>
        /// OpenGL ID
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// ID to texture attached to the frambuffer, it acts as a color buffer, and is used for sampling (this is public)
        /// </summary>
        public List<Texture> Textures { get; private set; }

        /// <summary>
        /// ID to the renderbuffer attached to the framebuffer, it acts as a depth and stencill buffer, not used for sampling, 
        /// but still needed when doing depth and stencil testing
        /// </summary>
        private int RenderBufferID;

        /// <summary>
        /// Size of the buffer, we cannot change this, it would be easier to create a new on instead of changing size
        /// </summary>
        public Size Size { get; private set; }

        /// <summary>
        /// Create the frambuffer and attach the color and stencil/depth buffers
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Framebuffer(int width, int height)
        {
            Textures = new List<Texture>();
            RenderBufferID = -1;

            this.Size = new Size(width, height);

            //gen frambuffer
            ID = GL.GenFramebuffer();
        }

        public void AttachColorBuffer(
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba, 
            PixelFormat format = PixelFormat.Rgba, 
            PixelType type = PixelType.UnsignedByte,
            TextureMinFilter minFilter = TextureMinFilter.Linear,
            TextureMagFilter magFilter = TextureMagFilter.Linear,
            TextureWrapMode wrapMode = TextureWrapMode.ClampToBorder)
        {
            //gen colorbuffer as texture
            Texture texture = new Texture2d();
            Textures.Add(texture);

            int txtId = texture.Id;

            GL.BindTexture(TextureTarget.Texture2D, txtId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, Size.Width, Size.Height, 0, format, type, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + Textures.Count - 1, TextureTarget.Texture2D, txtId, 0);

            if (Textures.Count > 1)
            {
                DrawBuffersEnum[] e = new DrawBuffersEnum[Textures.Count];
                for (int i = 0; i < Textures.Count; i++)
                    e[i] = DrawBuffersEnum.ColorAttachment0 + i;

                GL.DrawBuffers(Textures.Count, e);
            }

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
                throw new Exception(status.ToString());

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        //public void AttachColorBufferMS(PixelInternalFormat internalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Rgba, PixelType type = PixelType.UnsignedByte,
        //                                TextureMinFilter filter = TextureMinFilter.Linear, TextureWrapMode wrapMode = TextureWrapMode.ClampToBorder)
        //{
        //    //gen colorbuffer as texture
        //    TextureID[ColorAttachments] = GL.GenTexture();
        //    ColorAttachments++;
        //    GL.BindTexture(TextureTarget.Texture2DMultisample, TextureID[ColorAttachments - 1]);
        //    GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, 8, internalFormat, Size.Width, Size.Height, true);
        //    //GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureMinFilter, (float)filter);
        //    //GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureMagFilter, (float)filter);
        //    //GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureWrapS, (float)wrapMode);
        //    //GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureWrapT, (float)wrapMode);

        //    GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);
        //    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + ColorAttachments - 1, TextureTarget.Texture2DMultisample, TextureID[ColorAttachments - 1], 0);

        //    GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, ID);
        //    GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

        //    //GL.BlitFramebuffer(0, 0, Size.Width, Size.Height, 0, 0, Size.Width, Size.Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);

        //    if (ColorAttachments > 1)
        //    {
        //        DrawBuffersEnum[] e = new DrawBuffersEnum[ColorAttachments];
        //        for (int i = 0; i < ColorAttachments; i++)
        //            e[i] = DrawBuffersEnum.ColorAttachment0 + i;

        //        GL.DrawBuffers(ColorAttachments, e);
        //    }

        //    var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        //    if (status != FramebufferErrorCode.FramebufferComplete)
        //        throw new Exception(status.ToString());

        //    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        //    GL.BindTexture(TextureTarget.Texture2D, 0);
        //}

        public void AttachDepthStencilBuffer()
        {
            //gen stencil/depth buffer as renderbuffer
            RenderBufferID = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RenderBufferID);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, Size.Width, Size.Height);

            //attach color buffer and stencil/depth buffers
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, RenderBufferID);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
                throw new Exception(status.ToString());
        }

        /// <summary>
        /// bind the frambuffer and set the viewport to its size
        /// </summary>
        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);
            GL.Viewport(0,0, Size.Width, Size.Height);
        }

        /// <summary>
        /// delete the buffers from memory
        /// </summary>
        public void Dispose()
        {
            foreach (var txt in Textures)
                txt.Dispose();

            if (RenderBufferID != -1)
                GL.DeleteRenderbuffer(RenderBufferID);

            GL.DeleteFramebuffer(ID);
        }
    }
}
