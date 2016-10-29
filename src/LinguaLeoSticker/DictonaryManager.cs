using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DictonaryManager
{
    class DictMng
    {

        private string[] Data = null;    
        private int DictonaryLine = 0;

        private const char separator = ':';


        private string[] LineToText(int LineNum)
        {
            string Line = "";

            try
            {
                Line = Data[LineNum];
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
                return null;
            }

            string[] couple = new string[2];

            int pos = Line.IndexOf(separator);
            if (pos != -1)
            {
                couple[0] = Line.Substring(0, pos);
                couple[1] = Line.Substring(pos + 1);
            }
            else
            {
                return null;
            }

            return couple;
        }

        public bool Open(string Path)
        {
            bool Ret = false;

            if (Path != "")
            {
                try
                {
                    Data = File.ReadAllLines(Path, System.Text.Encoding.Default/*Encoding.GetEncoding(1251)*/);
                    DictonaryLine = 0;

                    Ret = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return Ret;
        }

        public void Open(string[] Dict)
        {
            Data = Dict;
            DictonaryLine = 0;
        }

        public string[] GetRandomCouple()
        {

            if (Data.Length == 0 || Data == null)
            {
                return null;
            }

            return LineToText(new Random().Next(0, Data.Count()));
        }

        public string[] GetCurrentCouple()
        {

            if (Data == null || Data.Length == 0)
            {
                return null;
            }

            if (DictonaryLine >= Data.Length)
            {
                DictonaryLine = Data.Length - 1;
            } 
            
            return LineToText(DictonaryLine);
        }

        public string[] GetNextCouple()
        {

            if (Data == null || Data.Length == 0)
            {
                return null;
            }

            ++DictonaryLine;
            if (DictonaryLine >= Data.Count())
            {
                DictonaryLine = 0;
            }

            return LineToText(DictonaryLine);
        }

        public string[] GetPrevCouple()
        {

            if (Data.Length == 0 || Data == null)
            {
                return null;
            }

            --DictonaryLine;
            if (DictonaryLine < 0)
            {
                DictonaryLine = Data.Count() - 1;
            }

            return LineToText(DictonaryLine);
        }

        public bool RemoveWord(string[] couple)
        {
            var list = new List<string>(Data);
            list.Remove(string.Format("{0}{1}{2}", couple[0], separator, couple[1]));
            Data = list.ToArray();

            return true;
        }

        public bool AddWord(string Word, string Translate)
        {
            if (Word == "")
            {
                return false;
            }

            if (Translate == "")
            {
                return false;
            }

            string line = String.Format("{0}{1}{2}", Word, separator, Translate);

            if (Data != null)
            {
                Array.Resize(ref Data, Data.Length + 1);
                Data[Data.Length - 1] = line;
            }

            return true;
        }

        public bool Save(string Path)
        {
            if (Data != null)
            {
                File.WriteAllLines(Path, Data, System.Text.Encoding.UTF8);
            }

            return true;
        }
    }
}
