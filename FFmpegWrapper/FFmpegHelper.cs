using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace FFmpegWrapper
{
    public static class FFmpegHelper
    {
        public static string FFmpegPath
        {
            set
            {
                ffmpeg.RootPath = value;
            }
        }

        internal unsafe static IDictionary<string, string> DictionaryConvert(AVDictionary* avDictionary)
        {
            IDictionary<string, string> metaDataDictionary = new Dictionary<string, string>();
            AVDictionaryEntry* dictEntry = null;

            do
            {
                dictEntry = ffmpeg.av_dict_get(avDictionary, string.Empty, dictEntry, ffmpeg.AV_DICT_IGNORE_SUFFIX);
                if (dictEntry != null)
                {
                    metaDataDictionary.Add(
                        Marshal.PtrToStringAnsi(new IntPtr(dictEntry->key)),
                        Marshal.PtrToStringAnsi(new IntPtr(dictEntry->value)));
                }

            } while (dictEntry != null);

            return metaDataDictionary;
        }

    }
}
