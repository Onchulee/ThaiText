#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

namespace com.dgn.ThaiText
{
    [System.Serializable]
    public class PackageResourceImporter
    {

        public static string PackageFullPathFile {
            get {
                return GetPackageFullPath() + EssentialResources.PathFile;
            }
        }

        bool m_EssentialResourcesImported = false;

        public delegate void DelegateImportCompleted();
        DelegateImportCompleted OnImportCompleted;

        public PackageResourceImporter()
        {
            OnImportCompleted = delegate { };
        }

        public PackageResourceImporter(DelegateImportCompleted onImportCompleted) {
            OnImportCompleted = onImportCompleted;
        }

        public void Import()
        {
            // Check if the resources state has changed.
            m_EssentialResourcesImported = File.Exists(EssentialResources.PathFile);
            if (!m_EssentialResourcesImported)
            {
                AssignCallback();
                AssetDatabase.ImportPackage(PackageFullPathFile, false);
            }
        }
        
        /// <summary>
        ///
        /// </summary>
        /// <param name="packageName"></param>
        void ImportSuccess(string packageName)
        {
            if (packageName == "ThaiText Essential Resources")
            {
                m_EssentialResourcesImported = true;
#if UNITY_2018_3_OR_NEWER
                SettingsService.NotifySettingsProviderChanged();
#endif
                if (OnImportCompleted!=null) OnImportCompleted.Invoke();
            }

            Debug.Log("[" + packageName + "] have been imported.");
            UnassignCallback();
        }

        void ImportFailed(string packageName, string errorMessage)
        {
            string err_msg = " !!! Error: cannot import package: " + packageName;
            err_msg += "\n Error import message: " + errorMessage;
            Debug.LogError(err_msg);
            UnassignCallback();
        }

        void AssignCallback() {
            AssetDatabase.importPackageCompleted += ImportSuccess;
            AssetDatabase.importPackageFailed += ImportFailed;
        }

        void UnassignCallback() {

            AssetDatabase.importPackageCompleted -= ImportSuccess;
            AssetDatabase.importPackageFailed -= ImportFailed;
        }

        public static string GetPackageFullPath()
        {
            // Check for potential UPM package
            string packagePath = Path.GetFullPath("Packages/com.dgn.ThaiText");
            if (Directory.Exists(packagePath))
            {
                return packagePath;
            }

            packagePath = Path.GetFullPath("Assets/..");
            if (Directory.Exists(packagePath))
            {
                // Search default location for development package
                if (Directory.Exists(packagePath + "/Assets/Packages/com.dgn.ThaiText/Package Resources"))
                {
                    return packagePath + "/Assets/Packages/com.dgn.ThaiText";
                }

                // Search for default location of normal TextMesh Pro AssetStore package
                if (Directory.Exists(packagePath + "/Assets/DGN/ThaiText/Package Resources"))
                {
                    return packagePath + "/Assets/DGN/ThaiText";
                }

                // Search for potential alternative locations in the user project
                string[] matchingPaths = Directory.GetDirectories(packagePath, "ThaiText", SearchOption.AllDirectories);
                string path = ValidateLocation(matchingPaths, packagePath);
                if (path != null) return packagePath + path;
            }

            return null;
        }

        static string ValidateLocation(string[] paths, string projectPath)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                // Check if the Editor Resources folder exists.
                if (Directory.Exists(paths[i] + "/Package Resources"))
                {
                    string folderPath = paths[i].Replace(projectPath, "");
                    folderPath = folderPath.TrimStart('\\', '/');
                    return folderPath;
                }
            }
            return null;
        }
    }
}
#endif