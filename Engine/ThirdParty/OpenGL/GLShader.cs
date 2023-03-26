using System.Numerics;
using SBEngine;

namespace ThirdParty.OpenGL;

public unsafe class GLShader
{
    public string name;
    public uint vert, frag, geo;
    public uint program;

    public string? VertSrc, FragSrc, GeoSrc;

    public Dictionary<string, Uniform> Uniforms;
    public Dictionary<string, GLShaderAttribute> Attributes;
    public Dictionary<string, GLShaderVariable> Variables;
    
    OpenGL gl;

    private GLShader() { }

    public static GLShader CreateShaderFromFiles(OpenGL gl, ResourcePath vertPath, ResourcePath fragPath, string? debugName = null)
    {
        string vert_src = Utils.TryReadFile(vertPath.PathFromRoot);

        if (vert_src == null)
        {
            throw new Exception($"Could not read file: {vertPath}");
        }

        string frag_src = Utils.TryReadFile(fragPath.PathFromRoot);

        if (frag_src == null)
        {
            throw new Exception($"Could not read file: {vertPath}");
        }

        return CreateShaderFromSource(gl, vert_src, frag_src, null, debugName);
    }

    public static GLShader CreateShaderFromSource(OpenGL gl, string vertSrc, string fragSrc, string? geoSrc, string? name = null)
    {
        GLShader shader = new GLShader();

        shader.gl = gl;
        shader.VertSrc = vertSrc;
        shader.FragSrc = fragSrc;
        shader.GeoSrc = geoSrc;

        if (name == null)
        {
            name = $"Program {shader.program}";
        }

        shader.name = name;

        shader.Compile();

        gl.hfNameObject(GL.PROGRAM, shader.program, name);

        // chaching uniforms.

        shader.Uniforms = new Dictionary<string, Uniform>();
        int shader_uniform_count;
        gl.glGetProgramiv(shader.program, GL.ACTIVE_UNIFORMS, &shader_uniform_count);

        for (int i = 0; i < shader_uniform_count; i++)
        {
            var uniform = gl.hfGetUniformInfo(shader.program, (uint)i);

            if (uniform.array_size > 1)
            {
                uniform.name = uniform.name.Substring(0, uniform.name.Length - 3);
                uniform.location = gl.hfGetUniformLocation(shader.program, uniform.name + $"[0]");
            }
            else
            {
                uniform.location = gl.hfGetUniformLocation(shader.program, uniform.name);
            }

            shader.Uniforms.Add(uniform.name, uniform);
        }

        // chaching attributes.

        shader.Attributes = new Dictionary<string, GLShaderAttribute>();
        int shader_attribute_count;
        gl.glGetProgramiv(shader.program, GL.ACTIVE_ATTRIBUTES, &shader_attribute_count);

        for (int i = 0; i < shader_attribute_count; i++)
        {
            var attribute = gl.hfGetAttributeInfo(shader.program, (uint)i);

            if (attribute.array_size > 1)
            {
                attribute.name = attribute.name.Substring(0, attribute.name.Length - 3);
                attribute.location = gl.hfGetAttribLocation(shader.program, attribute.name + $"[0]");
            }
            else
            {
                attribute.location = gl.hfGetAttribLocation(shader.program, attribute.name);
            }

            shader.Attributes.Add(attribute.name, attribute);
        }
        
        shader.Variables = new Dictionary<string, GLShaderVariable>();
        
        return shader;
    }

    void Compile()
    {
        unsafe
        {
            program = gl.glCreateProgram();

            if (VertSrc != null)
            {
                gl.hfCreateShader(VertSrc, GL.VERTEX_SHADER, ref vert, name);
                gl.glAttachShader(program, vert);
            }

            if (FragSrc != null)
            {
                gl.hfCreateShader(FragSrc, GL.FRAGMENT_SHADER, ref frag, name);
                gl.glAttachShader(program, frag);
            }

            if (GeoSrc != null)
            {
                gl.hfCreateShader(GeoSrc, GL.GEOMETRY_SHADER, ref geo, name);
                gl.glAttachShader(program, geo);
            }

            gl.hfLinkProgram(program);

            if (name != null)
            {
                gl.hfNameObject(GL.PROGRAM, program, name);
            }
        }
    }

