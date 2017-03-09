using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Runtime.InteropServices;

namespace OpenGL
{
    public class VAO 
    {
        public int Id { get; private set; }
        public int ElementsId { get; private set; }
        public int Length { get; private set; }

        private List<int> Buffers { get; set; }

        public VAO(Vector3[] positions, Vector2[] uvs, Vector3[] normals)
        {
            Buffers = new List<int>();
            this.Length = positions.Length;
            this.Id = GL.GenVertexArray();

            this.addAttributeArray(positions, 3, 0 );
            this.addAttributeArray(uvs, 2, 1);
            this.addAttributeArray(normals, 3, 2);
        }

        public VAO(Vector3[] positions)
        {
            Buffers = new List<int>();
            this.Length = positions.Length;
            this.Id = GL.GenVertexArray();

            this.addAttributeArray(positions, 3, 0);
        }

        public VAO(Vector2[] positions)
        {
            Buffers = new List<int>();
            this.Length = positions.Length;
            this.Id = GL.GenVertexArray();

            this.addAttributeArray(positions, 2, 0);
        }

        public VAO(float[] positions)
        {
            Buffers = new List<int>();
            this.Length = positions.Length;
            this.Id = GL.GenVertexArray();

            this.addAttributeArray(positions, 1, 0);
        }

        public int addAttributeArray<E>(E[] bufferData, int stride, int location) where E : struct
        {
            GL.BindVertexArray(Id);

            int typeSize = Marshal.SizeOf(bufferData.GetType().GetElementType());

            //generate buffer add it and enable atribute array
            int bufferID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferID);         
            GL.BufferData(BufferTarget.ArrayBuffer, bufferData.Length * typeSize, bufferData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(location, stride, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(location);
            Buffers.Add(bufferID);
            
            //unbind our buffers
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return Buffers.Count;
        }

        public void addElementArray(uint[] elements)
        {
            GL.BindVertexArray(Id);

            this.Length = elements.Length;

            ElementsId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementsId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, elements.Length * sizeof(uint), elements, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void Bind()
        {
            GL.BindVertexArray(Id);
        }

        public void DrawArrays(PrimitiveType type = PrimitiveType.Triangles)
        {
            Bind();
            GL.DrawArrays(type, 0, Length);
            Unbind();
        }

        public void DrawElements(PrimitiveType type = PrimitiveType.Triangles)
        {
            Bind();
            GL.DrawElements(type, Length, DrawElementsType.UnsignedInt, 0);
            Unbind();
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            foreach (var bufferID in Buffers)
                GL.DeleteBuffer(bufferID);

            GL.DeleteVertexArray(Id);
        }
    }
}
