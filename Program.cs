using System;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

class Program
{
    static Shader shader;
    static int vertexArrayObject;

    static int frameCount = 0;
static float elapsedTime = 0.0f;


    static float time = 0.0f;

    static void Main(string[] args)
    {
        var settings = new GameWindowSettings();
        var nativeSettings = new NativeWindowSettings()
        {
            Size = new Vector2i(800, 600),
            Title = "OpenGL Window"
        };

        using (var window = new GameWindow(settings, nativeSettings))
        {
            window.Load += () => 
            {
                GL.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);

                shader = new Shader("vertexShader.glsl", "fragmentShader.glsl");
                shader.Use();

                float[] vertices = {
                    // positions        // texture coords
                     1.0f,  1.0f, 0.0f,  1.0f, 1.0f,
                     1.0f, -1.0f, 0.0f,  1.0f, 0.0f,
                    -1.0f, -1.0f, 0.0f,  0.0f, 0.0f,
                    -1.0f,  1.0f, 0.0f,  0.0f, 1.0f
                };
                
                uint[] indices = {
                    0, 1, 3,
                    1, 2, 3
                };

                vertexArrayObject = GL.GenVertexArray();
                int vertexBufferObject = GL.GenBuffer();
                int elementBufferObject = GL.GenBuffer();

                GL.BindVertexArray(vertexArrayObject);

                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);
            };

            window.RenderFrame += (e) => 
{
    time += (float)e.Time;
    elapsedTime += (float)e.Time;
    frameCount++;

    if (elapsedTime >= 1.0f)
    {
        float fps = frameCount / elapsedTime;
        Console.WriteLine($"FPS: {fps}");
        frameCount = 0;
        elapsedTime = 0.0f;
    }

    GL.Clear(ClearBufferMask.ColorBufferBit);

    shader.Use();
    int iTimeLocation = shader.GetUniformLocation("iTime");
    GL.Uniform1(iTimeLocation, time);
    
    int iResolutionLocation = shader.GetUniformLocation("iResolution");
    GL.Uniform2(iResolutionLocation, new Vector2(window.Size.X, window.Size.Y));

    GL.BindVertexArray(vertexArrayObject);
    GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
    GL.BindVertexArray(0);

    window.SwapBuffers();
};


            window.Resize += (e) => 
            {
                GL.Viewport(0, 0, window.Size.X, window.Size.Y);
            };

            window.Run();
        }
    }
}
