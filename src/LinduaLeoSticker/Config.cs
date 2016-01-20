using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Drawing;
using System.ComponentModel;

namespace ConfigFile
{

    public struct ParamList
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int BackgroundColor;
        public int TextColor;
        public int TextTranslateColor;
        public int TimeText;
        public int TimeTextTranslate;
        public string DictonaryPath;
        public string TextFont;
        public string TextTranslateFont;
   
    }

    class Config
    {
        private string FileName;

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


        [XmlElement("BackgroundColor")]
        public int BackgroundColorConvert
        {
            get { return BackgroundColor.ToArgb(); }
            set { BackgroundColor = Color.FromArgb(value); }
        }

        [XmlElement("TextColor")]
        public int TextColorConvert
        {
            get { return TextColor.ToArgb(); }
            set { TextColor = Color.FromArgb(value); }
        }

        [XmlElement("TextTranslateColor")]
        public int TextTranslateColorConvert
        {
            get { return TextTranslateColor.ToArgb(); }
            set { TextTranslateColor = Color.FromArgb(value); }
        }

        [XmlElement("TextTranslateFont")]
        public string TextTranslateFontConvert
        {
            get { return FontXmlConverter.ConvertToString(TextTranslateFont); }
            set { TextTranslateFont = FontXmlConverter.ConvertToFont(value); }
        }

        [XmlElement("TextFont")]
        public string TextFontConvert
        {
            get { return FontXmlConverter.ConvertToString(TextFont); }
            set { TextFont = FontXmlConverter.ConvertToFont(value); }
        }

        public Config(string file)
        {
            FileName = file;

            X = 100;
            Y = 100;
            Width = 200;
            Height = 100;
            BackgroundColor = Color.Yellow;
            TextColor = Color.Black;
            TextTranslateColor = Color.Black;
            TimeText = 2000;
            TimeTextTranslate = 2000;
            DictonaryPath = "default.txt";
            TextFont = new Font(new FontFamily("Courier New"), 20, FontStyle.Regular, GraphicsUnit.Pixel);
            TextTranslateFont = new Font(new FontFamily("Courier New"), 18, FontStyle.Regular, GraphicsUnit.Pixel);

            try
            {

                XmlSerializer deserializer = new XmlSerializer(typeof(ParamList));
                StreamReader reader = new StreamReader(FileName);
                ParamList config;
                config = (ParamList)deserializer.Deserialize(reader);
                reader.Close();

                X = config.X;
                Y = config.Y;
                Width = config.Width;
                Height = config.Height;
                BackgroundColorConvert = config.BackgroundColor;
                TextColorConvert = config.TextColor;
                TextTranslateColorConvert = config.TextTranslateColor;
                TimeText = config.TimeText;
                TimeTextTranslate = config.TimeTextTranslate;
                DictonaryPath = config.DictonaryPath;
                TextFontConvert = config.TextFont;
                TextTranslateFontConvert = config.TextTranslateFont;      
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

                ParamList config = new ParamList();

                config.X = X;
                config.Y = Y;
                config.Width = Width;
                config.Height = Height;
                config.BackgroundColor = BackgroundColorConvert;
                config.TextColor = TextColorConvert;
                config.TextTranslateColor = TextTranslateColorConvert;
                config.TimeText = TimeText;
                config.TimeTextTranslate = TimeTextTranslate;
                config.DictonaryPath = DictonaryPath;
                config.TextFont = TextFontConvert;
                config.TextTranslateFont = TextTranslateFontConvert;   

                XmlSerializer ser = new XmlSerializer(typeof(ParamList));
                StreamWriter writer = new StreamWriter(FileName);
                ser.Serialize(writer, config);
                writer.Close();
            }
            catch (Exception ext)
            {
                System.Diagnostics.Debug.WriteLine(ext.Message);
            }
        }

        public static class FontXmlConverter
        {
            public static string ConvertToString(Font font)
            {
                try
                {
                    if (font != null)
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
                        return converter.ConvertToString(font);
                    }
                    else
                        return null;
                }
                catch { System.Diagnostics.Debug.WriteLine("Unable to convert"); }
                return null;
            }
            public static Font ConvertToFont(string fontString)
            {
                try
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
                    return (Font)converter.ConvertFromString(fontString);
                }
                catch { System.Diagnostics.Debug.WriteLine("Unable to convert"); }
                return null;
            }
        }

    }
}
