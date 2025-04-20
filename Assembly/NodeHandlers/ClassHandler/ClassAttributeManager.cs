using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Ast;
using BoomifyCS.Exceptions;
using BoomifyCS.Lexer;

namespace BoomifyCS.Assembly.NodeHandlers.ClassHandler
{
    struct ClassAttribute
    {
        public string Name;
        public BifyValue Value;
        public BifyType Type;
        public override string ToString()
        {
            return $"{Name}(value:{Value};type:{Type})";
        }
    }
    class ClassAttributeManager
    {
        private AstClass classNode;
        private AssemblyCompiler compiler = AssemblyCompiler.Instance;
        public ClassAttributeManager(AstClass classNode) {
            this.classNode = classNode;
        }
        public ClassAttribute[] GetAttributes()
        {
            List<ClassAttribute> attributes = new List<ClassAttribute>();
            foreach (AstNode node in ((AstBlock)classNode.BodyNode).ChildNodes)
            {
                if (node is AstVarDecl varDecl)
                {
                    attributes.Add(HandleAttribute(varDecl));
                }
            }
            return attributes.ToArray();
        }
        private ClassAttribute HandleAttribute(AstVarDecl node)
        {
            string varName = node.AssignmentNode.Left.Token.Value;
            var variableHandler = new VariableDeclarationNodeHandler(AssemblyCompiler.Instance);
            BifyType attributeType = variableHandler.DetermineVariableType(node,varName);
            BifyValue attributeValue = variableHandler.GetVariableValue(node, varName,attributeType);
            ClassAttribute attribute = new ClassAttribute();
            attribute.Name = varName;
            attribute.Value = attributeValue;
            attribute.Type = attributeType;

            return attribute;
        }
       
    }
    
}
