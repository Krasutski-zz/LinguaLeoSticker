using System;
using System.Collections.Generic;
using System.IO;

namespace LinguaLeoSticker
{
    class DictMng
    {
        private string[] _data;
        private int _dictonaryLine;

        private const char Separator = ':';

        private string[] LineToText(int lineNum)
        {
            if (lineNum <= 0) throw new ArgumentOutOfRangeException(nameof(lineNum));
            string line;

            try
            {
                line = _data[lineNum];
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
                return null;
            }

            string[] couple = new string[2];

            int pos = line.IndexOf(Separator);
            if (pos != -1)
            {
                couple[0] = line.Substring(0, pos);
                couple[1] = line.Substring(pos + 1);
            }
            else
            {
                return null;
            }

            return couple;
        }

        public bool Open(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            var ret = false;

            if (path != "")
            {
                try
                {
                    _data = File.ReadAllLines(path, System.Text.Encoding.Default/*Encoding.GetEncoding(1251)*/);
                    _dictonaryLine = 0;

                    ret = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return ret;
        }

        public void Open(string[] dict)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));
            _data = dict;
            _dictonaryLine = 0;
        }

        public string[] GetRandomCouple()
        {

            if (_data.Length == 0 || _data == null)
            {
                return null;
            }
            _dictonaryLine = new Random().Next(0, maxValue: _data.Length);

            return LineToText(_dictonaryLine);
        }

        public string[] GetCurrentCouple()
        {

            if (_data == null || _data.Length == 0)
            {
                return null;
            }

            if (_dictonaryLine >= _data.Length)
            {
                _dictonaryLine = _data.Length - 1;
            } 
            
            return LineToText(_dictonaryLine);
        }

        public string[] GetNextCouple()
        {

            if (_data == null || _data.Length == 0)
            {
                return null;
            }

            ++_dictonaryLine;
            if (_dictonaryLine >= _data.Length)
            {
                _dictonaryLine = 0;
            }

            return LineToText(_dictonaryLine);
        }

        public string[] GetPrevCouple()
        {

            if (_data.Length == 0 || _data == null)
            {
                return null;
            }

            --_dictonaryLine;
            if (_dictonaryLine < 0)
            {
                _dictonaryLine = _data.Length - 1;
            }

            return LineToText(_dictonaryLine);
        }

        public bool RemoveWord(string[] couple)
        {
            var list = new List<string>(_data);
            list.Remove($"{couple[0]}{Separator}{couple[1]}");
            _data = list.ToArray();

            return true;
        }

        public bool AddWord(string word, string translate)
        {
            if (word == "")
            {
                return false;
            }

            if (translate == "")
            {
                return false;
            }

            string line = $"{word.ToLower()}{Separator}{translate.ToLower()}";

            if (_data != null)
            {
                Array.Resize(ref _data, _data.Length + 1);
                _data[_data.Length - 1] = line;
            }

            return true;
        }

        public bool Save(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (_data != null)
            {
                File.WriteAllLines(path, _data, System.Text.Encoding.UTF8);
            }

            return true;
        }
    }
}
