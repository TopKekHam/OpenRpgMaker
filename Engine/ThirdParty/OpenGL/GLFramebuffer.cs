using SBEngine;

namespace ThirdParty.OpenGL;

public unsafe class GLFramebuffer
{

    public class GLFramebufferAttchment
    {
        public GL AttachmentPlace;
        public GL Format;
        public uint TextureObject;
    }

    public Vector2I Size { get; private set; }
    public uint Handle { get; private set; }

    OpenGL _gl;
    List<GLFramebufferAttchment> _attachments = new List<GLFramebufferAttchment>();

    public GLFramebuffer(OpenGL gl, string debugName = "unknown framebuffer")
    {
        this._gl = gl;
        
        uint handle;
        gl.glCreateFramebuffers(1, &handle);
        Handle = handle;
        
        gl.hfNameObject(GL.FRAMEBUFFER, handle, debugName);
    }

    public void Bind()
    {
        _gl.glBindFramebuffer(GL.FRAMEBUFFER, (uint)Handle);
    }

    public void SetSize(int width, int height)
    {
        Size = new Vector2I(width, height);

        for (int i = 0; i < _attachments.Count; i++)
        {
            var attachment = _attachments[i];
            _gl.glTextureStorage2D(attachment.TextureObject, 1, attachment.Format, (uint)Size.X, (uint)Size.Y);
        }
    }

    public void SetAttachment(GL attachmentPlace, GL format)
    {
        GLFramebufferAttchment? attachment = null;

        for (int i = 0; i < _attachments.Count; i++)
        {
            if (_attachments[i].AttachmentPlace == attachmentPlace)
            {
                attachment = _attachments[i];
            }
        }

        if(attachment == null)
        {
            uint to;
            _gl.glGenTextures(1, &to);

            attachment = new GLFramebufferAttchment() {
                TextureObject = to
            };

            _attachments.Add(attachment);
        }

        attachment.AttachmentPlace = attachmentPlace;
        attachment.Format = format;

        _gl.glBindTexture(GL.TEXTURE_2D, attachment.TextureObject);
        _gl.glTexStorage2D(GL.TEXTURE_2D, 1, attachment.Format, (uint)Size.X, (uint)Size.Y);
        //gl.glTextureStorage2D(attachment.textureObject, 1, attachment.format, (uint)size.X, (uint)size.Y);
        _gl.glBindFramebuffer(GL.FRAMEBUFFER, (uint)Handle);
        _gl.glFramebufferTexture2D(GL.FRAMEBUFFER, attachment.AttachmentPlace, GL.TEXTURE_2D, attachment.TextureObject, 0);
        //gl.glNamedFramebufferTexture((uint)handle, attachment.attachmentPlace, attachment.textureObject, 0);

        GL res = _gl.glCheckFramebufferStatus(GL.FRAMEBUFFER);

        if(res != GL.FRAMEBUFFER_COMPLETE)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Framebuffer incomplete: {res}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public uint GetAttachmentTexture(GL attachmentPlace)
    {
        for (int i = 0; i < _attachments.Count; i++)
        {
            if(_attachments[i].AttachmentPlace == attachmentPlace)
            {
                return _attachments[i].TextureObject;
            }
        }

        throw new Exception($"No textures attached to {attachmentPlace}");
    }

    public void Delete()
    {
        _gl.glDeleteFramebuffer(Handle);
        
        for (int i = 0; i < _attachments.Count; i++)
        {
            uint texture = _attachments[i].TextureObject;
            _gl.glDeleteBuffers(1, &texture);
        }

        _gl = null;
    }

}

