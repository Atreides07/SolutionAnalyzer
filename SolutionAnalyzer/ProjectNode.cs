using System.Collections.Generic;

namespace SolutionAnalyzer
{
    public class ProjectNode
    {
        public string RootProject { get; set; }
        public List<ProjectNode> ChildProjects { get; set; } = new List<ProjectNode>();

        public List<ProjectNode> ParentProjects { get; set; }=new List<ProjectNode>();
    }
}