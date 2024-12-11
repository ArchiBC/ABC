using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;

namespace ABC_Rhino;

[CommandStyle(Style.ScriptRunner)]
public class ABC_EngCommandSearch:Command
{
    public ABC_EngCommandSearch()
    {
        // Rhino only creates one instance of each command class defined in a
        // plug-in, so it is safe to store a refence in a static property.
        Instance = this;
    }

    ///<summary>The only instance of this command.</summary>
    public static ABC_EngCommandSearch Instance { get; private set; }

    ///<returns>The command name as it appears on the Rhino command line.</returns>
    public override string EnglishName => this.GetType().Name;

    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
        var pluginPath = Rhino.PlugIns.PlugIn.PathFromId(ABC_RhinoPlugin.Instance.Id);
        var pluginFolderPath = Path.GetDirectoryName(pluginPath);
        
        var searchStr = "";
        var result = RhinoGet.GetString("输入中文命令",false, ref searchStr);

        if (!Directory.Exists(pluginFolderPath))
        {
            RhinoApp.WriteLine("插件目录不存在");
            return Result.Failure;
        }

        var ruiFiles = Directory.GetFiles(pluginFolderPath, "*.rui", SearchOption.AllDirectories);
        
        var xElementList =new List<XElement>();
        foreach (var filePath in ruiFiles)
        {
            var xdoc = XDocument.Load(filePath);
            var elements = xdoc.Descendants("macro_item")
                .Where(e => e.Element("text")?
                    .Element("locale_2052")?
                    .Value.Contains(searchStr) == true);
            xElementList.AddRange(elements);
        }

        var count = 0;
        foreach (var xElement in xElementList)
        {
            var text = xElement.Element("text")?.Element("locale_2052")?.Value;
            var script = xElement.Element("script")?.Value;
            RhinoApp.WriteLine($"[{count}]{text} {script}");
            count++;
        }
        
        var index = 0;
        var result2 = RhinoGet.GetInteger("选择第几个命令",false, ref index);
        
        Rhino.RhinoApp.RunScript(xElementList[index].Element("script")?.Value, false);
        
        return Result.Success;
    }
}