using System;
using System.Text;

namespace Common.Html
{
    public static class HtmlParser
    {
        public static string Tidy(string html)
        {
            var doc = Parse(html);
            return doc.ToString();
        }

        public static HtmlElement Parse(string html, bool removeInstructions = false)
        {
            html = html.Trim();
            html = RemoveSpaces(html);

            var root = new HtmlDocument();
            AddNodes(root, html, removeInstructions);
            return root;
        }

        private static void AddNodes(HtmlElement parent, string html, bool removeInstructions)
        {
            var len = html.Length;
            var i = -1;
            var text = "";
            while (i < len - 1)
            {
                i++;

                if (i >= html.Length)
                    break;

                var s = html.Substring(i, 1);

                if (s == "<")
                {
                    var start = i + 1;
                    var stop = html.IndexOf(">", i, StringComparison.Ordinal);
                    if (stop < i)
                        continue;
                    if (text.Trim() != "")
                        parent.Add(new HtmlText(text));
                    text = "";

                    var fragment = html.Substring(start, stop - start);
                    if (fragment.StartsWith("!--"))
                    {
                        var minus = 3;
                        stop = html.IndexOf("-->", start + 3, StringComparison.Ordinal);
                        if (stop == -1)
                        {
                            stop = html.Length;
                            minus = 0;
                        }
                        parent.Add(new HtmlComment(html.Substring(start + 3, stop - start - minus)));
                        i = stop + minus;
                    }
                    else if (fragment.StartsWith("!"))
                    {
                        if (!removeInstructions)
                            parent.Add(new HtmlInstruction(fragment.Substring(1).Substring(0, fragment.Length - 1)));
                        i = stop;
                    }
                    else if (fragment.StartsWith("?"))
                    {
                        if (!removeInstructions)
                            parent.Add(new HtmlDeclaration(fragment.Substring(1).Substring(0, fragment.Length - 1)));
                        i = stop;
                    }
                    else if (fragment.StartsWith("/"))
                    {
                        //remove invalid closing tags
                        i = stop;
                    }
                    else
                    {
                        var node = CreateNode(html, start, ref stop);
                        parent.Add(node);
                        i = stop + (node.IsClosed ? 0 : (node.Name.Length + 2));
                    }
                    continue;
                }

                text += s;
            }

            if (text.Trim() != "")
                parent.Add(new HtmlText(text));
        }

        private static HtmlElement CreateNode(string html, int start, ref int stop)
        {
            var fragment = html.Substring(start, stop - start);
            var node = CreateNode(fragment);
            if (node.IsClosed)
                return node;

            var ix = FindClosingTagIndex(node.Name, html, stop + 1);
            if (ix == -1)
            {
                ix = html.Length;
            }

            var contents = html.Substring(stop + 1, ix - stop - 1);
            //Do not parse script contents
            if (node.Name == "script" || node.Name == "title" || node.Name == "style")
                node.ChildNodes.Add(new HtmlText(contents));
            //else if (node.Name == "style")
            //    node.ChildNodes.Add(HtmlStyle.Parse(contents));
            else
                AddNodes(node, contents, true);

            stop = ix;

            return node;
        }

        private static int FindClosingTagIndex(string tagName, string html, int startIndex)
        {
            var tag1 = "<" + tagName;
            var tag2 = "</" + tagName + ">";
            var ix = startIndex;
            while (ix > -1 && ix < html.Length)
            {

                var ix1 = html.IndexOf(tag1, ix, StringComparison.CurrentCultureIgnoreCase);
                var ix2 = html.IndexOf(tag2, ix, StringComparison.CurrentCultureIgnoreCase);

                if (ix1 == -1 && ix2 == -1)
                    break;

                if (ix2 < ix1 || (ix1 == -1 && ix2 > -1))
                    return ix2;

                //make sure we are not within script tags
                var ix3 = html.IndexOf("<script ", ix, StringComparison.CurrentCultureIgnoreCase);
                if (ix3 > -1 && ix3 < ix2)
                {
                    ix3 = html.IndexOf("</script>", ix3 + 7, StringComparison.CurrentCultureIgnoreCase);
                    if (ix3 > ix2)
                    {
                        ix = ix3 + 8;
                        continue;
                    }
                }

                ix = ix2 + tag1.Length;
            }
            return -1;
        }