    public void Bind()
    {
        gl.glUseProgram(program);
    }

    public void Delete()
    {
        gl.glDeleteShader(program);
        program = 0;
    }

    public int GetAttibuteLocation(string name)
    {
        if (Attributes.TryGetValue(name, out GLShaderAttribute value))
        {
            return (int)value.location;
        }

        return -1;
    }

    public int GetUniform(string name)
    {
        if (Uniforms.TryGetValue(name, out Uniform value))
        {
            return (int)value.location;
        }

        return -1;
    }

    public int GetUniformArray(string name, int index)
    {
        if (Uniforms.TryGetValue(name, out Uniform value))
        {
            return value.location + index;
        }

        return -1;
    }

    public void SetVector3(string name, Vector3* Vector3)
    {
        int uniform = GetUniform(name);
        gl.glUniform3fv(uniform, 1, (float*)Vector3);
    }

    public void SetVector3(string name, Vector3 Vector3)
    {
        int uniform = GetUniform(name);
        gl.glUniform3fv(uniform, 1, (float*)&Vector3);
    }
    
    public void SetVector3(int uniform, Vector3 Vector3)
    {
        gl.glUniform3fv(uniform, 1, (float*)&Vector3);
    }

    public void SetVector4(string name, Vector4* Vector4)
    {
        int uniform = GetUniform(name);
        gl.glUniform4fv(uniform, 1, (float*)Vector4);
    }

    public void SetVector4(string name, Vector4 Vector4)
    {
        int uniform = GetUniform(name);
        gl.glUniform4fv(uniform, 1, (float*)&Vector4);
    }

    public void SetVector2(int uniform, Vector2 Vector2)
    {
        gl.glUniform2fv(uniform, 1, (float*)&Vector2);
    }
    
    public void SetVector2(string name, Vector2 Vector2)
    {
        int uniform = GetUniform(name);
        gl.glUniform2fv(uniform, 1, (float*)&Vector2);
    }

    public void SetVector1(int uniform, float vec1)
    {
        gl.glUniform1f(uniform, vec1);
    }

    public void SetVector1(string name, float vec1)
    {
        int uniform = GetUniform(name);
        gl.glUniform1f(uniform, vec1);
    }

    public void SetVector4(int uniform, Vector4 Vector4)
    {
        gl.glUniform4fv(uniform, 1, (float*)&Vector4);
    }

    public void SetMatrix4x4(int uniform, Matrix4x4 mat)
    {
        gl.glUniformMatrix4fv(uniform, 1, GL.FALSE, (float*)&mat);
    }
    
    public void SetMatrix4x4(string name, Matrix4x4 mat)
    {
        int uniform = GetUniform(name);
        gl.glUniformMatrix4fv(uniform, 1, GL.FALSE, (float*)&mat);
    }

    public void SetMatrix4x4(string name, Matrix4x4* mat)
    {
        int uniform = GetUniform(name);
        gl.glUniformMatrix4fv(uniform, 1, GL.FALSE, (float*)mat);
    }

    public void SetMatrix4x4(int uniform, Matrix4x4* mat)
    {
        gl.glUniformMatrix4fv(uniform, 1, GL.FALSE, (float*)mat);
    }

    public void SetTexture(string name, int texture)
    {
        int uniform = GetUniform(name);
        gl.glUniform1i(uniform, texture);
    }

    public void SetTexture(int uniform, int texture)
    {
        gl.glUniform1i(uniform, texture);
    }
    
    public void SetTexture2D(int uniform, int textureSlot, int textureIdentifier)
    { 
        GL glTextureSlot = GL.TEXTURE0;
        int temp = (int)GL.TEXTURE0 + textureSlot;
        Utils.AssignUnsafe(ref temp, ref glTextureSlot);
        
        gl.glActiveTexture(glTextureSlot);
        gl.glBindTexture(GL.TEXTURE_2D, (uint)textureIdentifier);
        gl.glUniform1i(uniform, textureSlot);
    }

    public void SetTextureArray(string name, int index, int textureSlot)
    {
        int uniform = GetUniformArray(name, index);
        gl.glUniform1i(uniform, textureSlot);
    }
}
