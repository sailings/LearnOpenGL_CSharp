using GlmNet;
using SharpGL;
using SharpGL.Shaders;
using SharpGL.VertexBuffers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace _8.advanced_glsl_ubo
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
        -0.5f, -0.5f, -0.5f,
         0.5f, -0.5f, -0.5f,
         0.5f,  0.5f, -0.5f,
         0.5f,  0.5f, -0.5f,
        -0.5f,  0.5f, -0.5f,
        -0.5f, -0.5f, -0.5f,

        -0.5f, -0.5f,  0.5f,
         0.5f, -0.5f,  0.5f,
         0.5f,  0.5f,  0.5f,
         0.5f,  0.5f,  0.5f,
        -0.5f,  0.5f,  0.5f,
        -0.5f, -0.5f,  0.5f,

        -0.5f,  0.5f,  0.5f,
        -0.5f,  0.5f, -0.5f,
        -0.5f, -0.5f, -0.5f,
        -0.5f, -0.5f, -0.5f,
        -0.5f, -0.5f,  0.5f,
        -0.5f,  0.5f,  0.5f,

         0.5f,  0.5f,  0.5f,
         0.5f,  0.5f, -0.5f,
         0.5f, -0.5f, -0.5f,
         0.5f, -0.5f, -0.5f,
         0.5f, -0.5f,  0.5f,
         0.5f,  0.5f,  0.5f,

        -0.5f, -0.5f, -0.5f,
         0.5f, -0.5f, -0.5f,
         0.5f, -0.5f,  0.5f,
         0.5f, -0.5f,  0.5f,
        -0.5f, -0.5f,  0.5f,
        -0.5f, -0.5f, -0.5f,

        -0.5f,  0.5f, -0.5f,
         0.5f,  0.5f, -0.5f,
         0.5f,  0.5f,  0.5f,
         0.5f,  0.5f,  0.5f,
        -0.5f,  0.5f,  0.5f,
        -0.5f,  0.5f, -0.5f,
    };

        private ShaderProgram shaderRed = new ShaderProgram();
        private ShaderProgram shaderGreen = new ShaderProgram();
        private ShaderProgram shaderBlue = new ShaderProgram();
        private ShaderProgram shaderYellow = new ShaderProgram();

        /// <summary>
        /// cubeVAO
        /// </summary>
        VertexBufferArray cubeVAO = new VertexBufferArray();

        private DateTime startTime = DateTime.Now;

        //摄像机对象
        Camera camera = new Camera(new vec3(0.0f, 0.0f, 3.0f), new vec3(0.0f, 1.0f, 0.0f));

        float lastX = SCR_WIDTH / 2.0f;
        float lastY = SCR_HEIGHT / 2.0f;
        bool firstMouse = true;

        float deltaTime = 0.0f;
        float lastFrame = 0.0f;

        uint[] uboMatrices = new uint[1];
        mat4 tempMat = new mat4(1);
        int mat4Length = 0;

        public Form1()
        {
            //4x4矩阵占用字节数
            mat4Length = tempMat.to_array().Length * sizeof(float);
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

            mat4 view = camera.GetViewMatrix();
            //绑定缓冲对象
            GL.BindBuffer(OpenGL.GL_UNIFORM_BUFFER, uboMatrices[0]);
            GL.BufferSubData(OpenGL.GL_UNIFORM_BUFFER, mat4Length, mat4Length, view.to_array());
            GL.BindBuffer(OpenGL.GL_UNIFORM_BUFFER, 0);

            //绑定VAO
            cubeVAO.Bind(GL);
            GL.UseProgram(shaderRed.ShaderProgramObject);

            //绘制4个立方体
            mat4 model = new mat4(1.0f);
            model = glm.translate(model, new vec3(-0.75f, 0.75f, 0.0f)); // move top-left
            shaderRed.SetUniformMatrix4(GL, "model", model.to_array());
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);
            GL.UseProgram(shaderGreen.ShaderProgramObject);
            model = new mat4(1.0f);
            model = glm.translate(model, new vec3(0.75f, 0.75f, 0.0f)); // move top-right
            shaderRed.SetUniformMatrix4(GL, "model", model.to_array());
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);
            GL.UseProgram(shaderYellow.ShaderProgramObject);
            model = new mat4(1.0f);
            model = glm.translate(model, new vec3(-0.75f, -0.75f, 0.0f)); // move bottom-left
            shaderRed.SetUniformMatrix4(GL, "model", model.to_array());
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);
            GL.UseProgram(shaderBlue.ShaderProgramObject);
            model = new mat4(1.0f);
            model = glm.translate(model, new vec3(0.75f, -0.75f, 0.0f)); // move bottom-right
            shaderRed.SetUniformMatrix4(GL, "model", model.to_array());
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

            //创建着色器
            shaderRed.Create(GL, "8.advanced_glsl.vs", "8.red.fs");
            shaderGreen.Create(GL, "8.advanced_glsl.vs", "8.green.fs");
            shaderBlue.Create(GL, "8.advanced_glsl.vs", "8.blue.fs");
            shaderYellow.Create(GL, "8.advanced_glsl.vs", "8.yellow.fs");

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
            GL.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 3 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(0);
            cubeVAO.Unbind(GL);

            //获取 uniform block索引
            uint uniformBlockIndexRed = GL.GetUniformBlockIndex(shaderRed.ShaderProgramObject, "Matrices");
            uint uniformBlockIndexGreen = GL.GetUniformBlockIndex(shaderGreen.ShaderProgramObject, "Matrices");
            uint uniformBlockIndexBlue = GL.GetUniformBlockIndex(shaderBlue.ShaderProgramObject, "Matrices");
            uint uniformBlockIndexYellow = GL.GetUniformBlockIndex(shaderYellow.ShaderProgramObject, "Matrices");

            //设置索引
            GL.UniformBlockBinding(shaderRed.ShaderProgramObject, uniformBlockIndexRed, 0);
            GL.UniformBlockBinding(shaderGreen.ShaderProgramObject, uniformBlockIndexGreen, 0);
            GL.UniformBlockBinding(shaderBlue.ShaderProgramObject, uniformBlockIndexBlue, 0);
            GL.UniformBlockBinding(shaderYellow.ShaderProgramObject, uniformBlockIndexYellow, 0);

            //生成缓冲对象
            GL.GenBuffers(1, uboMatrices);
            GL.BindBuffer(OpenGL.GL_UNIFORM_BUFFER, uboMatrices[0]);
            //初始化数据为0
            GL.BufferData(OpenGL.GL_UNIFORM_BUFFER, 2 * mat4Length, IntPtr.Zero, OpenGL.GL_STATIC_DRAW);
            //解绑
            GL.BindBuffer(OpenGL.GL_UNIFORM_BUFFER, 0);
            GL.BindBufferRange(OpenGL.GL_UNIFORM_BUFFER, 0, uboMatrices[0], 0, 2 * mat4Length);
            mat4 projection = glm.perspective(45.0f, (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);
            GL.BindBuffer(OpenGL.GL_UNIFORM_BUFFER, uboMatrices[0]);
            GL.BufferSubData(OpenGL.GL_UNIFORM_BUFFER, 0, mat4Length, projection.to_array());
            GL.BindBuffer(OpenGL.GL_UNIFORM_BUFFER, 0);

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