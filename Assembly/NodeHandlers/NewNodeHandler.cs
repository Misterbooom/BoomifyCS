using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Assembly.NodeHandlers.ClassHandler;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class NewNodeHandler : NodeHandler
    {
        public NewNodeHandler(AssemblyCompiler compiler) : base(compiler) { }
        public override void HandleNode(AstNode node)
        {
            AstNew astNew = (AstNew)node;
            compiler.Visit(astNew.ValueNode);
            IValue iValue = compiler.StackIValuePop();
            if (iValue is not ClassType)
            {
                new BifyTypeError("New expression supports only classes.").Throw();
            }
            ClassType classType = (ClassType)iValue;
            HandleBuildingClass(astNew.ArgumentsNode, classType);
        }
        private void HandleBuildingClass(AstNode argumentsNode, ClassType classType)
        {
            CallNodeHandler callNodeHandler = new CallNodeHandler(compiler);
            compiler.Visit(argumentsNode);
            List<BifyValue> arguments = callNodeHandler.GetArguments(CallNodeHandler.CountArgs(argumentsNode));
            ClassValue classValue = classType.InitClass();
            BifyMethodRef constructor = GetConstructor(classType,classValue);


            if (constructor == null && arguments.Count > 0)
            {
                BifyDebug.Log($"Argumetns count : {arguments.Count}");
                Traceback.Instance.ThrowException(new BifyAttributeError($"Class {classType.Name} doesn't have constructor."));
            }
            else if (constructor != null)
            {
                
                BifyFunction overload = constructor.Resolve([.. arguments.Select(x => x.GetBifyType())]);

                arguments = [.. arguments.Prepend(classValue)];

                callNodeHandler.ValidateAndAutoCastArguments(arguments, overload.FunctionArgs.BifyTypes, false, true);
                overload.Call([.. arguments]);
            }
            AssemblyCompiler.Instance.StackPush(classValue);
        }
        
        private BifyMethodRef GetConstructor(ClassType classType,ClassValue classValue)
        {
            //BifyDebug.Log($"[GetConstructor] Getting constructor for class {classType.Name} Current class is : {(compiler.CurrentClass == null ? "None" : compiler.CurrentClass)}");
            return classType.GetMethod("constructor", compiler.CurrentClass).GetMethodRef(classValue);
        }
    }
}
