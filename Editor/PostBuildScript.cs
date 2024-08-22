using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class PostBuildIosScript
{
  [PostProcessBuild(1)]
  public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
  {
#if UNITY_IOS
    Debug.Log("Initializing ApplePay capabilities for LinQ Unity SDK");

    var path = PBXProject.GetPBXProjectPath(pathToBuiltProject);
    PBXProject project = new PBXProject();
    project.ReadFromString(File.ReadAllText(path));

    string targetGuid = project.GetUnityMainTargetGuid();

    project.AddCapability(targetGuid, PBXCapabilityType.ApplePay);

    File.WriteAllText(path, project.WriteToString());

    var manager = new ProjectCapabilityManager(path, "Entitlements.entitlements", null, targetGuid);

    manager.AddApplePay(new string[]
    {
      "merchant.games.galactica.linq-test",
      "merchant.games.galactica.linq-2-test",
      "merchant.games.galactica.linq-3-test",
      "merchant.games.galactica.linq",
      "merchant.games.galactica.linq-2",
      "merchant.games.galactica.linq-3",
    });
    
    manager.AddKeychainSharing(new string[]
    {
      "$(AppIdentifierPrefix)games.galactica.linq.stg.shared",
      "$(AppIdentifierPrefix)games.galactica.linq.shared"
    });

    manager.WriteToFile();
#endif
    }
}
