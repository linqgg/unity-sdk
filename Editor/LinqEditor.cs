#if UNITY_IOS

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

namespace LinqUnity.Editor {

  public partial class LinqEditor {

    private const string KOUNT_FRAMEWORK_NAME = "KountDataCollector.xcframework";
    
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
      string pbxProjectPath = PBXProject.GetPBXProjectPath(buildPath);

      PBXProject project = new PBXProject();
      project.ReadFromFile(pbxProjectPath);

      string targetGuid = project.GetUnityMainTargetGuid();

      string sectionGuid = project.GetFrameworksBuildPhaseByTarget(targetGuid);

      var source = Path.Combine("Pods", "Kount", "xcframeworks", KOUNT_FRAMEWORK_NAME);
      var framework = project.AddFile(source, source);

      project.AddFileToBuildSection(targetGuid, sectionGuid, framework);
      
      project.WriteToFile(pbxProjectPath);
    }

  }
}

#endif
