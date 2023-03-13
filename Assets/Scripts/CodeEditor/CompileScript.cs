using Microsoft.CSharp;
using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using CodeEditorComponents;
using CommandControl;

//WARNING: Using infinite loops in the compiler will cause Unity to freeze
//this happens with any infinite loop

public class CompileScript : MonoBehaviour{
    public Text input;
    public static CompileScript Instance { get; private set; }

    public void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void Run(){
        Reset.Instance.Exterminatus();

        //put the compiler caller in a waiter coroutine to offset it a bit from scene load
        //because if it runs at the same time as the scene reloads
        //i.e. when Exterminatus is declared
        //the bot will look in one direction and reset immediately
        StartCoroutine(waiter());
    }

    private IEnumerator waiter(){
        yield return new WaitForSeconds(0.05f);
        if(Reset.isResetSuccessful()){
            Debug.LogAssertion("Exterminatus successful");
            //meat and potatoes
            //compiler goes through player code and executes methods found in Bot
            //after everything is enqueued, calls execute();
            //execute() then starts the queue and the magic happens
            
            try{
                var assembly = Compile(input.text);
                var method = assembly.GetType("Gawain").GetMethod("Main");
                var del = (Action)Delegate.CreateDelegate(typeof(Action), method);
                del.Invoke();

                Debug.LogWarning("Executuion started");
                Bot.Instance.execute();
                //throw new OutOfMemoryException();
            } catch(OutOfMemoryException e) {
                Debug.Log(e);
                Debug.Log("did you write an infinte loop?");
            }
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
