using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _1._2.depth_testing_view
{
    /// <summary>
    /// 摄像机移动方向
    /// </summary>
    public enum Camera_Movement
    {
        /// <summary>
        /// 前
        /// </summary>
        FORWARD,

        /// <summary>
        /// 后
        /// </summary>
        BACKWARD,

        /// <summary>
        /// 左
        /// </summary>
        LEFT,

        /// <summary>
        /// 右
        /// </summary>
        RIGHT
    };

    /// <summary>
    /// 摄像机
    /// </summary>
    public class Camera
    {
        const float YAW = -90.0f;
        const float PITCH = 0.0f;
        const float SPEED = 2.5f;
        const float SENSITIVITY = 0.1f;
        const float ZOOM = 45.0f;

        vec3 Position = new vec3(0.0f, 0.0f, 0.0f);
        vec3 Front = new vec3(0.0f, 0.0f, -1.0f);
        vec3 Up = new vec3(0.0f, 1.0f,0.0f);
        vec3 Right;
        vec3 WorldUp;

        float Yaw;
        float Pitch;
        float MovementSpeed = SPEED;
        float MouseSensitivity = SENSITIVITY;
        public float Zoom { get; set; } = ZOOM;

        public Camera(vec3 position, vec3 up, float yaw = YAW, float pitch = PITCH)
        {
            Position = position;
            WorldUp = up;
            Yaw = yaw;
            Pitch = pitch;
            UpdateCameraVectors();
        }

        public Camera(float posX, float posY, float posZ, float upX, float upY, float upZ, float yaw, float pitch)
        {
            Position = new vec3(posX, posY, posZ);
            WorldUp = new vec3(upX, upY, upZ);
            Yaw = yaw;
            Pitch = pitch;
            UpdateCameraVectors();
        }

        /// <summary>
        /// 获取观察矩阵
        /// </summary>
        /// <returns></returns>
        public mat4 GetViewMatrix()
        {
            return glm.lookAt(Position, Position + Front, Up);
        }

        /// <summary>
        /// 处理摄像机移动
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="deltaTime"></param>
        public void ProcessKeyboard(Camera_Movement direction, float deltaTime)
        {
            float velocity = MovementSpeed * deltaTime;
            if (direction == Camera_Movement.FORWARD)
                Position += Front * velocity;
            if (direction == Camera_Movement.BACKWARD)
                Position -= Front * velocity;
            if (direction == Camera_Movement.LEFT)
                Position -= Right * velocity;
            if (direction == Camera_Movement.RIGHT)
                Position += Right * velocity;
        }

        /// <summary>
        /// 处理鼠标移动
        /// </summary>
        /// <param name="xoffset"></param>
        /// <param name="yoffset"></param>
        /// <param name="constrainPitch"></param>
        public void ProcessMouseMovement(float xoffset, float yoffset, bool constrainPitch = true)
        {
            xoffset *= MouseSensitivity;
            yoffset *= MouseSensitivity;

            Yaw += xoffset;
            Pitch += yoffset;

            if (constrainPitch)
            {
                if (Pitch > 89.0f)
                    Pitch = 89.0f;
                if (Pitch < -89.0f)
                    Pitch = -89.0f;
            }

            UpdateCameraVectors();
        }

        /// <summary>
        /// 处理缩放
        /// </summary>
        /// <param name="yoffset"></param>
        public void ProcessMouseScroll(float yoffset)
        {
            if (Zoom >= 1.0f && Zoom <= 90.0f)
                Zoom -= yoffset;
            if (Zoom <= 1.0f)
                Zoom = 1.0f;
            if (Zoom >= 90.0f)
                Zoom = 90.0f;
        }

        /// <summary>
        /// 更新摄像机状态
        /// </summary>
        void UpdateCameraVectors()
        {
            vec3 front = new vec3();
            front.x = glm.cos(glm.radians(Yaw)) * glm.cos(glm.radians(Pitch));
            front.y = glm.sin(glm.radians(Pitch));
            front.z = glm.sin(glm.radians(Yaw)) * glm.cos(glm.radians(Pitch));
            Front = glm.normalize(front);
            Right = glm.normalize(glm.cross(Front, WorldUp));
            Up = glm.normalize(glm.cross(Right, Front));
        }
    }
}