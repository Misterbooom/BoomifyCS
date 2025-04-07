using System;
using System.IO;
using System.Text;
using BoomifyCS.Assembly.BifyObject;
using BoomifyCS.Exceptions;
using LLVMSharp;
using LLVMSharp.Interop;

namespace BoomifyCS.Assembly
{
    class DebugBuilder
    {
        private LLVMDIBuilderRef diBuilder;
        private LLVMModuleRef module;
        private LLVMMetadataRef compileUnit;
        private LLVMMetadataRef file;
        private LLVMMetadataRef currentScope;

        public unsafe DebugBuilder(LLVMModuleRef module)
        {
            this.module = module;

            module.AddModuleFlag("Debug Info Version", LLVMModuleFlagBehavior.LLVMModuleFlagBehaviorWarning, (uint)3);
            module.AddModuleFlag("Dwarf Version", LLVMModuleFlagBehavior.LLVMModuleFlagBehaviorError, 4u);
            module.AddModuleFlag("PIC Level", LLVMModuleFlagBehavior.LLVMModuleFlagBehaviorError, 2u);

            diBuilder = LLVM.CreateDIBuilder(module);

            file = diBuilder.CreateFile(
                Path.GetFileName(Traceback.Instance.FilePath),
                Path.GetDirectoryName(Traceback.Instance.FilePath));

            // Создание компиляционной единицы
            compileUnit = diBuilder.CreateCompileUnit(
                LLVMDWARFSourceLanguage.LLVMDWARFSourceLanguageC,
                file,
                "BoomifyCS Compiler",
                0, 
                "", 
                0, 
                "", 
                LLVMDWARFEmissionKind.LLVMDWARFEmissionFull,
                0,
                1, 1, "", ""
            );

            currentScope = compileUnit;
        }

        public void PushLexicalScope(LLVMMetadataRef scope)
        {
            currentScope = scope;
        }



        public LLVMMetadataRef CreateFunctionDebugInfo(
            string name,
            string linkageName,
            uint line,
            LLVMMetadataRef functionType,
            int isLocal = 1,
            int isDefinition = 1)
        {
            var func = diBuilder.CreateFunction(
                currentScope,
                name,
                linkageName,
                file,
                line,
                functionType,
                isLocal,
                isDefinition,
                line,
                LLVMDIFlags.LLVMDIFlagPublic,
                0);

            currentScope = func;
            return func;
        }

        public LLVMMetadataRef CreateSubroutineType(LLVMMetadataRef[] parameterTypes)
        {
            return diBuilder.CreateSubroutineType(
                file,
                parameterTypes,
                LLVMDIFlags.LLVMDIFlagZero

                );
        }

        public unsafe LLVMMetadataRef CreateDebugLocation(uint line, uint column)
        {
            return LLVM.DIBuilderCreateDebugLocation(
                module.Context,
                line,
                column,
                currentScope,
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
                     diBuilder, sp, (uint)name.Length,
                     sizeBits, 0, LLVMDIFlags.LLVMDIFlagZero


                     );
                }
            }

        }



    }
}
