//lmao i give up

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;

public class MapGrid : MonoBehaviour{
    static CompilerParameters CompilerParams = new CompilerParameters{
        GenerateInMemory = true,
        TreatWarningsAsErrors = false,
        GenerateExecutable = false
    };
    static CSharpCodeProvider provider = new CSharpCodeProvider();
    static string[] references = {"System.dll", "mscorlib.dll"};
    
    // Start is called before the first frame update
    void Start(){
        string dest = Application.dataPath + "/EditorSaves.json";
        CompilerParams.ReferencedAssemblies.AddRange(references);
        string[] code = {
            "using System;" + 
            "using System.IO;" + 
                "namespace dynamiccompilation" + 
                "{"+
                "   public class DynamicExample"+
                "   {"+
                "       static public void Main()"+
                "       {"+
                "           int guh = 5 + 2;"+
                "           File.WriteAllText(" + dest + ",\"chinko\");",
                "       }"+
                "   }"+
                "}"};
        CompileAndRun(code);
    }

    void CompileAndRun(string[] code){
        CompilerResults compile = provider.CompileAssemblyFromSource(CompilerParams, code);
        if(compile.Errors.HasErrors){
            foreach(CompilerError ce in compile.Errors){
                Debug.Log(ce.ToString());
            }
            return;
        } else {
            Module module = compile.CompiledAssembly.GetModules()[0];

            module
                .GetType("dynamiccompilation.DynamicExample")
                .GetMethod("Main")
                .Invoke(null, new object[] { });
        }
    }
}
