using System.Collections.Generic;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;

namespace BoomifyCS.Assembly.NodeHandlers.ClassHandler
{
    internal class ClassAttributeManager(AstClass classNode)
    {
        public Dictionary<ClassAttribute, AstNode>  GetAttributes()
        {
            Dictionary<ClassAttribute, AstNode> attributes = new();
            foreach (AstNode node in ((AstBlock)classNode.BodyNode).ChildNodes)
            {
                if (node is AstVarDecl varDecl)
                {
                    var res = HandleAttribute(varDecl);
                    attributes.Add(res.Item1, res.Item2);
                }
            }
            return attributes;
        }
        private (ClassAttribute, AstNode) HandleAttribute(AstVarDecl node)
        {

            
            string varName = node.AssignmentNode.Left.Token.Value;
            var variableHandler = new VariableDeclarationNodeHandler(AssemblyCompiler.Instance);
            var (attributeType, _) = variableHandler.DetermineVariableType(node,varName);
            FlagProcessor.SetFlags(FlagContext.CLASS_ATTRIBUTE, attributeType, node.Flag.Flags);
            // BifyValue attributeValue = variableHandler.GetVariableValue(node, varName,attributeType);
            
            ClassAttribute attribute = new ClassAttribute(varName,attributeType);
        
            BifyDebug.Log($"attribute: {attribute}");
            return (attribute, node);
        }
       
    }
    
}
