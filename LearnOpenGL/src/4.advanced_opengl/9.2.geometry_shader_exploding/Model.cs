using Assimp;
using GlmNet;
using SharpGL;
using SharpGL.Shaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace _9._2.geometry_shader_exploding
{
    /// <summary>
    /// 模型
    /// </summary>
    public class Model
    {
        /// <summary>
        /// 已加载纹理
        /// </summary>
        private List<Texture> textures_loaded = new List<Texture>();

        /// <summary>
        /// 网格集合
        /// </summary>
        private List<Mesh> meshes = new List<Mesh>();

        /// <summary>
        /// 模型所在路径
        /// </summary>
        private string directory;

        /// <summary>
        /// 模型名称
        /// </summary>
        private string fileName;

        /// <summary>
        /// OpenGL实例
        /// </summary>
        private OpenGL gl;

        /// <summary>
        /// Assimp上下文
        /// </summary>
        AssimpContext context = new AssimpContext();

        public Model(string path, OpenGL gl)
        {
            fileName = path;
            this.gl = gl;
            FileInfo fInfo = new FileInfo(fileName);
            directory = fInfo.Directory.Name;
            LoadModel(path);
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="shader">使用的shander</param>
        public void Draw(ShaderProgram shader)
        {
            //以此绘制网格
            for (int i = 0; i < meshes.Count; i++)
                meshes[i].Draw(shader, gl);
        }

        /// <summary>
        /// 加载模型
        /// </summary>
        /// <param name="path"></param>
        private void LoadModel(string path)
        {
            //使用AssimpContext加载模型
            var scene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace);

            //递归处理场景中的节点
            ProcessNode(scene.RootNode, scene);
        }

        /// <summary>
        /// 递归处理场景中的节点
        /// </summary>
        /// <param name="node">根节点</param>
        /// <param name="scene">场景</param>
        void ProcessNode(Node node, Scene scene)
        {
            //处理当前节点
            for (int i = 0; i < node.MeshCount; i++)
            {
                //获取对应的网格
                var mesh = scene.Meshes.Find(a => a.Name == node.Name);

                //加入网格列表
                meshes.Add(ProcessMesh(mesh, scene));
            }

            //处理子节点
            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i], scene);
            }
        }

        /// <summary>
        /// 处理Mesh
        /// </summary>
        /// <param name="mesh">网格</param>
        /// <param name="scene">场景</param>
        /// <returns></returns>
        private Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
        {
            //顶点
            List<Vertex> vertices = new List<Vertex>();

            //索引
            List<int> indices = new List<int>();

            //纹理
            List<Texture> textures = new List<Texture>();

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                //顶点
                Vertex vertex = new Vertex();

                //位置信息
                vec3 vector = new vec3();
                vector.x = mesh.Vertices[i].X;
                vector.y = mesh.Vertices[i].Y;
                vector.z = mesh.Vertices[i].Z;
                vertex.Position = vector;

                //法线信息
                vector.x = mesh.Normals[i].X;
                vector.y = mesh.Normals[i].Y;
                vector.z = mesh.Normals[i].Z;
                vertex.Normal = vector;

                //纹理信息
                if (mesh.HasTextureCoords(0))
                {
                    vec2 vec = new vec2();
                    vec.x = mesh.TextureCoordinateChannels[0][i].X;
                    vec.y = mesh.TextureCoordinateChannels[0][i].Y;
                    vertex.TexCoords = vec;
                }
                else
                    vertex.TexCoords = new vec2(0.0f, 0.0f);

                //切线信息
                vector.x = mesh.Tangents[i].X;
                vector.y = mesh.Tangents[i].Y;
                vector.z = mesh.Tangents[i].Z;
                vertex.Tangent = vector;

                //二重切线信息
                vector.x = mesh.BiTangents[i].X;
                vector.y = mesh.BiTangents[i].Y;
                vector.z = mesh.BiTangents[i].Z;
                vertex.Bitangent = vector;
                vertices.Add(vertex);
            }

            //顶点索引信息
            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];
                for (int j = 0; j < face.IndexCount; j++)
                    indices.Add(face.Indices[j]);
            }
            
            //处理材质相关
            Material material = scene.Materials[mesh.MaterialIndex];

            //漫反射贴图
            List<Texture> diffuseMaps = LoadMaterialTextures(material, TextureType.Diffuse, "texture_diffuse");
            textures.AddRange(diffuseMaps);

            //高光贴图
            List<Texture> specularMaps = LoadMaterialTextures(material, TextureType.Specular, "texture_specular");
            textures.AddRange(specularMaps);

            //法线贴图
            List<Texture> normalMaps = LoadMaterialTextures(material, TextureType.Normals, "texture_normal");
            textures.AddRange(normalMaps);

            //高度图
            List<Texture> heightMaps = LoadMaterialTextures(material, TextureType.Height, "texture_height");
            textures.AddRange(heightMaps);

            //生成新的网格
            return new Mesh(vertices.ToArray(), indices.ToArray(), textures.ToArray(), gl);
        }

        /// <summary>
        /// 价值材质纹理
        /// </summary>
        /// <param name="mat">材质</param>
        /// <param name="type">纹理类型</param>
        /// <param name="typeName">类型名称</param>
        /// <returns>纹理集合</returns>
        List<Texture> LoadMaterialTextures(Material mat, TextureType type, string typeName)
        {
            List<Texture> textures = new List<Texture>();
            for (int i = 0; i < mat.GetMaterialTextureCount(type); i++)
            {
                //获取纹理
                mat.GetMaterialTexture(type, i, out TextureSlot textureTmp);
                bool skip = false;
                for (int j = 0; j < textures_loaded.Count(); j++)
                {
                    //避免重复加载
                    if (textures_loaded[j].path == textureTmp.FilePath)
                    {
                        textures.Add(textures_loaded[j]);
                        skip = true;
                        break;
                    }
                }
                if (!skip)
                {   
                    //加载纹理
                    SharpGL.SceneGraph.Assets.Texture txure = new SharpGL.SceneGraph.Assets.Texture();
                    txure.Create(gl, directory + "\\" + textureTmp.FilePath);

                    //生成新的纹理对象
                    Texture texture = new Texture();
                    texture.id = (int)txure.TextureName;
                    texture.type = typeName;
                    texture.path = textureTmp.FilePath;
                    textures.Add(texture);

                    //加入纹理集合
                    textures_loaded.Add(texture);
                }
            }
            return textures;
        }
    }
}