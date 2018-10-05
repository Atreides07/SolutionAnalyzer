using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SolutionAnalyzer
{
    public class ProjectNode
    {
        public string RootProject { get; set; }
        public List<ProjectNode> ChildProjects { get; set; } = new List<ProjectNode>();
    }

    class Program
    {
        static Dictionary<string, ProjectNode> hash = new Dictionary<string, ProjectNode>();


        static void Main(string[] args)
        {
            var fileName = @"c:\test\DocumentServices.sln";
            var projectLinks = SolutionParser.ParseFile(fileName);

            var di = new FileInfo(fileName);

            foreach (var item in projectLinks)
            {
                var projectFullPath = Path.Combine(di.DirectoryName, item);
                Console.WriteLine(projectFullPath);

                AddSelectedNode("solution", projectFullPath);

                var projectFile = new FileInfo(projectFullPath);

                var subProjectLinks = ProjectParser.ParseFile(projectFullPath);
                foreach (var subProjectLink in subProjectLinks)
                {
                    var subprojectFullPath = Path.GetFullPath(Path.Combine(projectFile.DirectoryName, subProjectLink));
                    AddSelectedNode(projectFullPath, subprojectFullPath);
                    Console.WriteLine($"   {subprojectFullPath}");
                }

            }

            Console.ReadLine();
        }

        private static void AddSelectedNode(string projectFullPath, string childFile)
        {
            var rootNode = GetdNode(projectFullPath);

            var childNode = GetdNode(childFile);

            rootNode.ChildProjects.Add(childNode);
        }

        private static ProjectNode GetdNode(string file)
        {
            if (!hash.ContainsKey(file))
            {
                hash[file] = new ProjectNode()
                {
                    RootProject = file,
                    ChildProjects = new List<ProjectNode>()
                };
            }

            return hash[file];
        }
    }

    public class ProjectParser
    {
        public static IEnumerable<string> ParseFile(string projectFullPath)
        {
            var xDocument = XDocument.Load(projectFullPath);
            var subProjectReferences = xDocument.Descendants().Where(i => i.Name.LocalName == "ProjectReference");
            var files = subProjectReferences.Select(n =>
                  n.Attributes().Where(i => i.Name.LocalName == "Include").Select(i => i.Value).First());
            return files;
        }
    }
}
