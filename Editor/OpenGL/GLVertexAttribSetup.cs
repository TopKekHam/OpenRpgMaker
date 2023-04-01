using static OpenGL;

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

    public GLVertexAttribSetup()
    {
        attribs = new SortedList<uint, GLVertexAttrib>();
    }

    public void AddAttrib(int sizeInBytes, GL type = GL.FLOAT)
    {
        uint position = (uint)attribs.Count;
        attribs.Add(position, new GLVertexAttrib(position, sizeInBytes, type));
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

            glEnableVertexAttribArray(attrib.position);
            glVertexAttribPointer(attrib.position, attrib.sizeInBytes / 4, attrib.type, GL.FALSE, stride, offset);
            offset = (byte*)offset + attrib.sizeInBytes;
        }

    }

}
