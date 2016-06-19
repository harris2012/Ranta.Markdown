using System;
using System.Collections.Generic;
using System.Text;

namespace Ranta.Markdown
{
    internal struct Token
    {
        public Token(TokenType type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        public TokenType Type;

        public string Value;
    }
}
