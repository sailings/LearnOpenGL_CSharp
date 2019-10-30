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

namespace _2._4.hello_triangle_exercise2
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
        /// 第一个三角形
        /// </summary>
        private float[] firstTriangle = {
                -0.9f, -0.5f, 0.0f,  // 左
                -0.0f, -0.5f, 0.0f,  // 右
                -0.45f, 0.5f, 0.0f,  // 上
        };

        /// <summary>
        /// 第二个三角形
        /// </summary>
        private float[] secondTriangle = {
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
        VertexBufferArray[] vao = new VertexBufferArray[2] { new VertexBufferArray(), new VertexBufferArray() };

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

            //绑定vao0
            vao[0].Bind(GL);

            //绘制
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);

            //解绑vao0
            vao[0].Unbind(GL);

            //绑定vao1
            vao[1].Bind(GL);

            //绘制
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 3);

            //解绑vao1
            vao[1].Unbind(GL);

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

            #region vao0

            //创建vao0
            vao[0].Create(GL);

            //绑定vao0
            vao[0].Bind(GL);

            //创建vbo0
            var vbo0 = new VertexBuffer();
            vbo0.Create(GL);

            //绑定vbo0
            vbo0.Bind(GL);

            //设置vbo0数据
            vbo0.SetData(GL, 0, firstTriangle, false, 3);

            //解绑vbo0
            vbo0.Unbind(GL);

            //解绑vao0
            vao[0].Unbind(GL);

            #endregion

            #region vao1

            //创建vao1
            vao[1].Create(GL);

            //绑定vao1
            vao[1].Bind(GL);

            //创建vbo1
            var vbo1 = new VertexBuffer();
            vbo1.Create(GL);

            //绑定vbo1
            vbo1.Bind(GL);

            //设置vbo1数据
            vbo1.SetData(GL, 0, secondTriangle, false, 3);

            //解绑vbo1
            vbo1.Unbind(GL);

            //解绑vao1
            vao[1].Unbind(GL);

            #endregion

            //设置窗体的大小
            Size = new Size(SCR_WIDTH, SCR_HEIGHT);
        }
    }
}