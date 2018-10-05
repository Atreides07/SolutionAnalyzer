using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolutionAnalyzer
{
    public class SolutionTree
    {
        private IEnumerable<string> ParseSlnFile(string fileName)
        {
            var slnFileLines = File.ReadAllLines(fileName);
            var projectLines = slnFileLines.Where(i => i.StartsWith("Project") && i.Contains(".csproj"));
            var projectItems = projectLines.Select(i => i.Split('=')[1].Replace("\"","").Split(','));

            return projectItems.Select(i => i[1].Trim());
            //return projectItems.Select(i => new ProjectItem()
            //{
            //    ProjectName=i[0],
            //    FileName = i[1]
            //});


        }

        Dictionary<string, ProjectNode> hash = new Dictionary<string, ProjectNode>();

        string rootPath = "solution.sln";

        public ProjectNode ParseFileName(string fileName)
        {
            //AddSelectedNode(rootPath,"1");
            //return hash[rootPath];

            var projectLinks = ParseSlnFile(fileName);

            var di = new FileInfo(fileName);

            

            foreach (var item in projectLinks)
            {
                var projectFullPath = Path.Combine(di.DirectoryName, item);
                //Console.WriteLine(projectFullPath);

                AddSelectedNode(rootPath, projectFullPath);

                var projectFile = new FileInfo(projectFullPath);

                var subProjectLinks = ProjectParser.ParseFile(projectFullPath);
                foreach (var subProjectLink in subProjectLinks)
                {
                    var subprojectFullPath = Path.GetFullPath(Path.Combine(projectFile.DirectoryName, subProjectLink));
                    AddSelectedNode(projectFullPath, subprojectFullPath);
                    //Console.WriteLine($"   {subprojectFullPath}");
                }
            }

            return hash[rootPath.ToLower()];
        }

        private void AddSelectedNode(string projectFullPath, string childFile)
        {
            var rootFi=new FileInfo(projectFullPath);
            var childFi=new FileInfo(childFile);
            var rootNode = GetdNode(rootFi.Name.ToLower());
            var childNode = GetdNode(childFi.Name.ToLower());

            //var rootNode = GetdNode(projectFullPath.ToLower());
            //var childNode = GetdNode(childFile.ToLower());

            rootNode.ChildProjects.Add(childNode);
            childNode.ParentProjects.Add(rootNode);
        }

        private ProjectNode GetdNode(string file)
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

        public ProjectNode AddReference(string rootProject, string childProject)
        {
            AddSelectedNode(rootProject.ToLower(),childProject.ToLower());
            return hash[rootPath];
        }

        public Dictionary<string, ProjectNode> FlatNodes()
        {
            return hash;
        }

        public bool HasNode(string fileName)
        {
            var fi=new FileInfo(fileName);
            return hash.ContainsKey(fi.Name.ToLower());
        }
    }

    public class ProjectItem
    {
        public string FileName { get; set; }
        public string ProjectName { get; set; }
    }
}