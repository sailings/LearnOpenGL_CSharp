using Assimp;
using GlmNet;
using SharpGL;
using SharpGL.Shaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace _1.model_loading
{
    class Model
    {
        /*  Model Data */
        List<Texture> textures_loaded = new List<Texture>();    // stores all the textures loaded so far, optimization to make sure textures aren't loaded more than once.
        List<Mesh> meshes = new List<Mesh>();
        string directory;
        string fileName;
        bool gammaCorrection;
        private OpenGL gl;
        AssimpContext context = new AssimpContext();

        /*  Functions   */
        // constructor, expects a filepath to a 3D model.
        public Model(string path, OpenGL gl, bool gamma = false)
        {
            fileName = path;
            this.gl = gl;
            gammaCorrection = gamma;
            FileInfo fInfo = new FileInfo(fileName);
            directory = fInfo.Directory.Name;
            LoadModel(path);
        }

        // draws the model, and thus all its meshes
        public void Draw(ShaderProgram shader)
        {
            for (int i = 0; i < meshes.Count; i++)
                meshes[i].Draw(shader, gl);
        }

        /*  Functions   */
        // loads a model with supported ASSIMP extensions from file and stores the resulting meshes in the meshes vector.
        void LoadModel(string path)
        {
            // read file via ASSIMP
            //Assimp::Importer importer;
            //const aiScene* scene = importer.ReadFile(path, aiProcess_Triangulate | aiProcess_FlipUVs | aiProcess_CalcTangentSpace);
            //// check for errors
            //if(!scene || scene->mFlags & AI_SCENE_FLAGS_INCOMPLETE || !scene->mRootNode) // if is Not Zero
            //{
            //    cout << "ERROR::ASSIMP:: " << importer.GetErrorString() << endl;
            //    return;
            //}
            // retrieve the directory path of the filepath
            var scene = context.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace);

            //directory = path.substr(0, path.find_last_of('/'));

            // process ASSIMP's root node recursively
            ProcessNode(scene.RootNode, scene);
        }

        // processes a node in a recursive fashion. Processes each individual mesh located at the node and repeats this process on its children nodes (if any).
        void ProcessNode(Node node, Scene scene)
        {
            // process each mesh located at the current node
            for (int i = 0; i < node.MeshCount; i++)
            {
                // the node object only contains indices to index the actual objects in the scene. 
                // the scene contains all the data, node is just to keep stuff organized (like relations between nodes).
                //var mesh = scene->mMeshes[node->mMeshes[i]];
                //var mesh = scene.Meshes[i];
                var mesh = scene.Meshes.Find(a => a.Name == node.Name);
                meshes.Add(ProcessMesh(mesh, scene));
                //meshes.push_back(ProcessMesh(mesh, scene));
            }
            // after we've processed all of the meshes (if any) we then recursively process each of the children nodes
            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i], scene);
            }
        }

        Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
        {
            // data to fill
            List<Vertex> vertices = new List<Vertex>();
            List<int> indices = new List<int>();
            List<Texture> textures = new List<Texture>();

            // Walk through each of the mesh's vertices
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vertex vertex = new Vertex();
                vec3 vector = new vec3(); // we declare a placeholder vector since assimp uses its own vector class that doesn't directly convert to glm's vec3 class so we transfer the data to this placeholder glm::vec3 first.
                                          // positions

                vector.x = mesh.Vertices[i].X;
                vector.y = mesh.Vertices[i].Y;
                vector.z = mesh.Vertices[i].Z;

                vertex.Position = vector;
                // normals
                vector.x = mesh.Normals[i].X;
                vector.y = mesh.Normals[i].Y;
                vector.z = mesh.Normals[i].Z;
                vertex.Normal = vector;
                // texture coordinates
                if (mesh.HasTextureCoords(0))
                //if (mesh->mTextureCoords[0]) // does the mesh contain texture coordinates?
                {
                    vec2 vec = new vec2();
                    // a vertex can contain up to 8 different texture coordinates. We thus make the assumption that we won't 
                    // use models where a vertex can have multiple texture coordinates so we always take the first set (0).

                    vec.x = mesh.TextureCoordinateChannels[0][i].X;
                    vec.y = mesh.TextureCoordinateChannels[0][i].Y;
                    vertex.TexCoords = vec;
                }
                else
                    vertex.TexCoords = new vec2(0.0f, 0.0f);
                // tangent

                vector.x = mesh.Tangents[i].X;
                vector.y = mesh.Tangents[i].Y;
                vector.z = mesh.Tangents[i].Z;
                vertex.Tangent = vector;
                // bitangent

                vector.x = mesh.BiTangents[i].X;
                vector.y = mesh.BiTangents[i].Y;
                vector.z = mesh.BiTangents[i].Z;
                vertex.Bitangent = vector;
                vertices.Add(vertex);
            }
            // now wak through each of the mesh's faces (a face is a mesh its triangle) and retrieve the corresponding vertex indices.
            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];
                // retrieve all indices of the face and store them in the indices vector
                for (int j = 0; j < face.IndexCount; j++)
                    indices.Add(face.Indices[j]);
            }
            // process materials
            Material material = scene.Materials[mesh.MaterialIndex];
            // we assume a convention for sampler names in the shaders. Each diffuse texture should be named
            // as 'texture_diffuseN' where N is a sequential number ranging from 1 to MAX_SAMPLER_NUMBER. 
            // Same applies to other texture as the following list summarizes:
            // diffuse: texture_diffuseN
            // specular: texture_specularN
            // normal: texture_normalN

            // 1. diffuse maps

            List<Texture> diffuseMaps = LoadMaterialTextures(material, TextureType.Diffuse, "texture_diffuse");
            //textures.insert(textures.end(), diffuseMaps.begin(), diffuseMaps.end());
            textures.AddRange(diffuseMaps);
            // 2. specular maps

            List<Texture> specularMaps = LoadMaterialTextures(material, TextureType.Specular, "texture_specular");
            //textures.insert(textures.end(), specularMaps.begin(), specularMaps.end());
            textures.AddRange(specularMaps);
            // 3. normal maps

            List<Texture> normalMaps = LoadMaterialTextures(material, TextureType.Normals, "texture_normal");
            //textures.insert(textures.end(), normalMaps.begin(), normalMaps.end());
            textures.AddRange(normalMaps);
            // 4. height maps

            List<Texture> heightMaps = LoadMaterialTextures(material, TextureType.Height, "texture_height");
            //textures.insert(textures.end(), heightMaps.begin(), heightMaps.end());
            textures.AddRange(heightMaps);

            // return a mesh object created from the extracted mesh data
            return new Mesh(vertices.ToArray(), indices.ToArray(), textures.ToArray(), gl);
        }

        // checks all material textures of a given type and loads the textures if they're not loaded yet.
        // the required info is returned as a Texture struct.
        List<Texture> LoadMaterialTextures(Material mat, TextureType type, string typeName)
        {
            List<Texture> textures = new List<Texture>();
            for (int i = 0; i < mat.GetMaterialTextureCount(type); i++)
            {
                //aiString str;
                //mat->GetTexture(type, i, &str);
                mat.GetMaterialTexture(type, i, out TextureSlot textureTmp);
                // check if texture was loaded before and if so, continue to next iteration: skip loading a new texture
                bool skip = false;
                for (int j = 0; j < textures_loaded.Count(); j++)
                {
                    if (textures_loaded[j].path == textureTmp.FilePath)
                    //if (std::strcmp(textures_loaded[j].path.data(), str.C_Str()) == 0)
                    {
                        textures.Add(textures_loaded[j]);
                        //textures.push_back(textures_loaded[j]);
                        skip = true; // a texture with the same filepath has already been loaded, continue to next one. (optimization)
                        break;
                    }
                }
                if (!skip)
                {   // if texture hasn't been loaded already, load it
                    SharpGL.SceneGraph.Assets.Texture txure = new SharpGL.SceneGraph.Assets.Texture();
                    txure.Create(gl, directory + "\\" + textureTmp.FilePath);
                    //txure.TextureName
                    Texture texture = new Texture();
                    //texture.id = TextureFromFile(str.C_Str(), this->directory);
                    texture.id = (int)txure.TextureName;
                    texture.type = typeName;
                    //texture.path = str.C_Str();
                    texture.path = textureTmp.FilePath;
                    textures.Add(texture);
                    textures_loaded.Add(texture);
                }
            }
            return textures;
        }
    }
}
