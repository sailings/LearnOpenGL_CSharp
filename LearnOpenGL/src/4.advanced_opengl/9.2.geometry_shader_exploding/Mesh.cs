using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using GlmNet;
using SharpGL;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;

namespace _9._2.geometry_shader_exploding
{
    /// <summary>
    /// 顶点信息
    /// </summary>
    public struct Vertex
    {
        /// <summary>
        /// 位置
        /// </summary>
        public vec3 Position { get; set; }

        /// <summary>
        /// 法线
        /// </summary>
        public vec3 Normal { get; set; }
        
        /// <summary>
        /// 纹理坐标
        /// </summary>
        public vec2 TexCoords { get; set; }

        /// <summary>
        /// 切线
        /// </summary>
        public vec3 Tangent { get; set; }

        /// <summary>
        /// 二重切线
        /// </summary>
        public vec3 Bitangent { get; set; }

        /// <summary>
        /// 转换成数组
        /// </summary>
        /// <returns></returns>
        public float[] ToArray()
        {
            var pArray = Position.to_array().ToList();
            var nArray = Normal.to_array().ToList();
            var teArray = TexCoords.to_array().ToList();
            var taArray = Tangent.to_array().ToList();
            var bArray = Bitangent.to_array().ToList();
            List<float> result = new List<float>();
            result.AddRange(pArray);
            result.AddRange(nArray);
            result.AddRange(teArray);
            result.AddRange(taArray);
            result.AddRange(bArray);
            return result.ToArray();
        }
    }

    /// <summary>
    /// 纹理
    /// </summary>
    public struct Texture
    {
        /// <summary>
        /// ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 纹理类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 纹理路径
        /// </summary>
        public string path { get; set; }
    }

    /// <summary>
    /// 网格
    /// </summary>
    public class Mesh
    {
        /// <summary>
        /// 顶点
        /// </summary>
        public Vertex[] vertices { get; set; }

        /// <summary>
        /// 顶点索引
        /// </summary>
        public int[] indices { get; set; }

        /// <summary>
        /// 纹理
        /// </summary>
        public Texture[] textures { get; set; }

        /// <summary>
        /// OpenGL实例
        /// </summary>
        private OpenGL gl;

        /// <summary>
        /// vao
        /// </summary>
        VertexBufferArray vao = new VertexBufferArray();

        public Mesh(Vertex[] vertices, int[] indices, Texture[] textures,OpenGL gl)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.textures = textures;
            this.gl = gl;
            SetupMesh();
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="shader">所用shader</param>
        /// <param name="gl">OpenGL实例</param>
        public void Draw(ShaderProgram shader, OpenGL gl)
        {
            //以下绑定贴图
            int diffuseNr = 1;
            int specularNr = 1;
            int normalNr = 1;
            int heightNr = 1;
            for (int i = 0; i < textures.Length; i++)
            {
                //激活纹理单元
                gl.ActiveTexture((uint)(OpenGL.GL_TEXTURE0 + i)); 
                string number = "";
                string name = textures[i].type;
                if (name == "texture_diffuse")  //漫反射贴图
                    number = (diffuseNr++).ToString();
                else if (name == "texture_specular")    //高光贴图
                    number = (specularNr++).ToString();
                else if (name == "texture_normal")  //法线贴图
                    number = (normalNr++).ToString();
                else if (name == "texture_height")  //高度图
                    number = (heightNr++).ToString();

                //将纹理单元传递给shader
                shader.SetUniform1(gl, name + number, i);

                //绑定纹理
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, (uint)textures[i].id);
            }

            //绑定vao
            vao.Bind(gl);

            //绘制
            gl.DrawElements(OpenGL.GL_TRIANGLES, indices.Length, indices.Select(x => (uint)x).ToArray());

            //解绑
            gl.BindVertexArray(0);
        }

        /// <summary>
        /// 适配网格
        /// </summary>
        void SetupMesh()
        {
            //创建vao
            vao.Create(gl);

            //绑定vao
            vao.Bind(gl);

            //创建vbo
            var vbo = new VertexBuffer();
            vbo.Create(gl);

            //绑定vbo
            vbo.Bind(gl);

            //获取定点数据
            float[] data = vertices.SelectMany(v => v.ToArray()).ToArray();

            //绑定数据
            gl.BufferData(OpenGL.GL_ARRAY_BUFFER, data, OpenGL.GL_STATIC_DRAW);

            //以下配置顶点属性
            //位置
            gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, Marshal.SizeOf(typeof(Vertex)), IntPtr.Zero);
            gl.EnableVertexAttribArray(0);

            unsafe
            {     
                //法线
                gl.VertexAttribPointer(1, 3, OpenGL.GL_FLOAT, false, Marshal.SizeOf(typeof(Vertex)), new IntPtr(sizeof(vec3)));
                gl.EnableVertexAttribArray(1);

                //纹理坐标
                gl.VertexAttribPointer(2, 2, OpenGL.GL_FLOAT, false, Marshal.SizeOf(typeof(Vertex)), new IntPtr(sizeof(vec3) * 2));
                gl.EnableVertexAttribArray(2);

                //切线
                gl.VertexAttribPointer(3, 3, OpenGL.GL_FLOAT, false, Marshal.SizeOf(typeof(Vertex)), new IntPtr(sizeof(vec3) * 2 + sizeof(vec2)));
                gl.EnableVertexAttribArray(3);

                //二重切线
                gl.VertexAttribPointer(4, 3, OpenGL.GL_FLOAT, false, Marshal.SizeOf(typeof(Vertex)), new IntPtr(sizeof(vec3) * 3 + sizeof(vec2)));
                gl.EnableVertexAttribArray(4);
            }

            //解绑
            vao.Unbind(gl);
        }
    }
}