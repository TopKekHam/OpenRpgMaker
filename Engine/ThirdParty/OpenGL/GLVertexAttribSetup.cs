namespace ThirdParty.OpenGL;

public unsafe class GLVertexAttribSetup
{
    public struct GLVertexAttrib
    {
        public uint position;
        public int sizeInBytes;
        public GL type;

        public GLVertexAttrib(uint position, int sizeInBytes, GL type)
        {
            this.position = position;
            this.sizeInBytes = sizeInBytes;
            this.type = type;
        }
    }

    SortedList<uint, GLVertexAttrib> attribs;

    OpenGL gl;

    public GLVertexAttribSetup(OpenGL gl)
    {
        this.gl = gl;
        attribs = new SortedList<uint, GLVertexAttrib>();
    }

    public void AddAttrib(int sizeInBytes, GL type = GL.FLOAT)
    {
        uint position = (uint)attribs.Count;
        attribs.Add(position, new GLVertexAttrib(position, sizeInBytes, type));
    }

    public void FromShader(GLShader shader)
    {
        SortedList<int, GLShaderAttribute> sortedAttribs = new SortedList<int, GLShaderAttribute>();
        
        foreach (var kv in shader.Attributes)
        {
            int location = kv.Value.location;
            sortedAttribs.Add(location, kv.Value);
        }

        for (int i = 0; i < sortedAttribs.Count; i++)
        {
            var attrib = sortedAttribs[i];
            AddAttrib(GetSizeInBytes(attrib.type), GetType(attrib.type));
        }
    }
    
    public void Bind(void* bufferOffset = null)
    {
        int stride = 0;

        for (int i = 0; i < attribs.Count; i++)
        {
            stride += attribs.Values[i].sizeInBytes;
        }

        void* offset = bufferOffset;

        for (int i = 0; i < attribs.Count; i++)
        {
            var attrib = attribs.Values[i];

            gl.glEnableVertexAttribArray(attrib.position);
            gl.glVertexAttribPointer(attrib.position, attrib.sizeInBytes / 4, attrib.type, GL.FALSE, stride, offset);
            offset = (byte*)offset + attrib.sizeInBytes;
        }

    }

    int GetSizeInBytes(GL type)
    {
        switch (type)
        {
            case GL.INT: return 4;
            case GL.INT_VEC2: return 8;
            case GL.INT_VEC3: return 12;
            case GL.INT_VEC4: return 16;
            
            case GL.FLOAT: return 4;
            case GL.FLOAT_VEC2: return 8;
            case GL.FLOAT_VEC3: return 12;
            case GL.FLOAT_VEC4: return 16; 
        }

        throw new Exception("Type not supported!");
    }

    GL GetType(GL type)
    {
        switch (type)
        {
            case GL.INT: return GL.INT;
            case GL.INT_VEC2: return GL.INT;
            case GL.INT_VEC3: return GL.INT;
            case GL.INT_VEC4: return GL.INT;
            
            case GL.FLOAT: return GL.FLOAT;
            case GL.FLOAT_VEC2: return GL.FLOAT;
            case GL.FLOAT_VEC3: return GL.FLOAT;
            case GL.FLOAT_VEC4: return GL.FLOAT; 
        }
        
        throw new Exception("Type not supported!");
    }

}
