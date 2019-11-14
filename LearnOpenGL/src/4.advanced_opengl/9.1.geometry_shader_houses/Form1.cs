using GlmNet;
using SharpGL;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace _9._1.geometry_shader_houses
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

        private float[] points = {
            -0.5f,  0.5f, 1.0f, 0.0f, 0.0f,
            0.5f,  0.5f, 0.0f, 1.0f, 0.0f,
            0.5f, -0.5f, 0.0f, 0.0f, 1.0f,
            -0.5f, -0.5f, 1.0f, 1.0f, 0.0f
        };

        /// <summary>
        /// vao
        /// </summary>
        VertexBufferArray vao = new VertexBufferArray();

        private ShaderProgram shader = new ShaderProgram();

        private DateTime startTime = DateTime.Now;

        //摄像机对象
        Camera camera = new Camera(new vec3(0.0f, 0.0f, 3.0f), new vec3(0.0f, 1.0f, 0.0f));

        float lastX = SCR_WIDTH / 2.0f;
        float lastY = SCR_HEIGHT / 2.0f;
        bool firstMouse = true;

        float deltaTime = 0.0f;
        float lastFrame = 0.0f;

        public Form1()
        {
            InitializeComponent();
            openGLControl1.MouseWheel += OpenGLControl1_MouseWheel;
            openGLControl1.MouseMove += OpenGLControl1_MouseMove;
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

            //使用shader
            GL.UseProgram(shader.ShaderProgramObject);

            //绑定vao
            vao.Bind(GL);

            //绘制
            GL.DrawArrays(OpenGL.GL_POINTS, 0, 4);
            
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
            shader.Create(GL, "9.1.geometry_shader.vs", "9.1.geometry_shader.fs", "9.1.geometry_shader.gs");

            vao.Create(GL);
            vao.Bind(GL);
            //创建VBO
            var vbo = new VertexBuffer();
            vbo.Create(GL);
            vbo.Bind(GL);
            //绑定数据
            GL.BufferData(OpenGL.GL_ARRAY_BUFFER, points, OpenGL.GL_STATIC_DRAW);
            //配置顶点属性
            GL.VertexAttribPointer(0, 2, OpenGL.GL_FLOAT, false, 5 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, OpenGL.GL_FLOAT, false, 5 * sizeof(float), new IntPtr(2 * sizeof(float)));
            GL.EnableVertexAttribArray(1);
            vao.Unbind(GL);

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