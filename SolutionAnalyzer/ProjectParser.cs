using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SolutionAnalyzer
{
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