using System;
using System.Collections.Generic;

namespace BoomifyCS.Assembly
{
    class AssemblerCodeManager
    {
        private List<string> code = new List<string>();
        private Dictionary<string, List<string>> labeledCode = new Dictionary<string, List<string>>();
        
        public AssemblerCodeManager() { }

        public void AddInstruction(string instruction, int indentLevel = 1)
        {
            string indent = new string(' ', indentLevel * 4); 
            this.code.Add(indent + instruction);
        }

        public void AddComment(string comment)
        {
            this.code.Add("; " + comment);
        }

        public void AddInstructionBlock(IEnumerable<string> instructions)
        {
            foreach (var instruction in instructions)
            {
                AddInstruction(instruction);
            }
        }

        public void AddInstructionForLabel(string label, string instruction,int indentLevel = 1)
        {
            if (!labeledCode.ContainsKey(label))
            {
                labeledCode[label] = new List<string>();
            }
            string indent = new string(' ', indentLevel * 4);
            labeledCode[label].Add(indent + instruction);
        }

        public string GetCode()
        {
            List<string> allCode = new List<string>(this.code);
            foreach (var label in labeledCode.Keys)
            {
                allCode.Add(label + ":");
                allCode.AddRange(labeledCode[label]);
            }

            return string.Join(Environment.NewLine, allCode);
        }



        public void ClearCode()
        {
            this.code.Clear();
        }

        public void RemoveLastInstruction()
        {
            if (this.code.Count > 0)
            {
                this.code.RemoveAt(this.code.Count - 1);
            }
        }
    }
}
