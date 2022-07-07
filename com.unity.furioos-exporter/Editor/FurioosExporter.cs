using UnityEngine;
using UnityEditor;
using System;

namespace Furioos.Exporter {
    public class FurioosExporter : EditorWindow
    {

        [MenuItem("Furioos/ App settings")]
        public static void PublishSettings()
        {
            AppSettingsForm.ShowWindow();
        }

        [MenuItem("Furioos/Run with Furioos", true)]
        public static bool ValidateRunMenu()
        {
            return FurioosSettings.isApiTokenValid && !String.IsNullOrEmpty(FsSettings.Current.ApplicationID);
        }

        [MenuItem("Furioos/Run with Furioos")]
        public static void RunToFurioos()
        {
            if (FurioosSettings.isApiTokenValid)
            {
                var buildManager = new BuildAndDeployHandler();
                buildManager.RunToFurioos();
            }
        }

    }
}
