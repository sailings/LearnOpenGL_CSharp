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

namespace _5._1.transformations
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
                // 位置          // 纹理坐标
                0.5f,  0.5f, 0.0f,   1.0f, 1.0f,
                0.5f, -0.5f, 0.0f,   1.0f, 0.0f,
                -0.5f, -0.5f, 0.0f,   0.0f, 0.0f, 
                -0.5f,  0.5f, 0.0f,   0.0f, 1.0f
        };

        /// <summary>
        /// 三角形顶点顺序
        /// </summary>
        private uint[] indices = {
                0, 1, 3,  // 第一个三角形
                1, 2, 3   // 第二个三角形
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

        public Form1()
        {
            InitializeComponent();

            //创建纹理
            texture1.Create(GL, "container.jpg");

            //创建纹理
            texture2.Create(GL, "awesomeface.png");
        }

        /// <summary>
        /// 启动时间
        /// </summary>
        private DateTime startTime = DateTime.Now;        

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

            //坐标变换
            mat4 transform = new mat4(1.0f);

            //缩放
            transform = glm.scale(transform, new vec3(0.5f, 0.5f, 0.5f));

            //平移
            transform = glm.translate(transform, new vec3(0.5f, -0.5f, 0.0f));

            //旋转
            transform = glm.rotate(transform, (float)(DateTime.Now - startTime).TotalSeconds, new vec3(0.0f, 0.0f, 1.0f));

            //传递给顶点着色器
            shaderProgram.SetUniformMatrix4(GL, "transform", transform.to_array());

            //绘制
            GL.DrawElements(OpenGL.GL_TRIANGLES, indices.Length, indices);

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

            using (StreamReader sr = new StreamReader("5.1.transform.vs"))
            {
                vertexShaderSource = sr.ReadToEnd();
            }

            using (StreamReader sr = new StreamReader("5.1.transform.fs"))
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