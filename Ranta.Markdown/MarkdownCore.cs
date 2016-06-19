using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ranta.Markdown.Model;
using Ranta.Markdown.Template;

namespace Ranta.Markdown
{
    /// <summary>
    /// 将markdown文本转换成html文本的
    /// </summary>
    public static class MarkdownCore
    {
        /// <summary>
        /// 将输入的markdown文本转换为html文本
        /// </summary>
        /// <param name="text">需要转换的markdown文本</param>
        /// <returns>转换成的html文本</returns>
        public static string Transform(string text)
        {
            string content = string.Empty;

            if (!string.IsNullOrEmpty(text))
            {
                var markdownNodeList = Process(text);

                if (markdownNodeList != null && markdownNodeList.Count() > 0)
                {
                    DocumentTemplate template = new DocumentTemplate();

                    template.MarkdownNodeList = markdownNodeList;

                    content = template.TransformText();
                }
            }

            return content;
        }

        private static IEnumerable<NodeBase> Process(string content)
        {
            List<NodeBase> nodeList = new List<NodeBase>();

            var lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            UL ul = null;
            OL ol = null;

            foreach (var line in lines)
            {
                switch (line[0])
                {
                    case '#':
                        {
                            ul = null; ol = null;

                            int level = 1;
                            while (level < Math.Min(line.Length, 6) && line[level] == '#')
                            {
                                level++;
                            }
                            if (level < line.Length && line[level] == ' ')
                            {
                                nodeList.Add(new Headline { Text = line.Substring(level), Level = level });
                            }
                            else
                            {
                                nodeList.Add(new Paragraph { ItemList = ProcessLine(line) });
                            }
                        }

                        break;
                    case '>':
                        {
                            ul = null; ol = null;
                            nodeList.Add(new Quote { Text = line.Substring(1) });
                        }

                        break;
                    case '*':
                        {
                            ol = null;
                            ListItem li = new ListItem { ItemList = ProcessLine(line.Substring(1)) };
                            if (ul != null)
                            {
                                ul.ItemList.Add(li);
                            }
                            else
                            {
                                ul = new UL();
                                ul.ItemList = new List<ListItem>();
                                ul.ItemList.Add(li);
                                nodeList.Add(ul);
                            }
                        }

                        break;
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '0':
                        {
                            ul = null;
                            StringBuilder builder = new StringBuilder();
                            int index = 0;
                            while (index < line.Length && char.IsNumber(line[index]))
                            {
                                builder.Append(line[index]);

                                index++;
                            }
                            if (index < line.Length && line[index] == '.' && index + 1 < line.Length && line[index + 1] == ' ')
                            {
                                ListItem li = new ListItem { ItemList = ProcessLine(line.Substring(index + 2)) };
                                if (ol != null)
                                {
                                    ol.ItemList.Add(li);
                                }
                                else
                                {
                                    ol = new OL();
                                    ol.ItemList = new List<ListItem>();
                                    ol.ItemList.Add(li);
                                    nodeList.Add(ol);
                                }
                            }
                            else
                            {
                                nodeList.Add(new Paragraph { ItemList = ProcessLine(line) });
                            }
                        }

                        break;
                    default:
                        {
                            ul = null; ol = null;
                            nodeList.Add(new Paragraph { ItemList = ProcessLine(line) });
                        }

                        break;
                }
            }

            return nodeList;
        }

