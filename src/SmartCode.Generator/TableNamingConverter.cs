using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.Generator.Entity;

namespace SmartCode.Generator
{
    public class TableNamingConverter : INamingConverter
    {
        private readonly ILogger<TableNamingConverter> _logger;

        public TableNamingConverter(ILogger<TableNamingConverter> logger)
        {
            _logger = logger;
        }

        public bool Initialized { get; private set; }

        public string Name { get; private set; } = "Table";

        public void Convert(BuildContext context)
        {
            var table = context.GetCurrentTable();
            TokenizerMapConverter convertMap_Table;
            if (table.Type == Table.TableType.View)
            {
                convertMap_Table = context.Build.NamingConverter.View;
            }
            else
            {
                convertMap_Table = context.Build.NamingConverter.Table;
            }
            table.ConvertedName = Convert(table.Name, convertMap_Table);

            TokenizerMapConverter convertMap_Column = context.Build.NamingConverter.Column;

            foreach (var col in table.Columns)
            {
                col.ConvertedName = Convert(col.Name, convertMap_Column);
            }
        }

        public void Initialize(IDictionary<string, object> paramters)
        {
            this.Initialized = true;
        }

        private string Convert(string phrase, TokenizerMapConverter tokenizerMapConverter)
        {
            var tokenizer = TokenizerFactory.Create(tokenizerMapConverter.Tokenizer);
            var words = tokenizer.Segment(phrase);
            var wordsConvert = WordsConverterFactory.Create(tokenizerMapConverter.Converter);
            return wordsConvert.Convert(words);
        }
    }
}
