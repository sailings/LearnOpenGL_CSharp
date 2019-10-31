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

namespace _6._2.coordinate_systems_depth
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

        public Form1()
        {
            InitializeComponent();

            //创建纹理
            texture1.Create(GL, "container.jpg");

            //创建纹理
            texture2.Create(GL, "awesomeface.png");
        }

        /// <summary>
        /// OpenGL绘制事件内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OpenGLControl1_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {
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

            //以下分别为model,view,projection矩阵
            mat4 model = new mat4(1.0f);
            mat4 view = new mat4(1.0f);
            mat4 projection = new mat4(1.0f);

            //旋转model矩阵
            model = glm.rotate(model, (float)(DateTime.Now - startTime).TotalSeconds, new vec3(0.5f, 1.0f, 0.0f));
            //平移view矩阵
            view = glm.translate(view, new vec3(0.0f, 0.0f, -3.0f));
            //生成投影矩阵
            projection = glm.perspective(glm.radians(45.0f), (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);

            //传递给顶点着色器
            shaderProgram.SetUniformMatrix4(GL, "model", model.to_array());
            shaderProgram.SetUniformMatrix4(GL, "view", view.to_array());
            shaderProgram.SetUniformMatrix4(GL, "projection", projection.to_array());

            //绘制物体
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);

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

            using (StreamReader sr = new StreamReader("6.2.coordinate_systems.vs"))
            {
                vertexShaderSource = sr.ReadToEnd();
            }

            using (StreamReader sr = new StreamReader("6.2.coordinate_systems.fs"))
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
    }
}