        #region 分析一个句子
        private static IEnumerable<ItemBase> ProcessLine(string text)
        {
            List<ItemBase> itemList = new List<ItemBase>();

            if (!string.IsNullOrEmpty(text))
            {
                var tokenList = GetTokenList(text);

                if (tokenList != null && tokenList.Count > 0)
                {
                    Stack<Status> statusStack = new Stack<Status>();
                    List<Token> tempTokenList = new List<Token>();

                    StringBuilder imageTextBuilder = new StringBuilder();
                    StringBuilder imageUrlBuilder = new StringBuilder();
                    StringBuilder linkTextBuilder = new StringBuilder();
                    StringBuilder linkUrlBuilder = new StringBuilder();

                    statusStack.Push(Status.Empty);

                    var index = 0;
                    while (index < tokenList.Count)
                    {
                        var currentStatus = statusStack.Peek();

                        switch (statusStack.Peek())
                        {
                            case Status.Empty:
                                {
                                    switch (tokenList[index].TokenType)
                                    {
                                        case TokenType.LeftMiddleBracket:
                                            {
                                                statusStack.Push(Status.Link1);
                                                tempTokenList.Add(tokenList[index]);
                                            }
                                            break;
                                        case TokenType.Excalmatory:
                                            {
                                                statusStack.Push(Status.Image1);
                                                tempTokenList.Add(tokenList[index]);
                                            }
                                            break;
                                        case TokenType.Plain:
                                        case TokenType.RightMiddleBracket:
                                        case TokenType.LeftSmallBracktet:
                                        case TokenType.RightSmallBracket:
                                        default:
                                            itemList.Add(new TextItem { Text = tokenList[index].Text });
                                            break;
                                    }
                                }
                                break;
                            case Status.Image1:
                                {
                                    if (tokenList[index].TokenType == TokenType.LeftMiddleBracket)
                                    {
                                        statusStack.Pop();
                                        statusStack.Push(Status.Image2);
                                        tempTokenList.Add(tokenList[index]);
                                    }
                                    else
                                    {
                                        statusStack.Pop();
                                        tempTokenList.RemoveAt(0);
                                        itemList.Add(new TextItem { Text = "!" });
                                    }
                                }
                                break;
                            case Status.Image2:
                                {
                                    if (tokenList[index].TokenType == TokenType.RightMiddleBracket)
                                    {
                                        statusStack.Pop();
                                        statusStack.Push(Status.Image4);
                                        tempTokenList.Add(tokenList[index]);
                                    }
                                    else
                                    {
                                        statusStack.Pop();
                                        statusStack.Push(Status.Image3);
                                        tempTokenList.Add(tokenList[index]);
                                        imageTextBuilder.Append(tokenList[index].Text);
                                    }
                                }
                                break;
                            case Status.Image3:
                                {
                                    if (tokenList[index].TokenType == TokenType.RightMiddleBracket)
                                    {
                                        statusStack.Pop();
                                        statusStack.Push(Status.Image4);
                                        tempTokenList.Add(tokenList[index]);
                                    }
                                    else
                                    {
                                        tempTokenList.Add(tokenList[index]);
                                        imageTextBuilder.Append(tokenList[index].Text);
                                    }
                                }
                                break;
                            case Status.Image4:
                                {
                                    if (tokenList[index].TokenType == TokenType.LeftSmallBracktet)
                                    {
                                        statusStack.Pop();
                                        statusStack.Push(Status.Image5);
                                        tempTokenList.Add(tokenList[index]);
                                    }
                                    else
                                    {
                                        statusStack.Pop();
                                        for (int i = 0; i < tempTokenList.Count; i++)
                                        {
                                            itemList.Add(new TextItem { Text = tempTokenList[i].Text });
                                        }
                                        tempTokenList.Clear();
                                        imageTextBuilder.Clear();
                                        index--;//将index-1，重新再判断状态
                                    }
                                }
                                break;
                            case Status.Image5:
                                {
                                    if (tokenList[index].TokenType == TokenType.RightSmallBracket)
                                    {
                                        statusStack.Pop();
                                        for (int i = 0; i < tempTokenList.Count; i++)
                                        {
                                            itemList.Add(new TextItem { Text = tempTokenList[i].Text });
                                        }
                                        tempTokenList.Clear();
                                        imageTextBuilder.Clear();
                                        itemList.Add(new TextItem { Text = ")" });
                                    }
                                    else
                                    {
                                        statusStack.Pop();
                                        statusStack.Push(Status.Image6);
                                        tempTokenList.Add(tokenList[index]);
                                        imageUrlBuilder.Append(tokenList[index].Text);
                                    }
                                }
                                break;
                            case Status.Image6:
                                {
                                    if (tokenList[index].TokenType == TokenType.RightSmallBracket)
                                    {
                                        statusStack.Pop();
                                        itemList.Add(new ImageItem { Text = imageTextBuilder.ToString(), Url = imageUrlBuilder.ToString() });
                                        imageTextBuilder.Clear();
                                        imageUrlBuilder.Clear();
                                    }
                                    else
                                    {
                                        tempTokenList.Add(tokenList[index]);
                                        imageUrlBuilder.Append(tokenList[index].Text);
                                    }
                                }
                                break;
                            case Status.Link1:
                                {
                                    if (tokenList[index].TokenType == TokenType.RightMiddleBracket)
                                    {
                                        statusStack.Pop();
                                        itemList.Add(new TextItem { Text = "[" });
                                        itemList.Add(new TextItem { Text = "]" });
                                    }
                                    else
                                    {
                                        statusStack.Pop();
                                        statusStack.Push(Status.Link2);
                                        tempTokenList.Add(tokenList[index]);
                                        linkTextBuilder.Append(tokenList[index].Text);
                                    }
                                }
                                break;
                            case Status.Link2:
                                if (tokenList[index].TokenType == TokenType.RightMiddleBracket)
                                {
                                    statusStack.Pop();
                                    statusStack.Push(Status.Link3);
                                    tempTokenList.Add(tokenList[index]);
                                }
                                else
                                {
                                    tempTokenList.Add(tokenList[index]);
                                    linkTextBuilder.Append(tokenList[index].Text);
                                }
                                break;
                            case Status.Link3:
                                {
                                    if (tokenList[index].TokenType == TokenType.LeftSmallBracktet)
                                    {
                                        statusStack.Pop();
                                        statusStack.Push(Status.Link4);
                                        tempTokenList.Add(tokenList[index]);
                                    }
                                    else
                                    {
                                        for (int i = 0; i < tempTokenList.Count; i++)
                                        {
                                            itemList.Add(new TextItem { Text = tempTokenList[index].Text });
                                        }
                                        linkTextBuilder.Clear();
                                        tempTokenList.Clear();
                                        index--;//将index-1，重新再判断状态
                                    }
                                }
                                break;
                            case Status.Link4:
                                {
                                    if (tokenList[index].TokenType == TokenType.RightSmallBracket)
                                    {
                                        itemList.Add(new LinkItem { Text = linkTextBuilder.ToString() });
                                        tempTokenList.Clear();
                                        linkTextBuilder.Clear();
                                    }
                                    else
                                    {
                                        statusStack.Pop();
                                        statusStack.Push(Status.Link5);
                                        tempTokenList.Add(tokenList[index]);
                                        linkUrlBuilder.Append(tokenList[index].Text);
                                    }
                                }
                                break;
                            case Status.Link5:
                                {
                                    if (tokenList[index].TokenType == TokenType.RightSmallBracket)
                                    {
                                        itemList.Add(new LinkItem { Text = linkTextBuilder.ToString(), Url = linkUrlBuilder.ToString() });
                                    }
                                    else
                                    {
                                        tempTokenList.Add(tokenList[index]);
                                        linkUrlBuilder.Append(tokenList[index].Text);
                                    }
                                }
                                break;
                            default:
                                break;
                        }

                        index++;
                    }
                }
            }

            return itemList;
        }

