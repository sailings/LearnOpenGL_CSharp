using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;
using GlmNet;

namespace _6._2.cubemaps_environment_mapping
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 屏幕宽度
        /// </summary>
        private const int SCR_WIDTH = 800;

        /// <summary>
        /// 屏幕高度
        /// </summary>
        private const int SCR_HEIGHT = 600;

        /// <summary>
        /// 标题
        /// </summary>
        private string title = "LearnOpenGL";

        /// <summary>
        /// OpenGL实例
        /// </summary>
        private OpenGL GL;

        /// <summary>
        /// 立方体顶点
        /// </summary>
        private float[] cubeVertices = {
        //位置                //法线
        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
         0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
         0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
         0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
        -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
         0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
         0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
         0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
        -0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,

        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

         0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
         0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
         0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
         0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
         0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
         0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
         0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
         0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
         0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
         0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
         0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
         0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
    };

        /// <summary>
        /// 天空盒立方体顶点
        /// </summary>
        private float[] skyboxVertices = {
        -1.0f,  1.0f, -1.0f,
        -1.0f, -1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,
         1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,
        -1.0f, -1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f,  1.0f,
        -1.0f, -1.0f,  1.0f,

         1.0f, -1.0f, -1.0f,
         1.0f, -1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,
        -1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f, -1.0f,  1.0f,
        -1.0f, -1.0f,  1.0f,

        -1.0f,  1.0f, -1.0f,
         1.0f,  1.0f, -1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
        -1.0f,  1.0f,  1.0f,
        -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f, -1.0f,
        -1.0f, -1.0f,  1.0f,
         1.0f, -1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,
        -1.0f, -1.0f,  1.0f,
         1.0f, -1.0f,  1.0f
    };

        /// <summary>
        /// Shader
        /// </summary>
        private ShaderProgram shader = new ShaderProgram();

        /// <summary>
        /// 
        /// </summary>
        private ShaderProgram skyboxShader = new ShaderProgram();

        /// <summary>
        /// cubeVAO
        /// </summary>
        VertexBufferArray cubeVAO = new VertexBufferArray();

        /// <summary>
        /// 
        /// </summary>
        VertexBufferArray skyboxVAO = new VertexBufferArray();
              
        private DateTime startTime = DateTime.Now;

        //摄像机对象
        Camera camera = new Camera(new vec3(0.0f, 0.0f, 3.0f), new vec3(0.0f, 1.0f, 0.0f));

        float lastX = SCR_WIDTH / 2.0f;
        float lastY = SCR_HEIGHT / 2.0f;
        bool firstMouse = true;

        float deltaTime = 0.0f;
        float lastFrame = 0.0f;

        /// <summary>
        /// 天空盒立方体纹理
        /// </summary>
        uint cubemapTexture;

        /// <summary>
        /// 天空盒六个面
        /// </summary>
        List<string> faces = new List<string>
        {
            "skybox/right.jpg",
            "skybox/left.jpg",
            "skybox/top.jpg",
            "skybox/bottom.jpg",
            "skybox/front.jpg",
            "skybox/back.jpg"
        };

        public Form1()
        {
            InitializeComponent();

            openGLControl1.MouseWheel += OpenGLControl1_MouseWheel;
            openGLControl1.MouseMove += OpenGLControl1_MouseMove;

            //创建纹理
            cubemapTexture = LoadCubemap(faces);
        }

        private void OpenGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //以下获取offset
                var xpos = e.X;
                var ypos = e.Y;

                if (firstMouse)
                {
                    lastX = xpos;
                    lastY = ypos;
                    firstMouse = false;
                }

                float xoffset = xpos - lastX;
                float yoffset = lastY - ypos;

                lastX = xpos;
                lastY = ypos;

                //传递给camera处理
                camera.ProcessMouseMovement(xoffset, yoffset);
            }
        }

        private void OpenGLControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            var yoffset = e.Delta / Math.Abs(e.Delta);
            camera.ProcessMouseScroll(yoffset);
        }

        private float GetTime()
        {
            return (float)(DateTime.Now - startTime).TotalSeconds;
        }

        /// <summary>
        /// OpenGL绘制事件内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OpenGLControl1_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {
            float currentFrame = GetTime();
            deltaTime = currentFrame - lastFrame;
            lastFrame = currentFrame;

            //清除颜色和深度缓冲
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //使用当前着色器
            GL.UseProgram(shader.ShaderProgramObject);

            //设置MVP矩阵
            mat4 model = new mat4(1.0f);
            mat4 view = camera.GetViewMatrix();
            mat4 projection = glm.perspective(glm.radians(camera.Zoom), (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);
            shader.SetUniformMatrix4(GL,"model", model.to_array());
            shader.SetUniformMatrix4(GL,"view", view.to_array());
            shader.SetUniformMatrix4(GL,"projection", projection.to_array());
            //设置摄像机位置
            shader.SetUniform3(GL,"cameraPos", camera.Position.x,camera.Position.y,camera.Position.z);

            //绑定立方体VAO
            cubeVAO.Bind(GL);
            //激活纹理单元
            GL.ActiveTexture(OpenGL.GL_TEXTURE0);
            //绑定贴图
            GL.BindTexture(OpenGL.GL_TEXTURE_CUBE_MAP,cubemapTexture);
            //绘制
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);
            //解绑VAO
            cubeVAO.Unbind(GL);

            //修改深度比较函数，让天空盒绘制在物体后面
            GL.DepthFunc(OpenGL.GL_LEQUAL);

            //使用着色器
            GL.UseProgram(skyboxShader.ShaderProgramObject);
            //移除观察矩阵中的位移
            view[3] = new vec4(0, 0, 0, 0);
            //传递观察和投影矩阵
            skyboxShader.SetUniformMatrix4(GL,"view", view.to_array());
            skyboxShader.SetUniformMatrix4(GL,"projection", projection.to_array());
            //绑定VAO
            skyboxVAO.Bind(GL);
            //激活纹理单元
            GL.ActiveTexture(OpenGL.GL_TEXTURE0);
            //绑定贴图
            GL.BindTexture(OpenGL.GL_TEXTURE_CUBE_MAP,cubemapTexture);
            //绘制
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);
            //解绑
            skyboxVAO.Unbind(GL);
            //恢复深度比较函数
            GL.DepthFunc(OpenGL.GL_LESS); 
            
            //设置标题，显示FPS
            Text = title + $"-FPS[{openGLControl1.FPS}]";
        }

        /// <summary>
        /// OpenGL初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenGLControl1_OpenGLInitialized(object sender, EventArgs e)
        {            
            //获取OpenGL对象
            GL = openGLControl1.OpenGL;

            //启用深度测试
            GL.Enable(OpenGL.GL_DEPTH_TEST);            

            //创建着色器
            shader.Create(GL,"6.2.cubemaps.vs", "6.2.cubemaps.fs");
            skyboxShader.Create(GL,"6.2.skybox.vs", "6.2.skybox.fs");

            //使用当前着色器
            GL.UseProgram(shader.ShaderProgramObject);
            //设置片元着色器中的skybox为0号纹理单元
            shader.SetUniform1(GL, "skybox", 0);

            GL.UseProgram(skyboxShader.ShaderProgramObject);
            skyboxShader.SetUniform1(GL, "skybox", 0);

            //创建立方体VAO
            cubeVAO.Create(GL);
            cubeVAO.Bind(GL);
            //创建VBO
            var vbo = new VertexBuffer();
            vbo.Create(GL);
            vbo.Bind(GL);
            //绑定数据
            GL.BufferData(OpenGL.GL_ARRAY_BUFFER, cubeVertices, OpenGL.GL_STATIC_DRAW);
            //配置顶点属性
            GL.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 6 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, OpenGL.GL_FLOAT, false, 6 * sizeof(float), new IntPtr(3 * sizeof(float)));
            GL.EnableVertexAttribArray(1);
            cubeVAO.Unbind(GL);

            skyboxVAO.Create(GL);
            skyboxVAO.Bind(GL);
            //重新设置vbo数据
            vbo = new VertexBuffer();
            vbo.Create(GL);
            vbo.Bind(GL);
            //绑定数据
            GL.BufferData(OpenGL.GL_ARRAY_BUFFER, skyboxVertices, OpenGL.GL_STATIC_DRAW);
            //配置顶点属性
            GL.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 3 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(0);
            skyboxVAO.Unbind(GL);

            //设置窗体的大小
            Size = new Size(SCR_WIDTH, SCR_HEIGHT);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //根据按下的键，更新摄像机位置信息
            if (keyData == Keys.W)
                camera.ProcessKeyboard(Camera_Movement.FORWARD, deltaTime);
            if (keyData == Keys.S)
                camera.ProcessKeyboard(Camera_Movement.BACKWARD, deltaTime);
            if (keyData == Keys.A)
                camera.ProcessKeyboard(Camera_Movement.LEFT, deltaTime);
            if (keyData == Keys.D)
                camera.ProcessKeyboard(Camera_Movement.RIGHT, deltaTime);

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// 加载天空盒六个面的贴图
        /// </summary>
        /// <param name="faces"></param>
        /// <returns></returns>
        private uint LoadCubemap(List<string> faces)
        {
            uint[] ids = new uint[1];
            GL.GenTextures(1, ids);
            uint textureID = ids[0];
            GL.BindTexture(OpenGL.GL_TEXTURE_CUBE_MAP, textureID);

            for (int i = 0; i < faces.Count; i++)
            {
                Texture texture = new Texture();
                texture.Create(GL, faces[i], false, (uint)(OpenGL.GL_TEXTURE_CUBE_MAP_POSITIVE_X + i));
            }
            GL.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            GL.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            GL.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
            GL.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);
            GL.TexParameter(OpenGL.GL_TEXTURE_CUBE_MAP, OpenGL.GL_TEXTURE_WRAP_R, OpenGL.GL_CLAMP_TO_EDGE);

            return textureID;
        }
    }
}