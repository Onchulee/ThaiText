using com.dgn.ThaiText;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Lexto.Editor
{
    public class LexitronWatcher
    {
        private static FileSystemWatcher m_Watcher;
        private static bool _triggerRecompile = false;
        private static double nextSeek = 0;
        private static readonly double threadSeek = 1d;
        
        [InitializeOnLoadMethod]
        public static void OnInitialized()
        {
            bool exists = Directory.Exists(Lexitron.Path);
            if (exists) {
                InitLexitronWatcher();
            }
            else {
                try
                {
                    PackageResourceImporter importer = new PackageResourceImporter(InitLexitronWatcher);
                    importer.Import();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        private static void InitLexitronWatcher()
        {
            if (Directory.Exists(Lexitron.Path))
            {
                m_Watcher = new FileSystemWatcher(Lexitron.Path, Lexitron.FileName);
                m_Watcher.Created += Recompile;
                m_Watcher.Changed += Recompile;
                nextSeek = EditorApplication.timeSinceStartup;
                EditorApplication.update += OnEditorApplicationUpdate;
                EditorApplication.quitting += TerminateLexitronWatcher;
                m_Watcher.EnableRaisingEvents = true;
                Debug.Log("!!! Lexitron Watcher Initialized");
            }
        }

        private static void TerminateLexitronWatcher()
        {
            if (m_Watcher != null) {
                m_Watcher.EnableRaisingEvents = false;
                EditorApplication.update -= OnEditorApplicationUpdate;
                m_Watcher.Created -= Recompile;
                m_Watcher.Changed -= Recompile;
                m_Watcher.Dispose();
            }
        }

        private static void Recompile(object sender, FileSystemEventArgs e)
        {
           _triggerRecompile = true;
           // Never call any Unity classes as we are not in the main thread
        }
        
        // note that this is called very often (100/sec)
        private static void OnEditorApplicationUpdate()
        {
            if (EditorApplication.timeSinceStartup > nextSeek)
            {
                if (_triggerRecompile)
                {
                    _triggerRecompile = false;
                    Debug.Log(Lexitron.FileName + " has been changed!");
                    UpdateLexto();
                }
                nextSeek = EditorApplication.timeSinceStartup + threadSeek;
            }
        }

        private static void UpdateLexto() {
            byte[] readText = File.ReadAllBytes(Lexitron.PathFile);
            LexTo.Instance.Load(readText);
            // These part is a workaround to force redraw Editor view
            ThaiText _ThaiText = GameObject.FindObjectOfType<ThaiText>();
            if (_ThaiText)
            {
                _ThaiText.enabled = false;
                _ThaiText.enabled = true;
            }
        }
    }
}