﻿using System.IO;
using System.Linq;
using I2.Loc;
using UnityEngine;

namespace COM3D2.i18nEx.Core.TranslationManagers
{
    internal class I2TranslationManager : TranslationManagerBase
    {
        private readonly GameObject go = new GameObject();

        public override void LoadLanguage(string langName)
        {
            DontDestroyOnLoad(go);

            Core.Logger.LogInfo($"Loading UI translations for language \"{langName}\"");

            var tlPath = Path.Combine(Paths.TranslationsRoot, langName);
            var textTlPath = Path.Combine(tlPath, "UI");
            if (!Directory.Exists(textTlPath))
            {
                Core.Logger.LogWarning(
                    $"No UI translations folder found for language {langName}. Skipping loading script translations...");
                return;
            }

            var source = go.GetComponent<LanguageSource>() ?? go.AddComponent<LanguageSource>();
            source.name = "i18nEx";
            source.ClearAllData();
            foreach (var directory in Directory.GetDirectories(textTlPath))
            {
                var fullDir = Path.GetFullPath(directory);

                foreach (var file in Directory.GetFiles(fullDir, "*.csv", SearchOption.AllDirectories))
                {
                    var categoryName = Path.GetFullPath(file).Substring(fullDir.Length + 1).Replace(".csv", "");
                    Core.Logger.LogInfo($"Loading category {categoryName}");
                    source.Import_CSV(categoryName.Replace("\\", "/"), File.ReadAllText(file),
                        eSpreadsheetUpdateMode.Merge);
                }
            }

            Core.Logger.LogInfo(
                $"Loaded the following languages: {string.Join(",", source.mLanguages.Select(d => d.Name).ToArray())}");
            LocalizationManager.Sources.Add(source);
        }

        public override void ReloadActiveTranslations()
        {
        }
    }
}