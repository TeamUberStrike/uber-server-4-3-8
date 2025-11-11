using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cmune.DataCenter.Utils
{
    public class CmuneCsv
    {
        private const string CsvFieldSeparator = ",";
        private const string CsvFieldContainer = "\"";
        private StringBuilder Output { get; set; }

        public CmuneCsv(List<string> headers)
        {
            Output = new StringBuilder();

            if (headers.Count > 0)
            {
                foreach (string header in headers)
                {
                    AddRowToCsv(header);
                }

                AppendLine();
            }
        }

        public void AddRowToCsv(string rowContent)
        {
            AddSeparatorToCsv();

            Output.Append(CsvFieldContainer);
            Output.Append(rowContent.Replace(CsvFieldContainer, CsvFieldContainer + CsvFieldContainer));
            Output.Append(CsvFieldContainer);
        }

        public void AddRowToCsv(int rowContent)
        {
            AddSeparatorToCsv();

            Output.Append(rowContent);
        }

        public void AddRowToCsv(bool rowContent)
        {
            AddSeparatorToCsv();

            Output.Append(rowContent);
        }

        public void AppendLine()
        {
            Output.AppendLine();
        }

        private void AddSeparatorToCsv()
        {
            int newLineLength = Environment.NewLine.Length;

            // TODO: Test should be refactored into method
            if (Output.Length > newLineLength && Output.ToString().Substring(Output.Length - newLineLength, newLineLength) != Environment.NewLine || newLineLength == 1 && Output.Length == 1 && Output[0].ToString() != Environment.NewLine)
            {
                Output.Append(CsvFieldSeparator);
            }
        }

        public override string ToString()
        {
            return Output.ToString();
        }
    }
}