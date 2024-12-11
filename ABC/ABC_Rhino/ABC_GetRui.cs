using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;

namespace ABC_Rhino
{
    /// <summary>
    /// 从Rhino文件获得当前rhino载入的RUI文件到插件路径
    /// </summary>
    public class ABC_GetRui : Command
    {
        public ABC_GetRui()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static ABC_GetRui Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => this.GetType().Name;

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var pluginPath = Rhino.PlugIns.PlugIn.PathFromId(ABC_RhinoPlugin.Instance.Id);
            var pluginFolderPath = Path.GetDirectoryName(pluginPath);

            foreach (var ruifile in RhinoApp.ToolbarFiles)
            {
                if (ruifile.SaveAs(pluginFolderPath+"/"+ruifile.Name))
                {
                    RhinoApp.WriteLine(" {0}.rui 已保存。", ruifile.Name);
                }
                else
                {
                    RhinoApp.WriteLine(" {0}.rui 保存失败", ruifile.Name);
                }
                
            }
            
            return Result.Success;
        }
    }
}