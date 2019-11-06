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

namespace _3._2.blending_sort
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

        private float[] transparentVertices = {
        //位置               //纹理坐标
        0.0f,  0.5f,  0.0f,  0.0f,  0.0f,
        0.0f, -0.5f,  0.0f,  0.0f,  1.0f,
        1.0f, -0.5f,  0.0f,  1.0f,  1.0f,

        0.0f,  0.5f,  0.0f,  0.0f,  0.0f,
        1.0f, -0.5f,  0.0f,  1.0f,  1.0f,
        1.0f,  0.5f,  0.0f,  1.0f,  0.0f
    };

        /// <summary>
        /// 窗口的位置信息
        /// </summary>
        List<vec3> windows = new List<vec3>
        {
            new vec3(-1.5f, 0.0f, -0.48f),
            new vec3( 1.5f, 0.0f, 0.51f),
            new vec3( 0.0f, 0.0f, 0.7f),
            new vec3(-0.3f, 0.0f, -2.3f),
            new vec3( 0.5f, 0.0f, -0.6f)
        };

        /// <summary>
        /// Shader
        /// </summary>
        private ShaderProgram shader = new ShaderProgram();

        /// <summary>
        /// cubeVAO
        /// </summary>
        VertexBufferArray cubeVAO = new VertexBufferArray();

        /// <summary>
        /// planeVAO
        /// </summary>
        VertexBufferArray planeVAO = new VertexBufferArray();

        /// <summary>
        /// 草VAO
        /// </summary>
        VertexBufferArray transparentVAO = new VertexBufferArray();

        /// <summary>
        /// cubeTexture
        /// </summary>
        private Texture cubeTexture = new Texture();

        /// <summary>
        /// floorTexture
        /// </summary>
        private Texture floorTexture = new Texture();

        /// <summary>
        /// transparentTexture
        /// </summary>
        private Texture transparentTexture = new Texture();

        private DateTime startTime = DateTime.Now;

        /// <summary>
        /// 排序后的列表
        /// </summary>
        List<KeyValuePair<float, vec3>> sorted = new List<KeyValuePair<float, vec3>>();

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
            floorTexture.Create(GL, "metal.png");
            transparentTexture.Create(GL, "window.png",false);
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

            //清除颜色缓冲，深度缓冲，模板缓冲
            GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            //使用当前着色器
            GL.UseProgram(shader.ShaderProgramObject);

            //投影矩阵和观察矩阵
            mat4 projection = glm.perspective(glm.radians(camera.Zoom), (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);
            mat4 view = camera.GetViewMatrix();
            mat4 model = new mat4(1.0f);
            shader.SetUniformMatrix4(GL, "projection", projection.to_array());
            shader.SetUniformMatrix4(GL, "view", view.to_array());

            //立方体
            cubeVAO.Bind(GL);
            GL.ActiveTexture(OpenGL.GL_TEXTURE0);
            cubeTexture.Bind(GL);
            model = glm.translate(model, new vec3(-1.0f, 0.0f, -1.0f));
            shader.SetUniformMatrix4(GL, "model", model.to_array());
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);
            model = new mat4(1.0f);
            model = glm.translate(model, new vec3(2.0f, 0.0f, 0.0f));
            shader.SetUniformMatrix4(GL,"model", model.to_array());
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);

            //平面
            planeVAO.Bind(GL);
            floorTexture.Bind(GL);
            model = new mat4(1.0f);
            shader.SetUniformMatrix4(GL,"model", model.to_array());
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 6);

            //窗口
            transparentVAO.Bind(GL);
            transparentTexture.Bind(GL);
            for (int i = 0; i < sorted.Count; i++)
            {
                model = new mat4(1.0f);
                model = glm.translate(model, sorted[i].Value);
                shader.SetUniformMatrix4(GL,"model", model.to_array());
                GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 6);
            }
            
            //设置标题，显示FPS
            Text = title + $"-FPS[{openGLControl1.FPS}]";
        }

        /// <summary>
        /// 计算向量的模
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private float Vec3Length(vec3 v)
        {
            return (float)Math.Pow(Math.Pow(v.x, 2) + Math.Pow(v.y, 2) + Math.Pow(v.z, 2), 0.5f);
        }

        /// <summary>
        /// OpenGL初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenGLControl1_OpenGLInitialized(object sender, EventArgs e)
        {
            //计算每个窗口位置到摄像机的距离
            SortedDictionary<float, vec3> sdic = new SortedDictionary<float, vec3>();
            for (int i = 0; i < windows.Count; i++)
            {
                float distance = Vec3Length(camera.Position - windows[i]);
                sdic.Add(distance,windows[i]);
            }
            //根据从大到小排序
            sorted = sdic.OrderByDescending(x => x.Key).ToList();

            //获取OpenGL对象
            GL = openGLControl1.OpenGL;

            //启用深度测试
            GL.Enable(OpenGL.GL_DEPTH_TEST);
            //开启混合
            GL.Enable(OpenGL.GL_BLEND);
            //配置混合函数
            GL.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            //创建着色器
            shader.Create(GL, "3.2.blending.vs", "3.2.blending.fs");

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

            //创建窗口VAO
            transparentVAO.Create(GL);
            transparentVAO.Bind(GL);
            vbo = new VertexBuffer();
            vbo.Create(GL);
            vbo.Bind(GL);
            //绑定数据
            GL.BufferData(OpenGL.GL_ARRAY_BUFFER, transparentVertices, OpenGL.GL_STATIC_DRAW);
            //配置顶点属性
            GL.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 5 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, OpenGL.GL_FLOAT, false, 5 * sizeof(float), new IntPtr(3 * sizeof(float)));
            GL.EnableVertexAttribArray(1);
            transparentVAO.Unbind(GL);

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