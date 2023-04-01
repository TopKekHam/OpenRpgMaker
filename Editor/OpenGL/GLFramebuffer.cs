using static OpenGL;

public unsafe class GLFramebuffer
{

    public class GLFramebufferAttchment
    {
        public GL attachmentPlace;
        public GL format;
        public uint textureObject;
    }

    public Vector2I size { get; private set; }

    public int handle { get; private set; }

    OpenGL gl;
    List<GLFramebufferAttchment> attachments = new List<GLFramebufferAttchment>();

    public GLFramebuffer(string debugName = "unknow")
    {

        handle = (int)glGenFramebuffer(debugName);
    }

    public GLFramebuffer(uint handle, string debugName = "unknow")
    {
        this.handle = (int)handle;

        if (handle != 0)
        {
            hfNameObject(GL.FRAMEBUFFER, handle, debugName);
        }
    }

    public void Bind()
    {
        glBindFramebuffer(GL.FRAMEBUFFER, (uint)handle);
    }

    public void Unbind()
    {
        glBindFramebuffer(GL.FRAMEBUFFER, 0);
    }

    public void SetSize(int width, int height)
    {
        size = new Vector2I(width, height);

        for (int i = 0; i < attachments.Count; i++)
        {
            var attachment = attachments[i];
            glTextureStorage2D(attachment.textureObject, 1, attachment.format, (uint)size.X, (uint)size.Y);
        }
    }

    public void SetAttachment(GL attachmentPlace, GL format)
    {
        GLFramebufferAttchment? attachment = null;

        for (int i = 0; i < attachments.Count; i++)
        {
            if (attachments[i].attachmentPlace == attachmentPlace)
            {
                attachment = attachments[i];
            }
        }

        if(attachment == null)
        {
            uint to;
            glGenTextures(1, &to);

            attachment = new GLFramebufferAttchment() {
                textureObject = to
            };

            attachments.Add(attachment);
        }

        attachment.attachmentPlace = attachmentPlace;
        attachment.format = format;

        glBindTexture(GL.TEXTURE_2D, attachment.textureObject);
        glTexStorage2D(GL.TEXTURE_2D, 1, attachment.format, (uint)size.X, (uint)size.Y);
        //gl.glTextureStorage2D(attachment.textureObject, 1, attachment.format, (uint)size.X, (uint)size.Y);
        glBindFramebuffer(GL.FRAMEBUFFER, (uint)handle);
        glFramebufferTexture2D(GL.FRAMEBUFFER, attachment.attachmentPlace, GL.TEXTURE_2D, attachment.textureObject, 0);
        //gl.glNamedFramebufferTexture((uint)handle, attachment.attachmentPlace, attachment.textureObject, 0);

        GL res = glCheckFramebufferStatus(GL.FRAMEBUFFER);

        if(res != GL.FRAMEBUFFER_COMPLETE)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Framebuffer incomplete: {res}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public uint GetAttachmentTexture(GL attachmentPlace)
    {
        for (int i = 0; i < attachments.Count; i++)
        {
            if(attachments[i].attachmentPlace == attachmentPlace)
            {
                return attachments[i].textureObject;
            }
        }

        throw new Exception($"No textures attached to {attachmentPlace}");
    }

    public void Delete()
    {
        glDeleteFramebuffer((uint)handle);
        handle = -1;

        for (int i = 0; i < attachments.Count; i++)
        {
            uint texture = attachments[i].textureObject;
            glDeleteBuffers(1, &texture);
        }
    }

}

