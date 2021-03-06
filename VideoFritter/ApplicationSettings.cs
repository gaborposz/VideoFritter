﻿using System;
using System.IO;
using System.Text.Json;

namespace VideoFritter
{
    internal static class ApplicationSettings
    {
        static ApplicationSettings()
        {
            if (File.Exists(SettingsFileName))
            {
                using (FileStream fileStream =
                    new FileStream(SettingsFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);

                    settingsData = JsonSerializer.Deserialize<SettingsData>(buffer);
                }
            }
            else
            {
                // Use default settings if the settings file is not available
                ExportQueuePath = @"$(VideoPath)\Export";
                TimeStampCorrection = true;
                AudioVolume = 0.5;
            }
        }

        public static string ExportQueuePath
        {
            get
            {
                return settingsData.ExportQueuePath;
            }

            set
            {
                settingsData.ExportQueuePath = value;
            }
        }

        public static bool TimeStampCorrection
        {
            get
            {
                return settingsData.TimeStampCorrection;
            }

            set
            {
                settingsData.TimeStampCorrection = value;
            }
        }

        public static double AudioVolume
        {
            get
            {
                return settingsData.AudioVolume;
            }

            set
            {
                settingsData.AudioVolume = value;
            }
        }

        public static void Save()
        {
            if (!Directory.Exists(SettingsFilePath))
            {
                Directory.CreateDirectory(SettingsFilePath);
            }

            using (FileStream fileStream =
                new FileStream(SettingsFileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                using (Utf8JsonWriter jsonWriter = new Utf8JsonWriter(fileStream, new JsonWriterOptions { Indented = true }))
                {
                    JsonSerializer.Serialize(jsonWriter, settingsData);
                }
            }
        }

        private static string SettingsFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VideoFritter");

        private static string SettingsFileName =
            Path.Combine(SettingsFilePath, "VideoFritter.settings.json");

        private struct SettingsData
        {
            public string ExportQueuePath { get; set; }

            public bool TimeStampCorrection { get; set; }

            public double AudioVolume { get; set; }
        }

        private static SettingsData settingsData;
    }
}
