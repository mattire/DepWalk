using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DepWalk
{

    /// <summary>
    /// For checking which assemblies in one folder are referenced by the given assembly
    /// Usage: DepWalk AssemblyName
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1) {
                Console.WriteLine("Usage: DepWalk <AssemblyName>");
                return;
            }
            string path = Path.Combine(Directory.GetCurrentDirectory(), args[0]);

            var dir = Path.GetDirectoryName(path);
            var name = Path.GetFileName(path);
            var fls = Directory.GetFiles(dir).Select(p=>Path.GetFileName(p)).ToList(); 

            var ns = GetRefAsmNames(dir, name, fls);
            foreach (var n in ns) { Console.WriteLine(n); }
        }

        public static List<string> GetRefAsmNames(string path, string name, List<string> files) {
            if (!files.Contains(name)) { return new List<string>(); }
            Assembly asm = Assembly.ReflectionOnlyLoadFrom(Path.Combine(path, name));
            var refNames = asm.GetReferencedAssemblies();
            var rns = refNames.Select(rn => rn.Name).ToList();
            var recusives = new List<string>();
            foreach (var rn in rns)
            {
                recusives.AddRange( GetRefAsmNames(path, rn, files));
            }
            rns.AddRange(recusives);
            return rns;
        }
    }
}
