#if UNITY_IOS

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

namespace LinqUnity.Editor {

  public partial class LinqEditor {

    private const string KOUNT_FRAMEWORK_NAME = "KountDataCollector.xcframework";
    
    [PostProcessBuild(1000)]
    public static void OnPostProcessBuild(string path)
    {
      string pbxProjectPath = PBXProject.GetPBXProjectPath(path);

      PBXProject project = new PBXProject();
      project.ReadFromFile(pbxProjectPath);

      string targetGuid = project.GetUnityMainTargetGuid();

      var source = Path.Combine("Pods", "Kount", "xcframeworks", KOUNT_FRAMEWORK_NAME);
      
      var framework = project.AddFile(source, source);

      // project.AddFileToBuild(mainTargetGuid, framework);
      project.AddFileToEmbedFrameworks(targetGuid, framework);
      
      project.WriteToFile(pbxProjectPath);
    }

  }
}

#endif