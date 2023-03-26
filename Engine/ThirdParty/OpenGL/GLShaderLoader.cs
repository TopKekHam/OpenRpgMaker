using System.Numerics;
using System.Text;
using SBEngine;

namespace ThirdParty.OpenGL;

public enum GLShaderVariableType
{
    Float,
    Vector2,
    Vector3,
    Vector4,
    Texture2D,
    Matrix4
}

public class GLShaderVariable
{
    public GLShaderVariableType Type;
    public string Name;
    public int Uniform;
    public object? DefaultValue;

    public GLShaderVariable(GLShaderVariableType type, string name, int uniform, object defaultValue = null)
    {
        Type = type;
        Name = name;
        Uniform = uniform;
        DefaultValue = defaultValue;
    }
}

public class GLShaderLoader
{
    private string? _vert, _frag, _geo;
    private StringBuilder _stringBuilder;
    private GL _shaderType;
    private GL _shaderTypeToSave;
    private string _version = "";
    private OpenGL _gl;
    private List<string> _defines;
    private bool _writeLines;
    private Dictionary<string, string> _buildInValues;
    private string _prevVariableLine;
    
    public GLShaderLoader(OpenGL gl)
    {
        this._gl = gl;
        _stringBuilder = new StringBuilder();
        _defines = new List<string>();

        _buildInValues = new Dictionary<string, string>();

        _buildInValues.Add("$MAX_TEXTURE_IMAGE_UNITS", $"{gl.hfGetMaxTextureImageUnits()}");
    }

    void SaveShader()
    {
        if (_stringBuilder.Length > 0)
        {
            switch (_shaderTypeToSave)
            {
                case GL.VERTEX_SHADER:
                    _vert = _version + _stringBuilder.ToString();
                    break;

                case GL.FRAGMENT_SHADER:
                    _frag = _version + _stringBuilder.ToString();
                    break;

                case GL.GEOMETRY_SHADER:
                    _geo = _version + _stringBuilder.ToString();
                    break;
            }

            _stringBuilder.Clear();
        }
    }

    public void AddDefine(string define)
    {
        if (Defined(define) == false)
        {
            _defines.Add(define);
        }
    }

    bool Defined(string define)
    {
        return _defines.Find(str => str.Equals(define)) != null;
    }

    public GLShader LoadShaderFromFile(ResourcePath path, string? debug_name)
    {
        return LoadShaderFromSource(File.ReadAllText(path.PathFromRoot), debug_name);
    }
    
    public GLShader LoadShaderFromSource(string source, string? debug_name)
    {
        _shaderType = GL.NONE;
        _shaderTypeToSave = GL.NONE;
        _stringBuilder.Clear();
        _vert = null;
        _frag = null;
        _geo = null;

        var lines = source.Split("\r\n");

        _writeLines = true;
        bool nextLineVariable = false;

        var variables = new Dictionary<string, GLShaderVariable>();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (StartsWithEx(line, "#"))
            {
                if (StartsWithEx(line, "#version"))
                {
                    _version = line + "\n";
                }
                else if (StartsWithEx(line, "#if"))
                {
                    var define = line.TrimStart().Substring(3).TrimStart().TrimEnd();
                    _writeLines = Defined(define);
                }
                else if (StartsWithEx(line, "#else"))
                {
                    _writeLines = !_writeLines;
                }
                else if (StartsWithEx(line, "#endif"))
                {
                    _writeLines = true;
                }
                else if (StartsWithEx(line, "#vert"))
                {
                    SaveShader();
                    _shaderTypeToSave = GL.VERTEX_SHADER;
                }
                else if (StartsWithEx(line, "#frag"))
                {
                    SaveShader();
                    _shaderTypeToSave = GL.FRAGMENT_SHADER;
                }
                else if (StartsWithEx(line, "#geo"))
                {
                    SaveShader();
                    _shaderTypeToSave = GL.GEOMETRY_SHADER;
                }
                else if (StartsWithEx(line, "#var"))
                {
                    nextLineVariable = true;
                    _prevVariableLine = line;
                }
            }
            else if (_writeLines)
            {
                if (nextLineVariable) // parsing variable
                {
                    nextLineVariable = false;
                    
                    // uniform sampler2D uTexture
                    var words = line.Split(' ', ';');

                    if (words[0] != "uniform") throw new Exception("Bad variable definition.");

                    var type = ParseType(words[1]);
                    var name = words[2];

                    var value = ParseVariableValue(type ,_prevVariableLine);

                    variables.Add(name, new GLShaderVariable(type, name, 0, value));
                }

                foreach (var vk in _buildInValues)
                {
                    line = line.Replace(vk.Key, vk.Value);
                }

                _stringBuilder.Append(line);
                _stringBuilder.Append("\n");
            }
        }

