using ThirdParty.OpenGL;

namespace SBEngine;

public class Material
{
    public GLShader Shader { get; private set; }
    public KeyValueArrayList<string, VariableValueUnion> Variables { get; private set; }

    public Material(GLShader shader)
    {
        Shader = shader;
        Variables = new KeyValueArrayList<string, VariableValueUnion>();

        foreach (var kv in Shader.Variables)
        {
            var glVar = shader.Variables[kv.Key];

            if (glVar.DefaultValue != null)
            {
                Variables[kv.Key] = VariableValueUnion.As(glVar.DefaultValue, kv.Value.Type);
            }
            else
            {
                Variables[kv.Key] = VariableValueUnion.GetDefaultValue(kv.Value.Type);
            }
            
        }
    }

    public void ApplyValues()
    {
        foreach (var kv in Variables)
        {
            var variable = Shader.Variables[kv.Key];
            kv.Value.Set(Shader, variable.Type, variable.Uniform);
        }
    }
}