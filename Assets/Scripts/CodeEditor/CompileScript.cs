using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using CodeEditorComponents;
using CommandControl;

public class CompileScript : MonoBehaviour{
    public Text input;
    SetEditorText set;

    public void Awake(){
        set = gameObject.GetComponent<SetEditorText>();
    }

    public void Run(){
        if(Reset.isResetSuccessful()){
            Debug.LogAssertion("Exterminatus successful");
                        
            //meat and potatoes
            var assembly = Compile(input.text);
            var method = assembly.GetType("Gawain").GetMethod("Main");
            var del = (Action)Delegate.CreateDelegate(typeof(Action), method);
            del.Invoke();
            
            Debug.LogWarning("Executuion started");
            Bot.Instance.execute();
        } else {
            Debug.LogAssertion("Exterminatus unsuccessful");
        }
    }

    public Assembly Compile(string source){
        var provider = new CSharpCodeProvider();
        var param = new CompilerParameters();

        foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()){
            param.ReferencedAssemblies.Add(assembly.Location);
        }

        param.GenerateExecutable = false;
        param.GenerateInMemory = true;

        var result = provider.CompileAssemblyFromSource(param, source);

        if(result.Errors.Count > 0){
            var msg = new StringBuilder();
            foreach(CompilerError error in result.Errors){
                msg.AppendFormat("Error ({0}): {1}\n", error.ErrorNumber, error.ErrorText);
            }
            throw new Exception(msg.ToString());
        }

        return result.CompiledAssembly;
    }
}
