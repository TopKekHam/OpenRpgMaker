using System.Numerics;
using static OpenGL;

/*
    HOW TO USE:
    use Shader.CreateShader##### to create the shader
    call the fuctions using the shader instance.
*/

public unsafe class GLShader
{
    public string name;
    public uint vert, frag, geo;
    public uint program;

    public string? vert_src, frag_src, geo_src;

    public Dictionary<string, Uniform> uniforms;
    public Dictionary<string, Attribute> attributes;

    private GLShader() { }

    public static GLShader CreateShaderFromFiles(string vert_path, string frag_path, string? _debug_name = null)
    {
        var vert_src = Utils.ReadFile(vert_path);

        if (vert_src)
        {
            throw new Exception($"Could not read file: {vert_path}");
        }

        var frag_src = Utils.ReadFile(frag_path);

        if (frag_src)
        {
            throw new Exception($"Could not read file: {vert_path}");
        }

        return CreateShaderFromSource(vert_src.Result, frag_src.Result, null, _debug_name);
    }

    public static GLShader CreateShaderFromSource(string _vert_src, string _frag_src, string? _geo_src, string? name = null)
    {
        GLShader shader = new GLShader();

        shader.vert_src = _vert_src;
        shader.frag_src = _frag_src;
        shader.geo_src = _geo_src;

        if (name == null)
        {
            name = $"Program {shader.program}";
        }

        shader.name = name;

        shader.Compile();

        hfNameObject(GL.PROGRAM, shader.program, name);

        // chaching uniforms.

        shader.uniforms = new Dictionary<string, Uniform>();
        int shader_uniform_count;
        glGetProgramiv(shader.program, GL.ACTIVE_UNIFORMS, &shader_uniform_count);

        for (int i = 0; i < shader_uniform_count; i++)
        {
            var uniform = hfGetUniformInfo(shader.program, (uint)i);

            if (uniform.array_size > 1)
            {
                uniform.name = uniform.name.Substring(0, uniform.name.Length - 3);
                uniform.location = hfGetUniformLocation(shader.program, uniform.name + $"[0]");
            }
            else
            {
                uniform.location = hfGetUniformLocation(shader.program, uniform.name);
            }

            shader.uniforms.Add(uniform.name, uniform);
        }

        // chaching attributes.

        shader.attributes = new Dictionary<string, Attribute>();
        int shader_attribute_count;
        glGetProgramiv(shader.program, GL.ACTIVE_ATTRIBUTES, &shader_attribute_count);

        for (int i = 0; i < shader_attribute_count; i++)
        {
            var attribute = hfGetAttributeInfo(shader.program, (uint)i);

            if (attribute.array_size > 1)
            {
                attribute.name = attribute.name.Substring(0, attribute.name.Length - 3);
                attribute.location = hfGetAttribLocation(shader.program, attribute.name + $"[0]");
            }
            else
            {
                attribute.location = hfGetAttribLocation(shader.program, attribute.name);
            }

            shader.attributes.Add(attribute.name, attribute);
        }

        return shader;
    }

    void Compile()
    {
        unsafe
        {
            program = glCreateProgram();

            if (vert_src != null)
            {
                hfCreateShader(vert_src, GL.VERTEX_SHADER, ref vert, name);
                glAttachShader(program, vert);
            }

            if (frag_src != null)
            {
                hfCreateShader(frag_src, GL.FRAGMENT_SHADER, ref frag, name);
                glAttachShader(program, frag);
            }

            if (geo_src != null)
            {
                hfCreateShader(geo_src, GL.GEOMETRY_SHADER, ref geo, name);
                glAttachShader(program, geo);
            }

            hfLinkProgram(program);

            if (name != null)
            {
                hfNameObject(GL.FRAMEBUFFER, program, name);
            }
        }
    }

    public void Bind()
    {
        glUseProgram(program);
    }

    public void Delete()
    {
        glDeleteShader(program);
        program = 0;
    }

    public int GetAttibuteLocation(string name)
    {
        if (attributes.TryGetValue(name, out Attribute value))
        {
            return (int)value.location;
        }

        return -1;
    }

    public int GetUniform(string name)
    {
        if (uniforms.TryGetValue(name, out Uniform value))
        {
            return (int)value.location;
        }

        return -1;
    }

    public int GetUniformArray(string name, int index)
    {
        if (uniforms.TryGetValue(name, out Uniform value))
        {
            return value.location + index;
        }

        return -1;
    }

    public void SetVector3(string name, Vector3* Vector3)
    {
        int uniform = GetUniform(name);
        glUniform3fv(uniform, 1, (float*)Vector3);
    }

    public void SetVector3(string name, Vector3 Vector3)
    {
        int uniform = GetUniform(name);
        glUniform3fv(uniform, 1, (float*)&Vector3);
    }

    public void SetVector4(string name, Vector4* Vector4)
    {
        int uniform = GetUniform(name);
        glUniform4fv(uniform, 1, (float*)Vector4);
    }

    public void SetVector4(string name, Vector4 Vector4)
    {
        int uniform = GetUniform(name);
        glUniform4fv(uniform, 1, (float*)&Vector4);
    }

    public void SetVector2(string name, Vector2 Vector2)
    {
        int uniform = GetUniform(name);
        glUniform2fv(uniform, 1, (float*)&Vector2);
    }

    public void SetVector1(int uniform, float vec1)
    {
        glUniform1f(uniform, vec1);
    }

    public void SetVector1(string name, float vec1)
    {
        int uniform = GetUniform(name);
        glUniform1f(uniform, vec1);
    }

    public void SetVector4(int uniform, Vector4 Vector4)
    {
        glUniform4fv(uniform, 1, (float*)&Vector4);
    }

    public void SetMatrix4x4(string name, Matrix4x4 mat)
    {
        int uniform = GetUniform(name);
        glUniformMatrix4fv(uniform, 1, GL.FALSE, (float*)&mat);
    }

    public void SetMatrix4x4(string name, Matrix4x4* mat)
    {
        int uniform = GetUniform(name);
        glUniformMatrix4fv(uniform, 1, GL.FALSE, (float*)mat);
    }

    public void SetMatrix4x4(int uniform, Matrix4x4* mat)
    {
        glUniformMatrix4fv(uniform, 1, GL.FALSE, (float*)mat);
    }

    public void SetTexture(string name, int texture)
    {
        int uniform = GetUniform(name);
        glUniform1i(uniform, texture);
    }

    public void SetTexture(int uniform, int texture)
    {
        glUniform1i(uniform, texture);
    }

    public void SetTextureArray(string name, int index, int texture_slot)
    {
        int uniform = GetUniformArray(name, index);
        glUniform1i(uniform, texture_slot);
    }
}
