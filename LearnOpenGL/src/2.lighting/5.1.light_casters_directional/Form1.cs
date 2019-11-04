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

namespace _5._1.light_casters_directional
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
        /// 顶点
        /// </summary>
        private float[] vertices = {
         //位置               //法线                //纹理坐标
        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  0.0f,
         0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  0.0f,
         0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  1.0f,
         0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f,  1.0f,
        -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  1.0f,
        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f,  0.0f,

        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  0.0f,
         0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  0.0f,
         0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  1.0f,
         0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f,  1.0f,
        -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  1.0f,
        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f,  0.0f,

        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  1.0f,
        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
        -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f,  0.0f,
        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f,  0.0f,

         0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f,
         0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  1.0f,
         0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
         0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  1.0f,
         0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f,  0.0f,
         0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f,  0.0f,

        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  1.0f,
         0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  1.0f,
         0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  0.0f,
         0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f,  0.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  0.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f,  1.0f,

        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  1.0f,
         0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  1.0f,
         0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  0.0f,
         0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  0.0f,
        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f,  1.0f
    };

        /// <summary>
        /// 立方体位置
        /// </summary>
        private vec3[] cubePositions = {
        new vec3( 0.0f,  0.0f,  0.0f),
        new vec3( 2.0f,  5.0f, -15.0f),
        new vec3(-1.5f, -2.2f, -2.5f),
        new vec3(-3.8f, -2.0f, -12.3f),
        new vec3( 2.4f, -0.4f, -3.5f),
        new vec3(-1.7f,  3.0f, -7.5f),
        new vec3( 1.3f, -2.0f, -2.5f),
        new vec3( 1.5f,  2.0f, -2.5f),
        new vec3( 1.5f,  0.2f, -1.5f),
        new vec3(-1.3f,  1.0f, -1.5f)
    };

        /// <summary>
        /// 立方体Shader
        /// </summary>
        private ShaderProgram cubeShaderProgram = new ShaderProgram();

        /// <summary>
        /// 光源Shader
        /// </summary>
        private ShaderProgram lampShaderProgram = new ShaderProgram();

        /// <summary>
        /// 立方体VAO
        /// </summary>
        VertexBufferArray cubeVAO = new VertexBufferArray();

        /// <summary>
        /// 光源VAO
        /// </summary>
        VertexBufferArray lightVAO = new VertexBufferArray();

        private DateTime startTime = DateTime.Now;

        //摄像机对象
        Camera camera = new Camera(new vec3(0.0f, 0.0f, 3.0f), new vec3(0.0f, 1.0f, 0.0f));

        float lastX = SCR_WIDTH / 2.0f;
        float lastY = SCR_HEIGHT / 2.0f;
        bool firstMouse = true;

        float deltaTime = 0.0f;
        float lastFrame = 0.0f;

        /// <summary>
        /// 光源位置
        /// </summary>
        vec3 lightPos = new vec3(1.2f, 1.0f, 2.0f);

        /// <summary>
        /// 光照贴图
        /// </summary>
        private Texture diffuseMap = new Texture();

        /// <summary>
        /// 高光贴图
        /// </summary>
        private Texture specularMap = new Texture();

        public Form1()
        {
            InitializeComponent();

            camera.Zoom=90.0f;

            openGLControl1.MouseWheel += OpenGLControl1_MouseWheel;
            openGLControl1.MouseMove += OpenGLControl1_MouseMove;

            //加载贴图
            diffuseMap.Create(GL, "container2.png");
            specularMap.Create(GL, "container2_specular.png");
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

            //清除，以颜色填充
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            //清除颜色缓冲，深度缓冲
            GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //使用立方体着色器
            GL.UseProgram(cubeShaderProgram.ShaderProgramObject);

            //设置当前激活的纹理单元
            GL.ActiveTexture(OpenGL.GL_TEXTURE0);

            //绑定纹理贴图
            diffuseMap.Bind(GL);

            GL.ActiveTexture(OpenGL.GL_TEXTURE1);

            specularMap.Bind(GL);

            //设置光源位置和观察视角
            cubeShaderProgram.SetUniform3(GL,"light.direction", -0.2f, -1.0f, -0.3f);
            cubeShaderProgram.SetUniform3(GL, "viewPos", camera.Position.x, camera.Position.y, camera.Position.z);

            //光源信息
            cubeShaderProgram.SetUniform3(GL, "light.ambient", 0.2f, 0.2f, 0.2f);
            cubeShaderProgram.SetUniform3(GL, "light.diffuse", 0.5f, 0.5f, 0.5f);
            cubeShaderProgram.SetUniform3(GL, "light.specular", 1.0f, 1.0f, 1.0f);

            //材质
            cubeShaderProgram.SetUniform1(GL, "material.shininess", 32.0f);

            //传递三个矩阵到着色器中
            mat4 projection = glm.perspective(glm.radians(camera.Zoom), (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);
            mat4 view = camera.GetViewMatrix();
            cubeShaderProgram.SetUniformMatrix4(GL, "projection", projection.to_array());
            cubeShaderProgram.SetUniformMatrix4(GL, "view", view.to_array());

            //绑定立方体VAO
            cubeVAO.Bind(GL);

            //绘制
            for (int i = 0; i < 10; i++)
            {
                mat4 modelCube = new mat4(1.0f);
                modelCube = glm.translate(modelCube, cubePositions[i]);
                float angle = 20.0f * i;
                modelCube = glm.rotate(modelCube, glm.radians(angle), new vec3(1.0f, 0.3f, 0.5f));

                cubeShaderProgram.SetUniformMatrix4(GL, "model", modelCube.to_array());

                GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);
            }

            //使用光源着色器
            GL.UseProgram(lampShaderProgram.ShaderProgramObject);

            //传递三个矩阵到着色器中
            lampShaderProgram.SetUniformMatrix4(GL, "projection", projection.to_array());
            lampShaderProgram.SetUniformMatrix4(GL, "view", view.to_array());
            mat4 model = new mat4(1.0f);
            model = glm.translate(model, lightPos);
            model = glm.scale(model, new vec3(0.2f));
            lampShaderProgram.SetUniformMatrix4(GL, "model", model.to_array());

            //绑定光源VAO
            lightVAO.Bind(GL);

            //绘制
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);

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

            //创建两个Shader
            cubeShaderProgram.Create(GL, "5.1.light_casters.vs", "5.1.light_casters.fs");
            lampShaderProgram.Create(GL, "5.1.lamp.vs", "5.1.lamp.fs");

            //使用当前着色器
            cubeShaderProgram.Bind(GL);

            //设置材质中的漫反射贴图为0号纹理单元
            cubeShaderProgram.SetUniform1(GL, "material.diffuse", 0);

            //设置材质中的高光贴图为1号纹理单元
            cubeShaderProgram.SetUniform1(GL, "material.specular", 1);

            //创建立方体VAO
            cubeVAO.Create(GL);

            //绑定立方体VAO
            cubeVAO.Bind(GL);

            //创建vbo
            var vbo = new VertexBuffer();
            vbo.Create(GL);

            //绑定vbo
            vbo.Bind(GL);

            //绑定数据
            GL.BufferData(OpenGL.GL_ARRAY_BUFFER, vertices, OpenGL.GL_STATIC_DRAW);

            //配置顶点属性
            GL.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 8 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, OpenGL.GL_FLOAT, false, 8 * sizeof(float), new IntPtr(3 * sizeof(float)));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 2, OpenGL.GL_FLOAT, false, 8 * sizeof(float), new IntPtr(6 * sizeof(float)));
            GL.EnableVertexAttribArray(2);

            //创建光源VAO
            lightVAO.Create(GL);

            //绑定光源VAO
            lightVAO.Bind(GL);

            //将vbo绑定到当前的光源VAO
            vbo.Bind(GL);

            //绑定数据
            GL.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 8 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(0);

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
    }
}