#if UNITY_IOS

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace LinqUnity.Editor {

  public static class LinqEditor {

    private const string KountFrameworkName = "KountDataCollector.xcframework";
    
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
      string pbxProjectPath = PBXProject.GetPBXProjectPath(buildPath);

      PBXProject project = new PBXProject();
      project.ReadFromFile(pbxProjectPath);

      string targetGuid = project.GetUnityMainTargetGuid();

      string sectionGuid = project.GetFrameworksBuildPhaseByTarget(targetGuid);

      var source = Path.Combine("Pods", "Kount", "xcframeworks", KountFrameworkName);
      var framework = project.AddFile(source, source);

      project.AddFileToBuildSection(targetGuid, sectionGuid, framework);
      
      project.WriteToFile(pbxProjectPath);
    }

  }
}

#endif
