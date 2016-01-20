using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Drawing;

namespace ConfigFile
{

    public struct fConfig
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public Color BackgroundColor;
        public Color TextColor;
        public Color TextTranslateColor;
        public int TimeText;
        public int TimeTextTranslate;
        public string DictonaryPath;
        public Font TextFont;
        public Font TextTranslateFont;
   
    }

    class Config
    {
        private string configFile;

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color BackgroundColor { get; set; }
        public Color TextColor { get; set; }
        public Color TextTranslateColor { get; set; }
        public int TimeText { get; set; }
        public int TimeTextTranslate { get; set; }
        public string DictonaryPath { get; set; }
        public Font TextFont { get; set; }
        public Font TextTranslateFont { get; set; }
        
        public Config(string file)
        {
            configFile = file;

            X = 100;
            Y = 100;
            Width = 200;
            Height = 100;
            BackgroundColor = Color.Yellow;
            TextColor = Color.Black;
            TextTranslateColor = Color.Black;
            TimeText = 1000;
            TimeTextTranslate = 1000;
            DictonaryPath = "";
            TextFont = new Font(new FontFamily("Times New Roman"), 16, FontStyle.Regular, GraphicsUnit.Pixel);
            TextTranslateFont = new Font(new FontFamily("Times New Roman"), 14, FontStyle.Regular, GraphicsUnit.Pixel);

            try
            {

                XmlSerializer deserializer = new XmlSerializer(typeof(fConfig));
                StreamReader reader = new StreamReader(configFile);
                fConfig config;
                config = (fConfig)deserializer.Deserialize(reader);
                reader.Close();

                X = config.X;
                Y = config.Y;
                Width = config.Width;
                Height = config.Height;
                BackgroundColor = config.BackgroundColor;
                TextColor = config.TextColor;
                TextTranslateColor = config.TextTranslateColor;
                TimeText = config.TimeText;
                TimeTextTranslate = config.TimeTextTranslate;
                DictonaryPath = config.DictonaryPath;
                TextFont = config.TextFont;
                TextTranslateFont = config.TextTranslateFont;      
            }
            catch (Exception ext)
            {
                System.Diagnostics.Debug.WriteLine(ext.Message);
            }

        }

    

        public void saveConfig()
        {
            try
            {

                fConfig config = new fConfig();

                config.X = X;
                config.Y = Y;
                config.Width = Width;
                config.Height = Height;
                config.BackgroundColor = BackgroundColor;
                config.TextColor = TextColor;
                config.TextTranslateColor = TextTranslateColor;
                config.TimeText = TimeText;
                config.TimeTextTranslate = TimeTextTranslate;
                config.DictonaryPath = DictonaryPath;
                config.TextFont = TextFont;
                config.TextTranslateFont = TextTranslateFont;   

                XmlSerializer ser = new XmlSerializer(typeof(fConfig));
                StreamWriter writer = new StreamWriter(configFile);
                ser.Serialize(writer, config);
                writer.Close();
            }
            catch (Exception ext)
            {
                System.Diagnostics.Debug.WriteLine(ext.Message);
            }
        }

    }
}
