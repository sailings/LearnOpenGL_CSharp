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

namespace _2.stencil_testing
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
        //位置                //纹理坐标
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
        /// 平面顶点
        /// </summary>
        private float[] planeVertices = {
        //位置                //纹理坐标
         5.0f, -0.5f,  5.0f,  2.0f, 0.0f,
        -5.0f, -0.5f,  5.0f,  0.0f, 0.0f,
        -5.0f, -0.5f, -5.0f,  0.0f, 2.0f,

         5.0f, -0.5f,  5.0f,  2.0f, 0.0f,
        -5.0f, -0.5f, -5.0f,  0.0f, 2.0f,
         5.0f, -0.5f, -5.0f,  2.0f, 2.0f
    };

        /// <summary>
        /// Shader
        /// </summary>
        private ShaderProgram shader = new ShaderProgram();

        /// <summary>
        /// shaderSingleColor
        /// </summary>
        private ShaderProgram shaderSingleColor = new ShaderProgram();

        /// <summary>
        /// cubeVAO
        /// </summary>
        VertexBufferArray cubeVAO = new VertexBufferArray();

        /// <summary>
        /// planeVAO
        /// </summary>
        VertexBufferArray planeVAO = new VertexBufferArray();

        /// <summary>
        /// cubeTexture
        /// </summary>
        private Texture cubeTexture = new Texture();

        /// <summary>
        /// floorTexture
        /// </summary>
        private Texture floorTexture = new Texture();

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

            //创建纹理
            cubeTexture.Create(GL, "marble.jpg");

            //创建纹理
            floorTexture.Create(GL, "metal.png");
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
            GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            GL.UseProgram(shaderSingleColor.ShaderProgramObject);
            mat4 model = new mat4(1.0f);
            mat4 view = camera.GetViewMatrix();
            mat4 projection = glm.perspective(glm.radians(camera.Zoom), (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);
            shaderSingleColor.SetUniformMatrix4(GL,"view", view.to_array());
            shaderSingleColor.SetUniformMatrix4(GL,"projection", projection.to_array());

            GL.UseProgram(shader.ShaderProgramObject);
            shader.SetUniformMatrix4(GL, "view", view.to_array());
            shader.SetUniformMatrix4(GL, "projection", projection.to_array());

            // draw floor as normal, but don't write the floor to the stencil buffer, we only care about the containers. We set its mask to 0x00 to not write to the stencil buffer.
            GL.StencilMask(0x00);

            // floor
            planeVAO.Bind(GL);
            floorTexture.Bind(GL);
            shader.SetUniformMatrix4(GL,"model", new mat4(1.0f).to_array());
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 6);
            planeVAO.Unbind(GL);

            // 1st. render pass, draw objects as normal, writing to the stencil buffer
            // --------------------------------------------------------------------
            GL.StencilFunc(OpenGL.GL_ALWAYS, 1, 0xFF);
            GL.StencilMask(0xFF);

            // cubes
            cubeVAO.Bind(GL);
            //glBindVertexArray(cubeVAO);
            GL.ActiveTexture(OpenGL.GL_TEXTURE0);
            cubeTexture.Bind(GL);
            //glBindTexture(GL_TEXTURE_2D, cubeTexture);
            model = glm.translate(model, new vec3(-1.0f, 0.0f, -1.0f));
            shader.SetUniformMatrix4(GL,"model", model.to_array());
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);
            model = new mat4(1.0f);
            model = glm.translate(model, new vec3(2.0f, 0.0f, 0.0f));
            shader.SetUniformMatrix4(GL,"model", model.to_array());
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);

            GL.StencilFunc(OpenGL.GL_NOTEQUAL, 1, 0xFF);
            GL.StencilMask(0x00);
            GL.Disable(OpenGL.GL_DEPTH_TEST);
            shaderSingleColor.Bind(GL);
            float scale = 1.1f;
            // cubes
            cubeVAO.Bind(GL);
            cubeTexture.Bind(GL);
            model = new mat4(1.0f);
            model = glm.translate(model, new vec3(-1.0f, 0.0f, -1.0f));
            model = glm.scale(model, new vec3(scale, scale, scale));
            shaderSingleColor.SetUniformMatrix4(GL,"model", model.to_array());
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);
            model = new mat4(1.0f);
            model = glm.translate(model, new vec3(2.0f, 0.0f, 0.0f));
            model = glm.scale(model, new vec3(scale, scale, scale));
            shaderSingleColor.SetUniformMatrix4(GL,"model", model.to_array());
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);
            cubeVAO.Unbind(GL);
            GL.StencilMask(0xFF);
            GL.Enable(OpenGL.GL_DEPTH_TEST);

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
            //配置深度测试函数
            GL.DepthFunc(SharpGL.Enumerations.DepthFunction.Less);
            GL.Enable(OpenGL.GL_STENCIL_TEST);
            GL.StencilFunc(OpenGL.GL_NOTEQUAL, 1, 0xFF);
            GL.StencilOp(OpenGL.GL_KEEP, OpenGL.GL_KEEP, OpenGL.GL_REPLACE);

            //创建着色器
            //shader.Create(GL, "1.2.depth_testing.vs", "1.2.depth_testing.fs");
            shader.Create(GL, "2.stencil_testing.vs", "2.stencil_testing.fs");
            shaderSingleColor.Create(GL, "2.stencil_testing.vs", "2.stencil_single_color.fs");

            //使用当前着色器
            GL.UseProgram(shader.ShaderProgramObject);

            //设置片元着色器中的texture1为0号纹理单元
            shader.SetUniform1(GL, "texture1", 0);

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
            GL.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 5 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, OpenGL.GL_FLOAT, false, 5 * sizeof(float), new IntPtr(3 * sizeof(float)));
            GL.EnableVertexAttribArray(1);
            cubeVAO.Unbind(GL);

            //创建平面VAO
            planeVAO.Create(GL);
            planeVAO.Bind(GL);
            //重新设置vbo数据
            vbo = new VertexBuffer();
            vbo.Create(GL);
            vbo.Bind(GL);
            //绑定数据
            GL.BufferData(OpenGL.GL_ARRAY_BUFFER, planeVertices, OpenGL.GL_STATIC_DRAW);
            //配置顶点属性
            GL.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 5 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, OpenGL.GL_FLOAT, false, 5 * sizeof(float), new IntPtr(3 * sizeof(float)));
            GL.EnableVertexAttribArray(1);
            planeVAO.Unbind(GL);

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