using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL;

public class Shader
{
    public int Handle;
    private readonly Dictionary<string, int> _uniformLocations;

    public Shader(string vertexPath, string fragmentPath)
    {
        string vertexShaderSource = File.ReadAllText(vertexPath);
        string fragmentShaderSource = File.ReadAllText(fragmentPath);

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        CheckShaderCompile(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        CheckShaderCompile(fragmentShader);

        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);
        GL.LinkProgram(Handle);
        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

        _uniformLocations = new Dictionary<string, int>();
        for (var i = 0; i < numberOfUniforms; i++)
        {
            var key = GL.GetActiveUniform(Handle, i, out _, out _);
            var location = GL.GetUniformLocation(Handle, key);
            _uniformLocations.Add(key, location);
        }
    }

    public void Use() => GL.UseProgram(Handle);

    private static void CheckShaderCompile(int shader)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Shader compilation failed: {infoLog}");
        }
    }

    public int GetUniformLocation(string name)
    {
        if (_uniformLocations.ContainsKey(name))
        {
            return _uniformLocations[name];
        }
        else
        {
            int location = GL.GetUniformLocation(Handle, name);
            _uniformLocations.Add(name, location);
            return location;
        }
    }

    public void SetInt(string name, int data)
    {
        GL.UseProgram(Handle);
        GL.Uniform1(GetUniformLocation(name), data);
    }

    public void SetFloat(string name, float data)
    {
        GL.UseProgram(Handle);
        GL.Uniform1(GetUniformLocation(name), data);
    }
}
