using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EasySwarm2._0
{
    class ExcelReader
    {
        public ExcelReader()
        {

        }

        public void Open(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader mysr = new StreamReader(fs, Encoding.Default);

            m_dictionaryRowString.Clear();
            string strline = null;
            int row = 1;
            while ((strline = mysr.ReadLine()) != null)
            {
                if (row == 1)
                {
                    ++row;
                    continue;
                }
                m_dictionaryRowString[row - 1] = strline;
                ++row;
            }

            mysr.Close();
            fs.Close();
        }

        public string GetCellString(int row, int col)
        {
            string str = null;
            if (Read(row, col, ref str))
            {
                return str;
            }
            else
                return "";
        }

        public int GetRowCount()
        {
            return m_dictionaryRowString.Count;
        }

        public double GetCellDouble(int row, int col)
        {
            string str = null;
            if (Read(row, col, ref str))
            {
                return Convert.ToDouble(str);
            }
            else
                return 0.0;
        }

        public int GetCellInt(int row, int col)
        {
            string str = null;
            if (Read(row, col, ref str))
            {
                return Convert.ToInt32(str);
            }
            else
                return 0;
        }

        private bool Read(int row, int col, ref string dst)
        {
            if (row <= 0 || col <= 0)
                return false;

            if (row > m_dictionaryRowString.Count)
                return false;

            string str = m_dictionaryRowString[row];
            
            string[] arraryLine = str.Split(',');

            if (col > arraryLine.Length)
                return false;

            dst = arraryLine[col - 1];

            return true;
        }

        private Dictionary<int, string> m_dictionaryRowString = new Dictionary<int, string>();
    }
}
