﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpGL.Shaders
{
    public class ShaderProgram
    {
        private readonly Shader vertexShader = new Shader();
        private readonly Shader fragmentShader = new Shader();
        private readonly Shader geometryShader = new Shader();

        public Shader FragmentShader
        {
            get {
                return fragmentShader;
            }
        }

        /// <summary>
        /// Creates the shader program.
        /// </summary>
        /// <param name="gl">The gl.</param>
        /// <param name="vertexShaderSource">The vertex shader source.</param>
        /// <param name="fragmentShaderSource">The fragment shader source.</param>
        /// <param name="attributeLocations">The attribute locations. This is an optional array of
        /// uint attribute locations to their names.</param>
        /// <exception cref="ShaderCompilationException"></exception>
        public void CreateFromSource(OpenGL gl, string vertexShaderSource, string fragmentShaderSource,string geometryShaderSource="",
            Dictionary<uint, string> attributeLocations=null)
        {
            //  Create the shaders.
            vertexShader.Create(gl, OpenGL.GL_VERTEX_SHADER, vertexShaderSource);
            fragmentShader.Create(gl, OpenGL.GL_FRAGMENT_SHADER, fragmentShaderSource);

            shaderProgramObject = gl.CreateProgram();
            if (!string.IsNullOrEmpty(geometryShaderSource))
            {
                geometryShader.Create(gl,OpenGL.GL_GEOMETRY_SHADER, geometryShaderSource);
                gl.AttachShader(shaderProgramObject, geometryShader.ShaderObject);
            }
            //  Create the program, attach the shaders.
            gl.AttachShader(shaderProgramObject, vertexShader.ShaderObject);
            gl.AttachShader(shaderProgramObject, fragmentShader.ShaderObject);

            //  Before we link, bind any vertex attribute locations.
            if (attributeLocations != null)
            {
                foreach (var vertexAttributeLocation in attributeLocations)
                    gl.BindAttribLocation(shaderProgramObject, vertexAttributeLocation.Key, vertexAttributeLocation.Value);
            }

            //  Now we can link the program.
            gl.LinkProgram(shaderProgramObject);

            //  Now that we've compiled and linked the shader, check it's link status. If it's not linked properly, we're
            //  going to throw an exception.
            if (GetLinkStatus(gl) == false)
            {
                throw new ShaderCompilationException(string.Format("Failed to link shader program with ID {0}.", shaderProgramObject), GetInfoLog(gl));
            }
        }

        public void Create(OpenGL gl, string vertexFile, string fragmentFile,string geometryFile="")
        {
            string vertexShaderSource, fragmentShaderSource, geometryShaderSource="";
            using (StreamReader sr = new StreamReader(vertexFile))
            {
                vertexShaderSource = sr.ReadToEnd();
            }
            using (StreamReader sr = new StreamReader(fragmentFile))
            {
                fragmentShaderSource = sr.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(geometryFile))
            {
                using (StreamReader sr = new StreamReader(geometryFile))
                {
                    geometryShaderSource = sr.ReadToEnd();
                }
            }
            CreateFromSource(gl, vertexShaderSource, fragmentShaderSource, geometryShaderSource, null);
        }

        public void Delete(OpenGL gl)
        {
            gl.DetachShader(shaderProgramObject, vertexShader.ShaderObject);
            gl.DetachShader(shaderProgramObject, fragmentShader.ShaderObject);
            vertexShader.Delete(gl);
            fragmentShader.Delete(gl);
            gl.DeleteProgram(shaderProgramObject);
            shaderProgramObject = 0;
        }

        public int GetAttributeLocation(OpenGL gl, string attributeName)
        {
            return gl.GetAttribLocation(shaderProgramObject, attributeName);
        }

        public void BindAttributeLocation(OpenGL gl, uint location, string attribute)
        {
            gl.BindAttribLocation(shaderProgramObject, location, attribute);
        }

        public void Bind(OpenGL gl)
        {
            gl.UseProgram(shaderProgramObject);
        }

        public void Unbind(OpenGL gl)
        {
            gl.UseProgram(0);
        }

        public bool GetLinkStatus(OpenGL gl)
        {
            int[] parameters = new int[] { 0 };
            gl.GetProgram(shaderProgramObject, OpenGL.GL_LINK_STATUS, parameters);
            return parameters[0] == OpenGL.GL_TRUE;
        }

        public string GetInfoLog(OpenGL gl)
        {
            //  Get the info log length.
            int[] infoLength = new int[] { 0 };
            gl.GetProgram(shaderProgramObject, OpenGL.GL_INFO_LOG_LENGTH, infoLength);
            int bufSize = infoLength[0];

            //  Get the compile info.
            StringBuilder il = new StringBuilder(bufSize);
            gl.GetProgramInfoLog(shaderProgramObject, bufSize, IntPtr.Zero, il);

            return il.ToString();
        }

        public void AssertValid(OpenGL gl)
        {
            if (vertexShader.GetCompileStatus(gl) == false)
                throw new Exception(vertexShader.GetInfoLog(gl));
            if (fragmentShader.GetCompileStatus(gl) == false)
                throw new Exception(fragmentShader.GetInfoLog(gl));
            if (GetLinkStatus(gl) == false)
                throw new Exception(GetInfoLog(gl));
        }

        public void SetUniform1(OpenGL gl, string uniformName, float v1)
        {
            gl.Uniform1(GetUniformLocation(gl, uniformName), v1);
        }

        public void SetUniform1(OpenGL gl, string uniformName, int v1)
        {
            gl.Uniform1(GetUniformLocation(gl, uniformName), v1);
        }

        public void SetUniform3(OpenGL gl, string uniformName, float v1, float v2, float v3)
        {
            gl.Uniform3(GetUniformLocation(gl, uniformName), v1, v2, v3);
        }

        public void SetUniform4(OpenGL gl, string uniformName, float v0, float v1, float v2, float v3)
        {
            gl.Uniform4(GetUniformLocation(gl, uniformName), v0, v1, v2, v3);
        }

        public void SetUniformMatrix3(OpenGL gl, string uniformName, float[] m)
        {
            gl.UniformMatrix3(GetUniformLocation(gl, uniformName), 1, false, m);
        }

        public void SetUniformMatrix4(OpenGL gl, string uniformName, float[] m)
        {
            gl.UniformMatrix4(GetUniformLocation(gl, uniformName), 1, false, m);
        }

        public int GetUniformLocation(OpenGL gl, string uniformName)
        {
            //  If we don't have the uniform name in the dictionary, get it's 
            //  location and add it.
            if (uniformNamesToLocations.ContainsKey(uniformName) == false)
            {
                uniformNamesToLocations[uniformName] = gl.GetUniformLocation(shaderProgramObject, uniformName);
                //  TODO: if it's not found, we should probably throw an exception.
            }

            //  Return the uniform location.
            return uniformNamesToLocations[uniformName];
        }

        /// <summary>
        /// Gets the shader program object.
        /// </summary>
        /// <value>
        /// The shader program object.
        /// </value>
        public uint ShaderProgramObject
        {
            get { return shaderProgramObject; }
        }

        private uint shaderProgramObject;

        /// <summary>
        /// A mapping of uniform names to locations. This allows us to very easily specify 
        /// uniform data by name, quickly looking up the location first if needed.
        /// </summary>
        private readonly Dictionary<string, int> uniformNamesToLocations = new Dictionary<string, int>();
    }
}
