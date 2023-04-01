using System.Text;
using static OpenGL;

public class GLShaderLoader
{
    string? vert, frag, geo;
    StringBuilder builder;

    GL shaderType;
    GL shaderTypeToSave;
    string version = "";

    List<string> defines;

    bool writeLines;

    Dictionary<string, string> buildInValues;

    public GLShaderLoader()
    {
        builder = new StringBuilder();
        defines = new List<string>();

        buildInValues = new Dictionary<string, string>();

        buildInValues.Add("$MAX_TEXTURE_IMAGE_UNITS", $"{hfGetMaxTextureImageUnits()}");
    }

    void SaveShader()
    {
        if (builder.Length > 0)
        {
            switch (shaderTypeToSave)
            {
                case GL.VERTEX_SHADER:
                    vert = version + builder.ToString();
                    break;

                case GL.FRAGMENT_SHADER:
                    frag = version + builder.ToString();
                    break;

                case GL.GEOMETRY_SHADER:
                    geo = version + builder.ToString();
                    break;
            }

            builder.Clear();
        }
    }

    public void AddDefine(string define)
    {
        if(Defined(define) == false)
        {
            defines.Add(define);
        }
    }

    bool Defined(string define)
    {
        return defines.Find(str => str.Equals(define)) != null;
    }

    public GLShader LoadShader(string path, string? debug_name)
    {
        shaderType = GL.NONE;
        shaderTypeToSave = GL.NONE;
        builder.Clear();
        vert = null; frag = null; geo = null;

        var lines = File.ReadAllLines(path);

        writeLines = true;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (StartsWithEx(line, "#"))
            {
                if (StartsWithEx(line, "#version"))
                {
                    version = line + "\n";
                }
                else if (StartsWithEx(line, "#if"))
                {
                    var define = line.TrimStart().Substring(3).TrimStart().TrimEnd();
                    writeLines = Defined(define);
                }
                else if (StartsWithEx(line, "#else"))
                {
                    writeLines = !writeLines;
                }
                else if (StartsWithEx(line, "#endif"))
                {
                    writeLines = true;
                }
                else if (StartsWithEx(line, "#vert"))
                {
                    SaveShader();
                    shaderTypeToSave = GL.VERTEX_SHADER;
                }
                else if (StartsWithEx(line, "#frag"))
                {
                    SaveShader();
                    shaderTypeToSave = GL.FRAGMENT_SHADER;
                }
                else if (StartsWithEx(line, "#geo"))
                {
                    SaveShader();
                    shaderTypeToSave = GL.GEOMETRY_SHADER;
                } 
            }
            else if (writeLines)
            {
                foreach (var vk in buildInValues)
                {
                    line = line.Replace(vk.Key, vk.Value);
                }

                builder.Append(line);
                builder.Append("\n");
            }
        }

        SaveShader();

        return GLShader.CreateShaderFromSource(vert, frag, geo, debug_name);
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

}
