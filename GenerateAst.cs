using System;
using System.Collections.Generic;
using System.IO;

namespace Metaprogramming {
    public class GenerateAst {
        public static void Main(string[] args) {
            if(args.Length != 1) {
                Console.WriteLine("Usage: generate_ast <output directory>");
                Environment.Exit(64);
            }

            string outputDir = args[0];
            defineAst(outputDir, "Expr", new string[] {
                "Binary   : Expr left, Token operator, Expr right",
                "Grouping : Expr expression",
                "Literal  : Object value",
                "Unary    : Token operator, Expr right"
            });
        }

        private static void defineAst(string outputPath, string baseClassName, string[] types) {
            string path = outputPath + "/" + baseClassName + ".cs";

            using(StreamWriter writer = new StreamWriter(path)) {

                writer.WriteLine("using System;");
                writer.WriteLine();
                writer.WriteLine("namespace SyntaxTree {");
                writer.WriteLine("  public class " + baseClassName + "{");

                /*
                    The AST classes.
                */
                foreach(string type in types) {
                    string className = type.Split(":")[0].Trim();
                    string fields = type.Split(":")[1].Trim();
                    defineType(writer, baseClassName, className, fields);
                }

                writer.WriteLine();
                writer.WriteLine("  abstract <R> R accept(Visitor<R> visitor);");


                writer.WriteLine("}");


               
                writer.WriteLine();
                writer.Close();

            }


        }

        private static void defineType(StreamWriter writer, string baseClassName, string className, string fieldList) {


        


            writer.WriteLine(" static class " + className + " extends " + baseClassName + " {");

            // constructor
            writer.WriteLine("      " + className + "(" + fieldList + " ) {");

            // store parameter in fields
            string[] fields = fieldList.Split(", ");
            foreach(string field in fields) {
                string name = field.Split(" ")[1];
                writer.WriteLine("        this." + name + "=" + name+ ";");
            }

            writer.WriteLine("      }");


             writer.WriteLine();
            writer.WriteLine("  @Override");
            writer.WriteLine("  <R> R accept(Visitor<R> visitor) {");
            writer.WriteLine("      return visitor.visit" + baseClassName + className + "(this);");

            writer.WriteLine("      ");

            // Fields
            writer.WriteLine();

            foreach(string field in fields) {
                writer.WriteLine("  final " + field + ";");
            }


            writer.WriteLine("}");
        }
    
        private static void defineVistor(StreamWriter writer, string baseName, string[] types) {

            writer.WriteLine("  interface Visitor<R> {");

            foreach(string type in types) {
                string typeName = type.Split(":")[0].Trim();
                writer.WriteLine("  R visit" + typeName + baseName + "(" + 
                typeName + " " + baseName.ToLower() + ");");
            }

            writer.WriteLine("  }");
        }
    }
}