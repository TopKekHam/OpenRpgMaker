using System.Numerics;
using System.Runtime.InteropServices;
using ThirdParty.OpenGL;
using ThirdParty.SDL;

namespace SBEngine;

[StructLayout(LayoutKind.Sequential)]
public struct DrawCallCommand
{
    public uint First;
    public uint Count;

    public DrawCallCommand(uint first, uint count)
    {
        First = first;
        Count = count;
    }
}

public unsafe class Renderer
{
    public int FramesRendered { get; private set; }
    public bool RenderingFrame { get; private set; }
    public GLShaderLoader ShaderLoader { get; private set; }
    public OpenGL Gl { get; private set; }
    
    private AssetsDatabase _assetsDatabase;

    // draw state

    private SortedList<int, List<DrawCall>> _renderQueue;

    // GL variables
    private GLVAO _vao;
    //private GLBuffer _vbo;
    //private GLVertexAttribSetup _vboAttibSetup;
    private GLShader _uberShader;

    private GLBuffer _indirectBuffer;

    public Renderer(Window window, AssetsDatabase assetsDatabase, PlatformEvents platformEvents)
    {
        Gl = window.GetOpenGLReference();
        _assetsDatabase = assetsDatabase;

        ShaderLoader = new GLShaderLoader(Gl);

        _vao = new GLVAO(Gl, "Renderer VAO");
        _vao.Bind();

        _indirectBuffer = new GLBuffer(Gl, GL.DRAW_INDIRECT_BUFFER, "Rendere indirect buffer");
        //_vbo = new GLBuffer(_gl, GL.ARRAY_BUFFER, "Renderer VBO");
        //_vbo.Bind();

        //_vboAttibSetup = new GLVertexAttribSetup(_gl);
        //
        //_vboAttibSetup.AddAttrib(sizeof(Vector2));
        //_vboAttibSetup.AddAttrib(sizeof(Vector2));
        //_vboAttibSetup.AddAttrib(sizeof(Vector4));
        //_vboAttibSetup.AddAttrib(sizeof(float));
        //
        //_vboAttibSetup.Bind();

        _uberShader = ShaderLoader.LoadShaderFromFile("/Shaders/uber.glsl", "Renderer Uber Shader");

        _renderQueue = new SortedList<int, List<DrawCall>>();
        
        platformEvents.SDLEventEvent.Subscribe(HandleEvent);
    }

    void HandleEvent(SDL_Event sdlEvent)
    {
        if (sdlEvent.window.event_id == SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
        {
            Gl.glViewport(0, 0, (uint)sdlEvent.window.data1, (uint)sdlEvent.window.data2);
        }
    }

    public void QueueDrawCall(DrawCall drawCall, int layer)
    {

        List<DrawCall> drawCalls;
        
        if (_renderQueue.TryGetValue(layer, out drawCalls) == false)
        {
            drawCalls = new List<DrawCall>();
            _renderQueue.Add(layer, drawCalls);
        }

        drawCalls.Add(drawCall);
    }

    public void ClearColor(Vector4 color)
    {
        Gl.glClearColor(color.X, color.Y, color.Z, color.W);
        Gl.glClear(GL.COLOR_BUFFER_BIT);
    }

    public void ClearDepth()
    {
        Gl.glClear(GL.DEPTH_BUFFER_BIT);
    }

    public void Render(Camera2D camera2D)
    {
        
        // alpha blending
        Gl.glEnable(ThirdParty.OpenGL.GL.BLEND);
        Gl.glBlendFunc(ThirdParty.OpenGL.GL.SRC_ALPHA, ThirdParty.OpenGL.GL.ONE_MINUS_SRC_ALPHA);

        var layersToDraw = _renderQueue.Keys.ToArray();
        
        for (int layerIndex = 0; layerIndex < layersToDraw.Length; layerIndex++)
        {
            var layerKey = layersToDraw[layerIndex];
            var layerDrawCalls = _renderQueue[layerKey];
            
            for (int i = 0; i < layerDrawCalls.Count; i++)
            {
                var drawCall = layerDrawCalls[i];
         
                // do one draw call
                
                _vao.Bind();
                drawCall.Mesh.Bind();
                
                drawCall.Material.Shader.Bind();
                drawCall.Material.Shader.SetMatrix4x4("uCamera", camera2D.GetMatrix4x4());
                drawCall.Material.Shader.SetMatrix4x4("uModel", drawCall.Transform.GetMatrix4x4());
                drawCall.Material.ApplyValues();

                if (drawCall.Multiple != null)
                {
                    fixed (int* firstPtr = drawCall.Multiple.First.Array)
                    fixed (uint* countPtr = drawCall.Multiple.Count.Array)
                    {
                        Gl.glMultiDrawArrays(GL.TRIANGLES, firstPtr, countPtr, drawCall.Multiple.DrawCount);
                    }
                }
                else
                {
                    Gl.glDrawArrays(GL.TRIANGLES, drawCall.VertexOffset, (uint)drawCall.VertexCount);
                }
                
            }

            layerDrawCalls.Clear();
        }
        
    }

    public void SetDepthTesting(bool value)
    {
        if (value)
        {
            Gl.glEnable(GL.DEPTH_TEST);
        }
        else
        {
            Gl.glDisable(GL.DEPTH_TEST);
        }
    }

}