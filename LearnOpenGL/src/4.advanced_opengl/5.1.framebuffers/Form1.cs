﻿using System;
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

namespace _5._1.framebuffers
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
        /// 平面
        /// </summary>
        private float[] quadVertices = {
        //位置         //纹理坐标
        -1.0f,  1.0f,  0.0f, 1.0f,
        -1.0f, -1.0f,  0.0f, 0.0f,
         1.0f, -1.0f,  1.0f, 0.0f,

        -1.0f,  1.0f,  0.0f, 1.0f,
         1.0f, -1.0f,  1.0f, 0.0f,
         1.0f,  1.0f,  1.0f, 1.0f
    };

        /// <summary>
        /// Shader
        /// </summary>
        private ShaderProgram shader = new ShaderProgram();

        /// <summary>
        /// 
        /// </summary>
        private ShaderProgram screenShader = new ShaderProgram();

        /// <summary>
        /// cubeVAO
        /// </summary>
        VertexBufferArray cubeVAO = new VertexBufferArray();

        /// <summary>
        /// planeVAO
        /// </summary>
        VertexBufferArray planeVAO = new VertexBufferArray();

        /// <summary>
        /// quadVAO
        /// </summary>
        VertexBufferArray quadVAO = new VertexBufferArray();

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

        /// <summary>
        /// 帧缓冲ID和颜色缓冲ID
        /// </summary>
        uint framebuffer, textureColorbuffer;

        public Form1()
        {
            InitializeComponent();

            openGLControl1.MouseWheel += OpenGLControl1_MouseWheel;
            openGLControl1.MouseMove += OpenGLControl1_MouseMove;

            //创建纹理
            cubeTexture.Create(GL, "marble.jpg");
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

            //绑定到帧缓冲
            GL.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, framebuffer);
            //启用深度测试
            GL.Enable(OpenGL.GL_DEPTH_TEST);

            //清除颜色和深度缓冲
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //使用着色器
            GL.UseProgram(shader.ShaderProgramObject);

            //设置投影和观察矩阵
            mat4 model = new mat4(1.0f);
            mat4 view = camera.GetViewMatrix();
            mat4 projection = glm.perspective(glm.radians(camera.Zoom), (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);
            shader.SetUniformMatrix4(GL,"view", view.to_array());
            shader.SetUniformMatrix4(GL,"projection", projection.to_array());

            //绑定立方体VAO
            cubeVAO.Bind(GL);
            //激活纹理单元0
            GL.ActiveTexture(OpenGL.GL_TEXTURE0);
            //绑定纹理
            cubeTexture.Bind(GL);
            //设置模型矩阵
            model = glm.translate(model, new vec3(-1.0f, 0.0f, -1.0f));
            shader.SetUniformMatrix4(GL,"model", model.to_array());
            //绘制第一个立方体
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);

            //设置模型矩阵
            model = new mat4(1.0f);
            model = glm.translate(model, new vec3(2.0f, 0.0f, 0.0f));
            shader.SetUniformMatrix4(GL, "model", model.to_array());
            //绘制第二个矩阵
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 36);

            //绑定平面VAO
            planeVAO.Bind(GL);
            //绑定纹理
            floorTexture.Bind(GL);
            //设置模型矩阵
            shader.SetUniformMatrix4(GL,"model", new mat4(1.0f).to_array());
            //绘制平面
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 6);

            //重新绑定到默认的帧缓冲中
            GL.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);

            //禁用深度测试
            GL.Disable(OpenGL.GL_DEPTH_TEST);

            //清除颜色缓冲
            GL.ClearColor(0.0f, 1.0f, 1.0f, 1.0f);
            GL.Clear(OpenGL.GL_COLOR_BUFFER_BIT);

            //使用screenShader
            GL.UseProgram(screenShader.ShaderProgramObject);
            //绑定vao
            quadVAO.Bind(GL);
            //绑定前面操作的颜色缓冲(纹理)
            GL.BindTexture(OpenGL.GL_TEXTURE_2D, textureColorbuffer);

            //绘制
            GL.DrawArrays(OpenGL.GL_TRIANGLES, 0, 6);

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
            shader.Create(GL,"5.1.framebuffers.vs", "5.1.framebuffers.fs");
            screenShader.Create(GL,"5.1.framebuffers_screen.vs", "5.1.framebuffers_screen.fs");

            //使用当前着色器
            GL.UseProgram(shader.ShaderProgramObject);
            //设置片元着色器中的texture1为0号纹理单元
            shader.SetUniform1(GL, "texture1", 0);

            GL.UseProgram(screenShader.ShaderProgramObject);
            screenShader.SetUniform1(GL, "screenTexture", 0);

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

            quadVAO.Create(GL);
            quadVAO.Bind(GL);
            vbo = new VertexBuffer();
            vbo.Create(GL);
            vbo.Bind(GL);
            //绑定数据
            GL.BufferData(OpenGL.GL_ARRAY_BUFFER, quadVertices, OpenGL.GL_STATIC_DRAW);
            //配置顶点属性
            GL.VertexAttribPointer(0, 2, OpenGL.GL_FLOAT, false, 4 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, OpenGL.GL_FLOAT, false, 4 * sizeof(float), new IntPtr(2 * sizeof(float)));
            GL.EnableVertexAttribArray(1);
            quadVAO.Unbind(GL);

            //创建帧缓冲
            uint[] ids = new uint[1];
            GL.GenFramebuffersEXT(1, ids);
            framebuffer = ids[0];
            //绑定帧缓冲
            GL.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, framebuffer);

            //生成纹理
            GL.GenTextures(1, ids);
            textureColorbuffer = ids[0];
            //绑定纹理
            GL.BindTexture(OpenGL.GL_TEXTURE_2D, textureColorbuffer);
            GL.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB, SCR_WIDTH, SCR_HEIGHT, 0, OpenGL.GL_RGB, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
            GL.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            GL.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            //将纹理作为帧缓冲的附件
            GL.FramebufferTexture2DEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_COLOR_ATTACHMENT0_EXT, OpenGL.GL_TEXTURE_2D, textureColorbuffer, 0);

            //生成渲染缓冲
            GL.GenRenderbuffersEXT(1, ids);
            uint rbo = ids[0];
            //绑定渲染缓冲
            GL.BindRenderbufferEXT(OpenGL.GL_RENDERBUFFER, rbo);
            //设置渲染缓冲
            GL.RenderbufferStorageEXT(OpenGL.GL_RENDERBUFFER, OpenGL.GL_DEPTH24_STENCIL8_EXT, SCR_WIDTH, SCR_HEIGHT);
            GL.FramebufferRenderbufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, OpenGL.GL_DEPTH_ATTACHMENT_EXT, OpenGL.GL_RENDERBUFFER, rbo);
            if (GL.CheckFramebufferStatusEXT(OpenGL.GL_FRAMEBUFFER_EXT) != OpenGL.GL_FRAMEBUFFER_COMPLETE_EXT)
            {
                //error
            }

            //重新绑定到默认的帧缓冲中
            GL.BindFramebufferEXT(OpenGL.GL_FRAMEBUFFER_EXT, 0);

            //GL.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);

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