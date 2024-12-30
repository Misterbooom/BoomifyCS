using BoomifyCS.Assembly;
using BoomifyCS.Ast;
using BoomifyCS.Lexer;
namespace BoomifyCS.Assembly.NodeHandlers
{
    class BinaryOpNodeHandler : NodeHandler
    {
        public BinaryOpNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            if (node is AstBinaryOp binaryOp)
            {
                // Если обе части - константы, применяем константную свертку
                if (binaryOp.Left is AstNumber leftConst && binaryOp.Right is AstNumber rightConst)
                {
                    long leftValue = long.Parse(leftConst.Token.Value);
                    long rightValue = long.Parse(rightConst.Token.Value);
                    long result = 0;

                    // Выполняем операцию на этапе компиляции
                    switch (binaryOp.Token.Type)
                    {
                        case TokenType.ADD:
                            result = leftValue + rightValue;
                            break;
                        case TokenType.SUB:
                            result = leftValue - rightValue;
                            break;
                        case TokenType.MUL:
                            result = leftValue * rightValue;
                            break;
                        case TokenType.DIV:
                            result = leftValue / rightValue; 
                            break;
                    }

                    // Генерируем инструкцию для загрузки результата в rax

                    compiler.assemblerCode.AddInstruction($"mov rax, {result}");
                    compiler.LastUsedRegister = "rax";
                    return;
                }

                // Обрабатываем левую часть
                compiler.Visit(binaryOp.Left);

                // Если правую часть можно обработать без использования стека
                if (binaryOp.Right is AstNumber rightImmediate)
                {
                    string immediateValue = rightImmediate.Token.Value;

                    // Генерируем инструкцию напрямую
                    switch (binaryOp.Token.Type)
                    {
                        case TokenType.ADD:
                            compiler.assemblerCode.AddInstruction($"add rax, {immediateValue}");
                            break;
                        case TokenType.SUB:
                            compiler.assemblerCode.AddInstruction($"sub rax, {immediateValue}");

                            break;
                        case TokenType.MUL:
                            compiler.assemblerCode.AddInstruction($"imul rax, {immediateValue}");

                            break;
                        case TokenType.DIV:
                            compiler.assemblerCode.AddInstruction("mov rdx, 0");
                            compiler.assemblerCode.AddInstruction($"mov rbx, {immediateValue}");
                            compiler.assemblerCode.AddInstruction("div rbx");
                            break;
                    }
                    compiler.LastUsedRegister = "rax";

                    return;
                }

                compiler.assemblerCode.AddInstruction("push rax");
                compiler.Visit(binaryOp.Right);
                compiler.assemblerCode.AddInstruction("pop rbx");

                // Генерируем инструкцию для операции
                switch (binaryOp.Token.Type)
                {
                    case TokenType.ADD:
                        compiler.assemblerCode.AddInstruction("add rax, rbx");
                        break;
                    case TokenType.SUB:
                        compiler.assemblerCode.AddInstruction("sub rax, rbx");
                        break;
                    case TokenType.MUL:
                        compiler.assemblerCode.AddInstruction("imul rax, rbx");
                        break;
                    case TokenType.DIV:
                        compiler.assemblerCode.AddInstruction("mov rdx, 0");
                        compiler.assemblerCode.AddInstruction("div rbx");
                        break;
                }
                compiler.LastUsedRegister = "rax";

            }
        }

    }
    class ConstantNodeHandler : NodeHandler
    {
        public ConstantNodeHandler(AssemblyCompiler compiler) : base(compiler) { }

        public override void HandleNode(AstNode node)
        {
            if (node is AstNumber astNumber)
            {
                if (compiler.LastLoadedConstant == astNumber.Token.Value)
                {
                    return;
                }

                compiler.assemblerCode.AddInstruction($"mov rax, {astNumber.Token.Value}");
                compiler.LastLoadedConstant = astNumber.Token.Value;
            }
        }
    }

}
