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

namespace _7._2.camera_keyboard_dt
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
        -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
         0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
        -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
         0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
         0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
         0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
        -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

        -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
         0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
         0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
         0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
         0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
         0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
         0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

        -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
    };

        /// <summary>
        /// 立方体位置信息
        /// </summary>
        vec3[] cubePositions = {
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
        /// Shader
        /// </summary>
        private ShaderProgram shaderProgram = new ShaderProgram();

        /// <summary>
        /// vao
        /// </summary>
        VertexBufferArray vao = new VertexBufferArray();

        /// <summary>
        /// 顶点着色器内容
        /// </summary>
        private string vertexShaderSource;

        /// <summary>
        /// 片元着色器内容
        /// </summary>
        private string fragmentShaderSource;

        /// <summary>
        /// 纹理1
        /// </summary>
        private Texture texture1 = new Texture();

        /// <summary>
        /// 纹理2
        /// </summary>
        private Texture texture2 = new Texture();

        private DateTime startTime = DateTime.Now;

        //摄像机位置
        vec3 cameraPos = new vec3(0.0f, 0.0f, 3.0f);

        //定义摄像机前面
        vec3 cameraFront = new vec3(0.0f, 0.0f, -1.0f);

        //定义摄像机上面
        vec3 cameraUp = new vec3(0.0f, 1.0f, 0.0f);

        float deltaTime = 0.0f;
        float lastFrame = 0.0f;

        public Form1()
        {
            InitializeComponent();

            //创建纹理
            texture1.Create(GL, "container.jpg");

            //创建纹理
            texture2.Create(GL, "awesomeface.png");
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

            //使用着色器
            GL.UseProgram(shaderProgram.ShaderProgramObject);

            //设置当前激活的纹理单元
            GL.ActiveTexture(OpenGL.GL_TEXTURE0);

            //绑定纹理
            texture1.Bind(GL);

            //设置当前激活的纹理单元
            GL.ActiveTexture(OpenGL.GL_TEXTURE1);

            //绑定纹理
            texture2.Bind(GL);

            //绑定vao
            vao.Bind(GL);

            //获取view矩阵
            mat4 view = glm.lookAt(cameraPos, cameraPos + cameraFront, cameraUp);

            //传递给着色器
            shaderProgram.SetUniformMatrix4(GL, "view", view.to_array());

            for (int i = 0; i < 10; i++)
            {
                //设置model矩阵
                mat4 model = new mat4(1.0f);
                model = glm.translate(model, cubePositions[i]);
                float angle = 20.0f * i;
                model = glm.rotate(model, glm.radians(angle), new vec3(1.0f, 0.3f, 0.5f));

                //传递给着色器
                shaderProgram.SetUniformMatrix4(GL, "model", model.to_array());

                //绘制
                GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);
            }

            //解绑vao
            vao.Unbind(GL);

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

            using (StreamReader sr = new StreamReader("7.2.camera.vs"))
            {
                vertexShaderSource = sr.ReadToEnd();
            }

            using (StreamReader sr = new StreamReader("7.2.camera.fs"))
            {
                fragmentShaderSource = sr.ReadToEnd();
            }

            //创建着色器
            shaderProgram.Create(GL, vertexShaderSource, fragmentShaderSource, null);

            //使用当前着色器
            GL.UseProgram(shaderProgram.ShaderProgramObject);

            //设置片元着色器中的texture1为0号纹理单元
            shaderProgram.SetUniform1(GL, "texture1", 0);

            //设置片元着色器中的texture2为1号纹理单元
            shaderProgram.SetUniform1(GL, "texture2", 1);

            //投影矩阵
            mat4 projection = glm.perspective(glm.radians(45.0f), (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);
            shaderProgram.SetUniformMatrix4(GL, "projection", projection.to_array());

            //创建vao
            vao.Create(GL);

            //绑定vao
            vao.Bind(GL);

            //创建vbo
            var vbo = new VertexBuffer();
            vbo.Create(GL);

            //绑定vbo
            vbo.Bind(GL);

            //绑定数据
            GL.BufferData(OpenGL.GL_ARRAY_BUFFER, vertices, OpenGL.GL_STATIC_DRAW);

            //配置顶点属性
            GL.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 5 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(0);

            //配置纹理坐标属性
            GL.VertexAttribPointer(1, 2, OpenGL.GL_FLOAT, false, 5 * sizeof(float), new IntPtr(3 * sizeof(float)));
            GL.EnableVertexAttribArray(1);

            //解绑vao
            vao.Unbind(GL);

            //设置窗体的大小
            Size = new Size(SCR_WIDTH, SCR_HEIGHT);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //根据按下的键，更新摄像机位置信息
            float cameraSpeed = 2.5f * deltaTime;
            if (keyData == Keys.W)
                cameraPos += cameraSpeed * cameraFront;
            if (keyData == Keys.S)
                cameraPos -= cameraSpeed * cameraFront;
            if (keyData == Keys.A)
                cameraPos -= glm.normalize(glm.cross(cameraFront, cameraUp)) * cameraSpeed;
            if (keyData == Keys.D)
                cameraPos += glm.normalize(glm.cross(cameraFront, cameraUp)) * cameraSpeed;

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}