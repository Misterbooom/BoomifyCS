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
            BifyFunction constructor = GetConstructor(classType);
            ClassValue classValue = classType.InitClass();

           
            if (constructor == null && arguments.Count > 0)
            {
                BifyDebug.Log($"Argumetns count : {arguments.Count}");
                Traceback.Instance.ThrowException(new BifyAttributeError($"Class {classType.Name} doesn't have constructor."));
            }
            else if (constructor != null)
            {
                arguments = arguments.Prepend(classValue).ToList();
                //foreach (var argument in arguments)
                //{
                //    Console.WriteLine($"Argument: {argument}");
                //}
                callNodeHandler.ValidateAndAutoCastArguments(arguments, constructor.FunctionArgs.BifyTypes, false, true);
                constructor.Call(arguments.ToArray());
            }
            AssemblyCompiler.Instance.StackPush(classValue);
        }
        
        private BifyFunction GetConstructor(ClassType classType)
        {
            
            return null;
        }
    }
}
