using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using GlmNet;
using SharpGL;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;

namespace _1.model_loading
{
    public struct Vertex
    {
        // position
        public vec3 Position { get; set; }
        // normal
        public vec3 Normal { get; set; }
        // texCoords
        public vec2 TexCoords { get; set; }
        // tangent
        public vec3 Tangent { get; set; }
        // bitangent
        public vec3 Bitangent { get; set; }

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
    };
    public struct Texture
    {
        public int id { get; set; }
        public string type { get; set; }
        public string path { get; set; }
    };
    public class Mesh
    {
    /*  Mesh Data  */
        public Vertex[] vertices { get; set; }
        public int[] indices { get; set; }
        public Texture[] textures { get; set; }

        private OpenGL gl;

        //uint VAO;

        /// <summary>
        /// vao
        /// </summary>
        VertexBufferArray vao = new VertexBufferArray();

        /*  Functions  */
        // constructor
        public Mesh(Vertex[] vertices, int[] indices, Texture[] textures,OpenGL gl)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.textures = textures;
            this.gl = gl;
            // now that we have all the required data, set the vertex buffers and its attribute pointers.
            SetupMesh();
        }

        // render the mesh
        public void Draw(ShaderProgram shader, OpenGL gl)
        {
            // bind appropriate textures
            int diffuseNr = 1;
            int specularNr = 1;
            int normalNr = 1;
            int heightNr = 1;
            for (int i = 0; i < textures.Length; i++)
            {
                gl.ActiveTexture((uint)(OpenGL.GL_TEXTURE0 + i)); // active proper texture unit before binding
                                                                  // retrieve texture number (the N in diffuse_textureN)
                string number = "";
                string name = textures[i].type;
                if (name == "texture_diffuse")
                    number = (diffuseNr++).ToString();
                else if (name == "texture_specular")
                    number = (specularNr++).ToString(); // transfer unsigned int to stream
                else if (name == "texture_normal")
                    number = (normalNr++).ToString(); // transfer unsigned int to stream
                else if (name == "texture_height")
                    number = (heightNr++).ToString(); // transfer unsigned int to stream

                // now set the sampler to the correct texture unit
                //glUniform1i(glGetUniformLocation(shader.ShaderObject, (name + number)), i);
                //gl.Uniform1(gl.GetUniformLocation(shader.ShaderObject, name + number), i);
                shader.SetUniform1(gl, name + number, i);
                // and finally bind the texture
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, (uint)textures[i].id);
            }

            // draw mesh
            //gl.BindVertexArray(VAO);
            vao.Bind(gl);
            gl.DrawElements(OpenGL.GL_TRIANGLES, indices.Length, indices.Select(x => (uint)x).ToArray());
            gl.BindVertexArray(0);

            // always good practice to set everything back to defaults once configured.
            gl.ActiveTexture(OpenGL.GL_TEXTURE0);
        }

        /*  Render data  */
        //VertexBuffer vbo = new VertexBuffer();
        //VertexBuffer ebo = new VertexBuffer();

        /*  Functions    */
        // initializes all the buffer objects/arrays
        void SetupMesh()
        {
            //// create buffers/arrays
            //gl.GenVertexArrays(1, &VAO);
            //gl.GenBuffers(1, &VBO);
            //glGenBuffers(1, &EBO);

            //glBindVertexArray(VAO);
            //// load data into vertex buffers
            //glBindBuffer(GL_ARRAY_BUFFER, VBO);
            //// A great thing about structs is that their memory layout is sequential for all its items.
            //// The effect is that we can simply pass a pointer to the struct and it translates perfectly to a glm::vec3/2 array which
            //// again translates to 3/2 floats which translates to a byte array.
            //glBufferData(GL_ARRAY_BUFFER, vertices.size() * sizeof(Vertex), &vertices[0], GL_STATIC_DRAW);

            //glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
            //glBufferData(GL_ELEMENT_ARRAY_BUFFER, indices.size() * sizeof(unsigned int), &indices[0], GL_STATIC_DRAW);

            //// set the vertex attribute pointers
            //;
            //// vertex Positions
            //glEnableVertexAttribArray(0);
            //glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)0);
            //// vertex normals
            //glEnableVertexAttribArray(1);
            //glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, Normal));
            //// vertex texture coords
            //glEnableVertexAttribArray(2);
            //glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, TexCoords));
            //// vertex tangent
            //glEnableVertexAttribArray(3);
            //glVertexAttribPointer(3, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, Tangent));
            //// vertex bitangent
            //glEnableVertexAttribArray(4);
            //glVertexAttribPointer(4, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, Bitangent));

            //glBindVertexArray(0);

            vao.Create(gl);
            vao.Bind(gl);

            //创建vbo
            var vbo = new VertexBuffer();
            vbo.Create(gl);

            //绑定vbo
            vbo.Bind(gl);

            float[] data = vertices.SelectMany(v => v.ToArray()).ToArray();
            //绑定数据
            gl.BufferData(OpenGL.GL_ARRAY_BUFFER, data, OpenGL.GL_STATIC_DRAW);

            //配置顶点属性
            gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, Marshal.SizeOf(typeof(Vertex)), IntPtr.Zero);
            gl.EnableVertexAttribArray(0);

            unsafe
            {                
                gl.VertexAttribPointer(1, 3, OpenGL.GL_FLOAT, false, Marshal.SizeOf(typeof(Vertex)), new IntPtr(sizeof(vec3)));
                gl.EnableVertexAttribArray(1);

                gl.VertexAttribPointer(2, 2, OpenGL.GL_FLOAT, false, Marshal.SizeOf(typeof(Vertex)), new IntPtr(sizeof(vec3) * 2));
                gl.EnableVertexAttribArray(2);

                gl.VertexAttribPointer(3, 3, OpenGL.GL_FLOAT, false, Marshal.SizeOf(typeof(Vertex)), new IntPtr(sizeof(vec3) * 2 + sizeof(vec2)));
                gl.EnableVertexAttribArray(3);

                gl.VertexAttribPointer(4, 3, OpenGL.GL_FLOAT, false, Marshal.SizeOf(typeof(Vertex)), new IntPtr(sizeof(vec3) * 3 + sizeof(vec2)));
                gl.EnableVertexAttribArray(4);
            }

            vao.Unbind(gl);
        }
    }
}