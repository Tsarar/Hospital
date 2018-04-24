using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Course.parser
{
    class TxtConfigParser : IConfigParser
    {
        private readonly Dictionary<string, string> _values = new Dictionary<string, string>();

        public TxtConfigParser(string path)
        {
            ReadFile(path);
        }
        public void ReadFile(string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show(Properties.Messages.Error + " " + path + " " + Properties.Messages.NotExist);
                return;
            }

            var filestream = new FileStream(path, 
                                            FileMode.Open,
                                            FileAccess.Read);
            var file = new StreamReader(filestream, Encoding.UTF8, true, 128);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                //except section names
                if (!line.Contains("[") && line != "")
                {
                    var arr = line.Split(':');
                    _values.Add(arr[0].Trim(), arr[1].Trim());
                }
            }
        }

        public string GetSetting(string paramName, string validationRule)
        {
            string pattern;
            //pattern is number
            if (validationRule.ToLower() == "number") { pattern = @"^\d+$"; }
            //pattern is host
            else if (validationRule.ToLower() == "host")
            {
                pattern = @"(localhost|^[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}$)";
            }
            //pattern is string
            else if (validationRule.ToLower() == "string") { pattern = @"^\w+$"; }
            else
            {
                MessageBox.Show("ERROR! Validation rule for "+paramName+" is not defined!");
                return "";
            }

            //wrong parameter
            if (!_values.ContainsKey(paramName))
            {
                MessageBox.Show(Properties.Messages.Error + " " + paramName + " " + Properties.Messages.NotExist);
                return "";
            }

            //validation
            Regex reg = new Regex(pattern);
            if (!reg.IsMatch(_values[paramName]))
            {
                MessageBox.Show(Properties.Messages.Error + " " + paramName + " " + Properties.Messages.Invalid);
                return "";
            }

            return _values[paramName];
        }

        public string GetRawSetting(string paramName)
        {
            if (_values.ContainsKey(paramName)) return _values[paramName];
            //nonexistent parameter
            MessageBox.Show(Properties.Messages.Error + " " + paramName + " " + Properties.Messages.NotExist);
            return "";
        }
    }
}
