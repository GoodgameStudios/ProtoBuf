using System;
using System.IO;
using System.Text;
using System.Reflection;
using SilentOrbit.Code;

namespace SilentOrbit.ProtocolBuffers
{
    /// <summary>
    ///   Generated classes are saved to single files (one class for each proto) instead of ending all in one big file
    /// </summary>
    internal static class ProtoCodeSplitted
    {

        private static readonly string ENUMS_PATH = "enums/";
        private static readonly string CLASSES_PATH = "protoClasses/";
        private static readonly string SERIALIZERS_PATH = "serializer/";

        /// <summary>
        /// Generate code for reading and writing protocol buffer messages
        /// </summary>
        public static void Save(ProtoCollection file, Options options)
        {

            CodeWriter.DefaultIndentPrefix = options.UseTabs ? "\t" : "    ";

            string csDir = options.OutputPath;

            //Basic structures
            string codeDir = Path.Combine(csDir, CLASSES_PATH);
            if (Directory.Exists(codeDir) == false)
            {
                Directory.CreateDirectory(codeDir);
            }
            string codePath = null;

            foreach (ProtoMessage m in file.Messages.Values)
            {
                codePath = Path.Combine(codeDir, m.CsType + ".Code.cs");
                Console.WriteLine("Generating code " + codePath);
                using (var cw = new CodeWriter(codePath))
                {
                    WriteHeaderCode(cw);
                    cw.Bracket("namespace " + m.CsNamespace);
                    var messageCode = new MessageCode(cw, options);
                    messageCode.GenerateClass(m);
                    cw.EndBracket();
                }
            }

            // Enums
            string enumDir = Path.Combine(csDir, ENUMS_PATH);
            if (Directory.Exists(enumDir) == false)
            {
                Directory.CreateDirectory(enumDir);
            }
            string enumPath = null;
            foreach (ProtoEnum e in file.Enums.Values)
            {
                enumPath = Path.Combine(enumDir, e.CsType + ".Enum.cs");
                Console.WriteLine("Generating enums " + enumPath);
                using (var cw = new CodeWriter(enumPath))
                {
                    cw.Bracket("namespace " + e.CsNamespace);
                    var messageCode = new MessageCode(cw, options);
                    messageCode.GenerateEnum(e);
                    cw.EndBracket();
                }
            }

            //.Serializer.cs  //Code for Reading/Writing 
            string serializerDir = Path.Combine(csDir, SERIALIZERS_PATH);
            if (Directory.Exists(serializerDir) == false)
            {
                Directory.CreateDirectory(serializerDir);
            }
            string serializerPath = null;
            foreach (ProtoMessage m in file.Messages.Values)
            {
                serializerPath = Path.Combine(serializerDir, m.CsType + ".Serializer.cs");
                Console.WriteLine("Generating serializer " + serializerPath);
                using (var cw = new CodeWriter(serializerPath))
                {
                    WriteHeaderSerializer(cw);
                    cw.Bracket("namespace " + m.CsNamespace);
                    var messageSerializer = new MessageSerializer(cw, options);
                    messageSerializer.GenerateClassSerializer(m);
                    cw.EndBracket();
                }
            }

            //Option to no include ProtocolParser.cs in the output directory
            if (options.NoProtocolParser)
            {
                Console.Error.WriteLine("ProtocolParser.cs not (re)generated");
            }
            else
            {
                string libPath = Path.Combine(Path.GetDirectoryName(csDir), "ProtocolParser.cs");
                Assembly assembly = Assembly.GetExecutingAssembly();
                string[] result = assembly.GetManifestResourceNames();
                Console.WriteLine("Assembly resources:");
                foreach (string s in result)
                {
                    Console.WriteLine("--> " + s);
                }
                Console.WriteLine("----------");
                using (TextWriter codeWriter = new StreamWriter(libPath, false, Encoding.UTF8))
                {
                    codeWriter.NewLine = "\r\n";
                  
                    ReadCode(codeWriter, "ProtocolParser", true);
                    ReadCode(codeWriter, "ProtocolParserExceptions", false);
                    ReadCode(codeWriter, "ProtocolParserFixed", false);
                    ReadCode(codeWriter, "ProtocolParserKey", false);
                    ReadCode(codeWriter, "ProtocolParserMemory", false);
                    if (options.Net2 == false)
                        ReadCode(codeWriter, "ProtocolParserMemory4", false);
                    ReadCode(codeWriter, "ProtocolParserVarInt", false);  
                    ReadCode(codeWriter, "IProtoBuf", false);
                }
            }
        }

        /// <summary>
        /// Read c# code from sourcePath and write it on code without the initial using statements.
        /// </summary>
        private static void ReadCode(TextWriter code, string name, bool includeUsing)
        {
            code.WriteLine("#region " + name);
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
            if (stream == null)
            {
                Console.WriteLine("could not find stream in assembly for " + name);
                code.WriteLine("#endregion");
                return;
            }
            using (
                TextReader tr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(name),
                    Encoding.UTF8))
            {
                while (true)
                {
                    string line = tr.ReadLine();
                    if (line == null)
                        break;
                    if (includeUsing == false && line.StartsWith("using"))
                        continue;

                    if (CodeWriter.DefaultIndentPrefix == "\t")
                        line = line.Replace("    ", "\t");
                    code.WriteLine(line);
                }
            }
            code.WriteLine("#endregion");
        }



        private static void WriteHeaderSerializer(CodeWriter cw)
        {
            cw.Comment(@"This is the backend code for reading and writing");
            cw.WriteLine();
            cw.Comment(@"Generated by ProtocolBuffer  (fork to output single files for each message and using IProtoBuf)
- a pure c# code generation implementation of protocol buffers
Report bugs to: https://silentorbit.com/protobuf/");
            cw.WriteLine();
            cw.Comment(@"DO NOT EDIT
This file will be overwritten when CodeGenerator is run.");

            cw.WriteLine("using System;");
            cw.WriteLine("using System.IO;");
            cw.WriteLine("using System.Text;");
            cw.WriteLine("using System.Collections.Generic;");
            cw.WriteLine();
        }

        private static void WriteHeaderCode(CodeWriter cw)
        {
            cw.Comment(@"Classes and structures being serialized");
            cw.WriteLine();
            cw.Comment(@"Generated by ProtocolBuffer  (fork to output single files for each message and using IProtoBuf)
- a pure c# code generation implementation of protocol buffers
Report bugs to: https://silentorbit.com/protobuf/");
            cw.WriteLine();
            cw.Comment(@"DO NOT EDIT
This file will be overwritten when CodeGenerator is run.
To make custom modifications, edit the .proto file and add //:external before the message line
then write the code and the changes in a separate file.");

            cw.WriteLine("using System;");
            cw.WriteLine("using System.Collections.Generic;");
            cw.WriteLine();
        }
    }
}

