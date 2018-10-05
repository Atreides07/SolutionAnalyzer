using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolutionAnalyzer
{
    internal class SolutionParser
    {
        internal static IEnumerable<string> ParseFile(string fileName)
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
    }

    public class ProjectItem
    {
        public string FileName { get; set; }
        public string ProjectName { get; set; }
    }
}