        private static HtmlElement CreateNode(string htmlPart)
        {
            var html = htmlPart.Trim();
            var isClosed = html.EndsWith("/");

            var ix = html.IndexOf(" ", StringComparison.Ordinal);
            if (ix < 0)
            {
                if (isClosed)
                    html = html.Substring(0, html.Length - 1);
                return new HtmlElement(html, isClosed || !CanHaveContent(html));
            }


            var tagName = html.Substring(0, ix);
            html = html.Substring(ix + 1);


            if (isClosed)
                html = html.Substring(0, html.Length - 1);

            if (!isClosed && !CanHaveContent(tagName.ToLower()))
                isClosed = true;

            var root = new HtmlElement(tagName, isClosed);

            var len = html.Length;

            var attrName = "";
            var attrVal = "";
            var isName = true;
            var isValue = false;
            var hasQuotes = false;
            for (var i = 0; i < len; i++)
            {
                var s = html.Substring(i, 1);

                if (s == "=")
                {
                    isName = false;
                    isValue = true;

                    var nextChar = html.Substring(i + 1, 1);
                    hasQuotes = (nextChar == "\"" || nextChar == "'");
                }
                else if (s == " " && isName)
                {
                    //add attribute that requires no value
                    root.Add(attrName, attrName);

                    //reset attribute name
                    attrName = "";
                    attrVal = "";
                }
                else if (s == " " && attrVal.Length > 0)
                {
                    if (!hasQuotes || (attrVal[0] == attrVal[attrVal.Length - 1]))
                    {
                        isValue = false;
                        isName = true;

                        var value = FixAttributeValue(attrVal);
                        root.Add(attrName, value);

                        attrName = "";
                        attrVal = "";
                    }
                    else if (isValue)
                        attrVal += s;
                }

                else if (isName)
                {
                    attrName += s;
                }
                else if (isValue)
                {
                    attrVal += s;

                    //if this is a function, copy arguments
                    if (s == "(")
                    {
                        var ien = html.IndexOf(")", i, StringComparison.Ordinal);
                        if (ien > -1)
                        {
                            var args = html.Substring(i + 1, ien - i).Replace("\"", "&#34;").Replace("'", "&#39;");
                            attrVal += args;
                            i = ien;
                        }
                    }
                }
            }

            if (isName && attrName != "" && attrVal == "")
                attrVal = attrName;

            if (attrName != "" && attrVal != "")
            {
                var value = FixAttributeValue(attrVal);
                root.Add(attrName, value);
            }

            return root;
        }

        private static bool CanHaveContent(string tagName)
        {
            return !"|img|br|input|hr|meta|base|link".Contains("|" + tagName + "|");
        }

        private static string FixAttributeValue(string attrValue)
        {
            if (attrValue.StartsWith("\"") && attrValue.EndsWith("\""))
                attrValue = attrValue.Substring(1, attrValue.Length - 2);
            else if (attrValue.StartsWith("'") && attrValue.EndsWith("'"))
                attrValue = attrValue.Substring(1, attrValue.Length - 2).Replace("\"", "&quot;");
            return attrValue;
        }

        private static string RemoveSpaces(string html)
        {
            html = html.Trim();

            var buf = new StringBuilder();
            var len = html.Length;
            var lastChar = "";
            var withinTag = false;
            var closingTag = false;
            var tagName = "";
            var isAttribute = false;
            var quoteOpen = "";

            for (var i = 0; i < len; i++)
            {
                var s = html.Substring(i, 1);
                if (s == "<")
                {
                    withinTag = true;
                }
                else if (s == ">")
                {
                    withinTag = false;
                    closingTag = false;
                    tagName = "";
                    isAttribute = false;
                }
                else if (s == "/" && withinTag)
                    closingTag = true;

                if (withinTag)
                {
                    if (s == "\t" || s == "\n" || s == "\r")
                        s = " ";

                    if (s == " ")
                    {
                        if (!string.IsNullOrEmpty(tagName))
                        {
                            isAttribute = true;
                            tagName = "";
                        }

                        if (lastChar == " " || lastChar == "/" || lastChar == "=" || lastChar == "<")
                            continue;

                        if (i < len + 1)
                        {
                            var nextChar = html.Substring(i + 1, 1);
                            if (nextChar == ">" || nextChar == "=" || nextChar == "/" || nextChar == "\"")
                                continue;
                        }
                    }
                    else
                    {
                        if (quoteOpen == "")
                        {
                            if (s == "\"" || s == "'")
                                quoteOpen = s;
                        }
                        else if (quoteOpen == s)
                        {
                            quoteOpen = "";
                            if (i < len + 1)
                            {
                                var nextChar = html.Substring(i + 1, 1);
                                if (nextChar != " ")
                                    s += " ";
                            }
                        }
                    }

                    if (!isAttribute)
                        tagName += s;
                }

                buf.Append(s);
                lastChar = s;
            }
            return buf.ToString();
        }
    }
}