        internal enum Status
        {
            Empty,

            /// <summary>
            /// 感叹号"!"
            /// </summary>
            Image1,
            /// <summary>
            /// 左中括号"["
            /// </summary>
            Image2,
            /// <summary>
            /// 中括号内文字
            /// </summary>
            Image3,
            /// <summary>
            /// 右中括号"]"
            /// </summary>
            Image4,
            /// <summary>
            /// 左小括号"("
            /// </summary>
            Image5,
            /// <summary>
            /// 小括号内文字
            /// </summary>
            Image6,

            /// <summary>
            /// 左中括号"["
            /// </summary>
            Link1,
            /// <summary>
            /// 中括号内文字
            /// </summary>
            Link2,
            /// <summary>
            /// 右中括号"]"
            /// </summary>
            Link3,
            /// <summary>
            /// 左小括号"("
            /// </summary>
            Link4,
            /// <summary>
            /// 小括号内文字
            /// </summary>
            Link5
        }

        internal enum TokenType
        {
            /// <summary>
            /// 左 中括号
            /// </summary>
            LeftMiddleBracket,

            /// <summary>
            /// 右中括号
            /// </summary>
            RightMiddleBracket,

            /// <summary>
            /// 左小括号
            /// </summary>
            LeftSmallBracktet,

            /// <summary>
            /// 右小括号
            /// </summary>
            RightSmallBracket,

            /// <summary>
            /// 感叹号
            /// </summary>
            Excalmatory,

            /// <summary>
            /// 星号
            /// </summary>
            Star,

            /// <summary>
            /// 文本
            /// </summary>
            Plain
        }

        internal struct Token
        {
            public TokenType TokenType { get; set; }

            public string Text { get; set; }
        }

        private static List<Token> GetTokenList(string text)
        {
            List<Token> tokens = new List<Token>();

            if (!string.IsNullOrEmpty(text))
            {
                StringBuilder current = new StringBuilder();
                for (int i = 0; i < text.Length; i++)
                {
                    switch (text[i])
                    {
                        case '!':
                            ProcessToken(tokens, current);
                            tokens.Add(new Token { Text = text[i].ToString(), TokenType = TokenType.Excalmatory });

                            break;
                        case '[':
                            ProcessToken(tokens, current);
                            tokens.Add(new Token { Text = text[i].ToString(), TokenType = TokenType.LeftMiddleBracket });

                            break;
                        case ']':
                            ProcessToken(tokens, current);
                            tokens.Add(new Token { Text = text[i].ToString(), TokenType = TokenType.RightMiddleBracket });

                            break;
                        case '(':
                            ProcessToken(tokens, current);
                            tokens.Add(new Token { Text = text[i].ToString(), TokenType = TokenType.LeftSmallBracktet });

                            break;
                        case ')':
                            ProcessToken(tokens, current);
                            tokens.Add(new Token { Text = text[i].ToString(), TokenType = TokenType.RightSmallBracket });

                            break;
                        case '*':
                            ProcessToken(tokens, current);
                            tokens.Add(new Token { Text = text[i].ToString(), TokenType = TokenType.Star });

                            break;
                        default:
                            current.Append(text[i]);

                            break;
                    }
                }
                if (current.Length > 0)
                {
                    tokens.Add(new Token { Text = current.ToString(), TokenType = TokenType.Plain });
                }
            }

            return tokens;
        }

        private static void ProcessToken(List<Token> tokens, StringBuilder current)
        {
            if (current.Length > 0)
            {
                tokens.Add(new Token
                {
                    Text = current.ToString(),
                    TokenType = TokenType.Plain
                }); current.Clear();
            }
        }
        #endregion
    }
}
