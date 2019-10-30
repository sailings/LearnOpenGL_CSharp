using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SharpGL;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;

namespace _2._3.hello_triangle_exercise1
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
        /// 顶点着色器内容
        /// </summary>
        private string vertexShaderSource = "#version 330 core\n" +
                                    "layout (location = 0) in vec3 aPos;\n" +
                                    "void main()\n" +
                                    "{\n" +
                                    "   gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);\n" +
                                    "}\0";

        /// <summary>
        /// 片元着色器内容
        /// </summary>
        private string fragmentShaderSource = "#version 330 core\n" +
                                              "out vec4 FragColor;\n" +
                                              "void main()\n" +
                                              "{\n" +
                                              "   FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);\n" +
                                              "}\n\0";

        /// <summary>
        /// 顶点
        /// </summary>
        private float[] vertices = {
                 // 第一个三角形
                -0.9f, -0.5f, 0.0f,  // 左
                -0.0f, -0.5f, 0.0f,  // 右
                -0.45f, 0.5f, 0.0f,  // 上
                // 第二个三角形
                0.0f, -0.5f, 0.0f,  // 左
                0.9f, -0.5f, 0.0f,  // 右
                0.45f, 0.5f, 0.0f   // 上
        };

        /// <summary>
        /// Shader
        /// </summary>
        private ShaderProgram shaderProgram = new ShaderProgram();

        /// <summary>
        /// vao
        /// </summary>
        VertexBufferArray vao = new VertexBufferArray();

        public Form1()
        {
            InitializeComponent();
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

            //绑定vao
            vao.Bind(GL);

            //绘制
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 6);

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

            //创建着色器
            shaderProgram.Create(GL, vertexShaderSource, fragmentShaderSource, null);

            //创建vao
            vao.Create(GL);

            //绑定vao
            vao.Bind(GL);

            //创建vbo
            var vbo = new VertexBuffer();
            vbo.Create(GL);

            //绑定vbo
            vbo.Bind(GL);

            //设置vbo数据
            vbo.SetData(GL, 0, vertices, false, 3);

            //解绑vbo
            vbo.Unbind(GL);            

            //解绑vao
            vao.Unbind(GL);

            //设置窗体的大小
            Size = new Size(SCR_WIDTH, SCR_HEIGHT);
        }
    }
}