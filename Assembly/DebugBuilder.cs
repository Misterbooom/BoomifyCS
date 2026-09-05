using System.IO;
using System.Text;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Exceptions;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly
{
    class DebugBuilder
    {
        private readonly LLVMDIBuilderRef _diBuilder;
        private readonly LLVMModuleRef _module;
        private readonly LLVMMetadataRef _compileUnit;
        private readonly LLVMMetadataRef _file;
        private LLVMMetadataRef _currentScope;

        public unsafe DebugBuilder(LLVMModuleRef module)
        {
            this._module = module;

            module.AddModuleFlag("Debug Info Version", LLVMModuleFlagBehavior.LLVMModuleFlagBehaviorWarning, (uint)3);
            module.AddModuleFlag("Dwarf Version", LLVMModuleFlagBehavior.LLVMModuleFlagBehaviorError, 4u);
            module.AddModuleFlag("PIC Level", LLVMModuleFlagBehavior.LLVMModuleFlagBehaviorError, 2u);

            _diBuilder = LLVM.CreateDIBuilder(module);

            _file = _diBuilder.CreateFile(
                Path.GetFileName(Traceback.Instance.FilePath),
                Path.GetDirectoryName(Traceback.Instance.FilePath));

            // Создание компиляционной единицы
            _compileUnit = _diBuilder.CreateCompileUnit(
                LLVMDWARFSourceLanguage.LLVMDWARFSourceLanguageC,
                _file,
                "BoomifyCS Compiler",
                0, 
                "", 
                0, 
                "", 
                LLVMDWARFEmissionKind.LLVMDWARFEmissionFull,
                0,
                1, 1, "", ""
            );

            _currentScope = _compileUnit;
        }

        public void PushLexicalScope(LLVMMetadataRef scope)
        {
            _currentScope = scope;
        }



        public LLVMMetadataRef CreateFunctionDebugInfo(
            string name,
            string linkageName,
            uint line,
            LLVMMetadataRef functionType,
            int isLocal = 1,
            int isDefinition = 1)
        {
            var func = _diBuilder.CreateFunction(
                _currentScope,
                name,
                linkageName,
                _file,
                line,
                functionType,
                isLocal,
                isDefinition,
                line,
                LLVMDIFlags.LLVMDIFlagPublic,
                0);

            _currentScope = func;
            return func;
        }

        public LLVMMetadataRef CreateSubroutineType(LLVMMetadataRef[] parameterTypes)
        {
            return _diBuilder.CreateSubroutineType(
                _file,
                parameterTypes,
                LLVMDIFlags.LLVMDIFlagZero

                );
        }

        public unsafe LLVMMetadataRef CreateDebugLocation(uint line, uint column)
        {
            return LLVM.DIBuilderCreateDebugLocation(
                _module.Context,
                line,
                column,
                _currentScope,
                null);
        }
        public LLVMMetadataRef[] CreateParametersType(BifyType[] type)
        {
            LLVMMetadataRef[] types = new LLVMMetadataRef[type.Length];
            for (int i = 0; i < type.Length; i++)
            {
                types[i] = CreateBasicType(type[i].Name, type[i].Size() * 8);
            }
            return types;
        }

        public LLVMMetadataRef CreateBasicType(string name, ulong sizeBits)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(name);


            unsafe
            {
                fixed (byte* p = bytes)
                {
                    sbyte* sp = (sbyte*)p;
                    return LLVM.DIBuilderCreateBasicType(
                     _diBuilder, sp, (uint)name.Length,
                     sizeBits, 0, LLVMDIFlags.LLVMDIFlagZero


                     );
                }
            }

        }



    }
}
