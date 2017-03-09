using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace OpenGL
{
    public class Model
    {
        public VAO Vao { get; protected set; }
        public Texture Texture { get; protected set; }

        private Matrix4 transform;
        public Matrix4 Transform
        {
            get { return transform; }
            protected set { transform = value; }
        }

        public Vector3 Translation
        {
            get { return Transform.ExtractTranslation(); }
            set { SetTranslation(value); }
        }

        public Vector3 Scale
        {
            get { return Transform.ExtractScale(); }
            set { setScale(value); }
        }

        public Quaternion Rotation
        {
            get { return Transform.ExtractRotation(); }
        }

        public Vector3 pickColor { get; protected set; }

        public Model(VAO _vao, Texture t)
        {
            this.Vao = _vao;
            this.Texture = t;
            this.Transform = Matrix4.Identity;
            this.pickColor = new Vector3(1, 0, 0);
        }

        public virtual void Draw(Camera cam, ShaderProgram shader)
        {
            Texture?.Bind();
            shader.Bind();
            shader.SetUniform("model", Transform);

            Vao.DrawElements(PrimitiveType.Quads);
        }

        public virtual void DrawToPickBuffer(Camera cam, ShaderProgram shader)
        {
            shader.Bind();
            shader.SetUniform("model", Transform);
            shader.SetUniform("view", cam.Transfrom);
            shader.SetUniform("pickColor", pickColor);

            Vao.DrawElements(PrimitiveType.Quads);
        }

        public virtual void Update(double delta)
        {
            
        }

        #region Translate
        public void ClearTranslation(Vector3 position)
        {
            transform.ClearTranslation();
        }

        public void SetTranslation(Vector3 position)
        {
            transform.Row3 = new Vector4(position, 1);
        }

        public void TranslateBy(Vector3 ammount)
        {
            transform.Row3 += new Vector4(ammount, 0);
        }

        #endregion

        #region scale
        public void clearScale()
        {
            transform = transform.ClearScale();
        }

        public void setScale(Vector3 scale)
        {
            transform = transform.ClearScale();
            scaleBy(scale);
        }

        public void scaleBy(Vector3 scale)
        {
            transform.Row0 *= new Vector4(new Vector3(scale.X), 1);
            transform.Row1 *= new Vector4(new Vector3(scale.Y), 1);
            transform.Row2 *= new Vector4(new Vector3(scale.Z), 1);
        }

        public void setScale(float scale)
        {
            setScale(new Vector3(scale, scale, scale));
        }

        public void scaleBy(float scale)
        {
            scaleBy(new Vector3(scale, scale, scale));
        }
        #endregion

        #region Rotation
        public void RotateXBy(float f)
        {
            transform *= Matrix4.CreateRotationX(f);
        }

        public void RotateYBy(float f)
        {
            transform *= Matrix4.CreateRotationY(f);
        }

        public void RotateZBy(float f)
        {
            transform *= Matrix4.CreateRotationZ(f);
        }

        public void RotateBy(Vector3 vec, float angle)
        {
            transform *= Matrix4.CreateFromAxisAngle(vec, angle);
        }

        public void ClearRoatation()
        {
            transform = transform.ClearRotation();
        }

        #endregion
    }
}
