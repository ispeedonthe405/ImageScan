using System;
using System.Diagnostics;
using Tesseract;

namespace ImageScan.Core
{
    internal static class OCR
    {
        static TesseractEngine engine;
        static OCR()
        {
            engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
        }

        public static void Shutdown()
        {
            if (!engine.IsDisposed)
            {
                engine.Dispose();
            }
        }

        public static void SetLanguage(string languageId)
        {
            try
            {
                if (!engine.IsDisposed)
                {
                    engine.Dispose();
                }
                engine = new TesseractEngine(@"./tessdata", languageId, EngineMode.Default);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }


        public static string ReadText(string path)
        {
            try
            {
                using (var img = Pix.LoadFromFile(path))
                {
                    using (var page = engine.Process(img))
                    {
                        return page.GetText();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public static string ParseByLine(string path)
        {
            try
            {
                string output = string.Empty;
                using (var img = Pix.LoadFromFile(path))
                {
                    using (var page = engine.Process(img, PageSegMode.Auto))
                    {
                        var pgLevel = PageIteratorLevel.TextLine;
                        using(var it = page.GetIterator())
                        {
                            it.Begin();
                            do
                            {
                                string line = it.GetText(pgLevel);
                                if (!string.IsNullOrWhiteSpace(line))
                                {
                                    output += line + Environment.NewLine;
                                }
                                //output += it.GetText(pgLevel) + Environment.NewLine;
                            } while (it.Next(pgLevel));
                        }
                    }
                }
                return output;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return string.Empty;
            }
        }
    }
}