        SaveShader();

        var shader = GLShader.CreateShaderFromSource(_gl, _vert, _frag, _geo, debug_name);
        shader.Variables = variables;
        
        // resolving variable uniforms;

        foreach (var kv in variables)
        {
            kv.Value.Uniform = shader.GetUniform(kv.Key);
        }
        
        return shader;
    }

    GLShaderVariableType ParseType(string typeString)
    {
        if (typeString == "mat4")
            return GLShaderVariableType.Matrix4;
        if (typeString == "sampler2D")
            return GLShaderVariableType.Texture2D;
        if (typeString == "float")
            return GLShaderVariableType.Float;
        if (typeString == "vec2")
            return GLShaderVariableType.Vector2;
        if (typeString == "vec3")
            return GLShaderVariableType.Vector3;
        if (typeString == "vec4")
            return GLShaderVariableType.Vector4;

        throw new Exception($"Unknown type: {typeString}.");
    }

    // ignores whites space in the start
    public bool StartsWithEx(string src, string comp)
    {
        int i = 0;

        while (i < src.Length && char.IsWhiteSpace(src[i]))
        {
            i++;
        }

        if (src.Length - i < comp.Length) return false;

        for (int ic = 0; ic < comp.Length && i < src.Length; ic++, i++)
        {
            if (src[i] != comp[ic]) return false;
        }

        return true;
    }

    object ParseVariableValue(GLShaderVariableType type, string varLine)
    {

        int i = 0;

        while(varLine.Length > i && char.IsWhiteSpace(varLine[i])) i++; // remove white space

        if (i == varLine.Length) return null;
        
        i += 4; // remove #var

        while(varLine.Length > i && char.IsWhiteSpace(varLine[i])) i++; // remove white space

        if (i == varLine.Length) return null;
        
        if (type == GLShaderVariableType.Float)
        {
            return NextFloat(varLine, ref i);
        }
        else if (type == GLShaderVariableType.Vector2)
        {
            Vector2 vec = Vector2.Zero;
            
            vec.X = NextFloat(varLine, ref i);
            vec.Y = NextFloat(varLine, ref i);

            return vec;
        }
        else if (type == GLShaderVariableType.Vector3)
        {
            Vector3 vec = Vector3.Zero;
            
            vec.X = NextFloat(varLine, ref i);
            vec.Y = NextFloat(varLine, ref i);
            vec.Z = NextFloat(varLine, ref i);

            return vec;
        }
        else if (type == GLShaderVariableType.Vector4)
        {
            Vector4 vec = Vector4.Zero;
            
            vec.X = NextFloat(varLine, ref i);
            vec.Y = NextFloat(varLine, ref i);
            vec.Z = NextFloat(varLine, ref i);
            vec.W = NextFloat(varLine, ref i);

            return vec;
        }
        else if (type == GLShaderVariableType.Matrix4)
        {
            return Matrix4x4.Identity;
        }
        else if (type == GLShaderVariableType.Texture2D)
        {
            return NextInt(varLine, ref i);
        }

        return null;
    }

    float NextFloat(string str, ref int i)
    {
        while(str.Length > i && (char.IsWhiteSpace(str[i]) || str[i] == ',')) i++; // remove white space

        int start = i;
        
        while(str.Length > i && (char.IsWhiteSpace(str[i]) == false && str[i] != ',')) i++; // remove white space

        var span = str.AsSpan(start, i - start);

        return float.Parse(span);
    }
    
    int NextInt(string str, ref int i)
    {
        while(str.Length > i && (char.IsWhiteSpace(str[i]) || str[i] == ',')) i++; // remove white space

        int start = i;
        
        while(str.Length > i && (char.IsWhiteSpace(str[i]) == false && str[i] != ',')) i++; // remove white space

        var span = str.AsSpan(start, i - start);

        return int.Parse(span);
    }

}