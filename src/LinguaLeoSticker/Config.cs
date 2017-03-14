using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;

namespace LinguaLeoSticker
{

    public struct ParamList
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public UInt32 BackgroundColor;
        public UInt32 TextColor;
        public UInt32 TextTranslateColor;
        public int TimeText;
        public int TimeTextTranslate;
        public string DictonaryPath;
        public string TextFont;
        public string TextTranslateFont;
        public bool AutoLoad;
        public string LinguaLeoUser;
        public string LinguaLeoPassword;
        public bool RandomMode;
    }

    class Config
    {
        private readonly string _fileName;

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
        public bool AutoLoad { get; set; }
        public string LinguaLeoUser { get; set; }
        public string LinguaLeoPassword { get; set; }
        public bool RandomMode { get; set; }


        [XmlElement("BackgroundColor")]
        public UInt32 BackgroundColorConvert
        {
            get { return (UInt32)BackgroundColor.ToArgb(); }
            set { BackgroundColor = Color.FromArgb((int)value); }
        }

        [XmlElement("TextColor")]
        public UInt32 TextColorConvert
        {
            get { return (UInt32)TextColor.ToArgb(); }
            set { TextColor = Color.FromArgb((int)value); }
        }

        [XmlElement("TextTranslateColor")]
        public UInt32 TextTranslateColorConvert
        {
            get { return (UInt32)TextTranslateColor.ToArgb(); }
            set { TextTranslateColor = Color.FromArgb((int)value); }
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
            _fileName = file;

            X = 100;
            Y = 100;
            Width = 280;
            Height = 100;
            BackgroundColor = Color.Yellow;
            TextColor = Color.Black;
            TextTranslateColor = Color.Black;
            TimeText = 3000;
            TimeTextTranslate = 2000;
            DictonaryPath = "default.txt";
            TextFont = new Font(new FontFamily("Arial"), 20, FontStyle.Regular, GraphicsUnit.Pixel);
            TextTranslateFont = new Font(new FontFamily("Arial"), 18, FontStyle.Regular, GraphicsUnit.Pixel);
            AutoLoad = true;
            LinguaLeoUser = "email";
            LinguaLeoPassword = "password";
            RandomMode = true;

            try
            {

                XmlSerializer deserializer = new XmlSerializer(typeof(ParamList));
                StreamReader reader = new StreamReader(_fileName);
                var config = (ParamList)deserializer.Deserialize(reader);
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
                AutoLoad = config.AutoLoad;

                if (config.LinguaLeoUser != null)
                {
                    LinguaLeoUser = config.LinguaLeoUser;
                }

                if (config.LinguaLeoPassword != null)
                {
                    LinguaLeoPassword = config.LinguaLeoPassword;
                }

                RandomMode = config.RandomMode;
            }
            catch (Exception ext)
            {
                System.Diagnostics.Debug.WriteLine(ext.Message);
            }

        }

    

        public void SaveConfig()
        {
            try
            {

                ParamList config = new ParamList
                {
                    X = X,
                    Y = Y,
                    Width = Width,
                    Height = Height,
                    BackgroundColor = BackgroundColorConvert,
                    TextColor = TextColorConvert,
                    TextTranslateColor = TextTranslateColorConvert,
                    TimeText = TimeText,
                    TimeTextTranslate = TimeTextTranslate,
                    DictonaryPath = DictonaryPath,
                    TextFont = TextFontConvert,
                    TextTranslateFont = TextTranslateFontConvert,
                    AutoLoad = AutoLoad,
                    LinguaLeoUser = LinguaLeoUser,
                    LinguaLeoPassword = LinguaLeoPassword,
                    RandomMode = RandomMode
                };


                XmlSerializer ser = new XmlSerializer(typeof(ParamList));
                StreamWriter writer = new StreamWriter(_fileName);
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
