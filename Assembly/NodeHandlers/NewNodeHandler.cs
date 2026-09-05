using System.Collections.Generic;
using System.Linq;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly.NodeHandlers
{
    class NewNodeHandler(AssemblyCompiler compiler) : NodeHandler(compiler)
    {
        public override void HandleNode(AstNode node)
        {
            AstNew astNew = (AstNew)node;
            Compiler.Visit(astNew.ValueNode);
            IValue iValue = Compiler.StackIValuePop();
            if (iValue is not ClassType)
            {
                new BifyTypeError("New expression supports only classes.").Throw();
            }
            ClassType classType = (ClassType)iValue;
            HandleBuildingClass(astNew.ArgumentsNode, classType);
        }
        private void HandleBuildingClass(AstNode argumentsNode, ClassType classType)
        {
            CallNodeHandler callNodeHandler = new CallNodeHandler(Compiler);
            Compiler.Visit(argumentsNode);
            List<BifyValue> arguments = callNodeHandler.GetArguments(CallNodeHandler.CountArgs(argumentsNode));
            ClassValue classValue = classType.InitClass();
            BifyMethodRef constructor = GetConstructor(classType,classValue);


            if (constructor == null && arguments.Count > 0)
            {
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
            // ReSharper disable once PossibleNullReferenceException
            return classType.GetMethod("constructor", Compiler.CurrentClass).GetMethodRef(classValue);
        }
    }
}
