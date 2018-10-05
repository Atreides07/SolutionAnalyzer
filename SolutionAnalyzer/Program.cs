using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace SolutionAnalyzer
{
    class Program
    {


        static void Main(string[] args)
        {
            //var fileName = @"c:\Projects\Lingvo2\DocumentServices.sln";
            var fileName = @"C:\temp\2\Solution1\Solution1.sln";
            var tree=new SolutionTree();
            tree.ParseFileName(fileName);

            //var n1=tree.HasNode(@"C:\Projects\Lingvo2\DocumentServices\NamedEntityDetector\NamedEntityDetector.csproj");
            //var n2=tree.HasNode(@"C:\Projects\Lingvo2\DocumentServices\PositiveNegative\FactExtractor.csproj");

            //tree.AddReference(
            //    @"C:\Projects\Lingvo2\DocumentServices\NamedEntityDetector\NamedEntityDetector.csproj",
            //    @"C:\Projects\Lingvo2\DocumentServices\PositiveNegative\FactExtractor.csproj");


            var flatNodes=tree.FlatNodes();

            bool hasCycle = false;
            while (!hasCycle && flatNodes.Any())
            {
                bool nodesWithoutReferences = false;
                foreach (var node in flatNodes)
                {
                    if (!node.Value.ParentProjects.Any())
                    {
                        nodesWithoutReferences = true;
                        RemoveNode(node.Value);
                        flatNodes.Remove(node.Key);
                        break;
                    }

                    if (!node.Value.ChildProjects.Any())
                    {
                        nodesWithoutReferences = true;
                        RemoveNode(node.Value);
                        flatNodes.Remove(node.Key);
                        break;
                    }
                }

                if (!nodesWithoutReferences)
                {
                    hasCycle = true;
                }
            }

            if (!flatNodes.Any())
            {
                Console.WriteLine("Циклы не обнаружены");
            }

            foreach (var projectNode in flatNodes)
            {
                Console.WriteLine(projectNode.Value.RootProject);
            }

            Console.ReadLine();
        }

        private static void RemoveNode(ProjectNode nodeValue)
        {
            foreach (var parent in nodeValue.ParentProjects)
            {
                parent.ChildProjects.Remove(nodeValue);
            }

            foreach (var parent in nodeValue.ChildProjects)
            {
                parent.ParentProjects.Remove(parent);
            }
        }
    }
}