using System;
using Be.Runtime.Format;
using Be.Runtime.Types;

namespace Be.Runtime.Parse
{
    public class TextParser
    {
        public string Text;
        private int pos, len;
        private int start, end;

        public TextParser(string text)
        {
            this.Text = (text == null ? "" : text);
            this.len = Text.Length;
        }

        public int GetLength()
        {
            return len;
        }

        public int GetPosition()
        {
            return pos;
        }

        public void SetPosition(int Position)
        {
            if (Position < 0 || Position > len)
            {
                throw new Exception("position out of range");
            }
            pos = start = end = Position;
        }

        private bool _Begin()
        {
            if (start > len - 1)
            {
                return false;
            }
            pos = start;
            return true;
        }

        private void _Finish(bool forward)
        {
            if (forward)
            {
                start = end = pos;
            }
            else
            {
                pos = end = start;
            }
        }

        public char NextChar(bool forward)
        {
            if (pos > len - 1)
            {
                return '\0';
            }
            char chr = Text[pos];
            pos++;
            _Finish(forward);
            return chr;
        }

        public char PrevChar(bool backward)
        {
            if (pos - 1 < 0)
            {
                return '\0';
            }
            pos--;
            _Finish(backward);
            return Text[pos];
        }

        private bool _Equal(string str)
        {
            if (pos + str.Length > len)
            {
                return false;
            }
            for (int i = 0; i < str.Length; i++)
            {
                if (Text[pos++] != str[i])
                {
                    return false;
                }
            }
            return true;
        }

        public bool Equal(string str, bool forward)
        {
            if (!_Equal(str))
            {
                _Finish(false);
                return false;
            }
            _Finish(forward);
            return true;
        }

        private static bool _IsSpace(char c)
        {
            if (c != ' ' && c != '\t' && c != '\r' && c != '\n')
            {
                return false;
            }
            return true;
        }

        private bool _EqualSpace()
        {
            if (pos > len - 1)
            {
                return false;
            }
            while (pos < len)
            {
                if (!_IsSpace(Text[pos]))
                {
                    break;
                }
                pos++;
            }
            return true;
        }

        public bool SkipSpace(bool forward)
        {
            _EqualSpace();
            _Finish(forward);
            return true;
        }

        public bool EqualSpace(bool forward)
        {
            if (!_Begin())
            {
                return false;
            }
            if (!_EqualSpace())
            {
                _Finish(false);
                return false;
            }
            _Finish(forward);
            return true;
        }

        public bool EqualNoneSpace(string str, bool forward)
        {
            if (!_Begin())
            {
                return false;
            }
            _EqualSpace();
            if (!_Equal(str))
            {
                _Finish(false);
                return false;
            }
            _Finish(forward);
            return true;
        }

        public bool EqualEndSpace(string str, bool forward)
        {
            if (!_Begin())
            {
                return false;
            }
            _EqualSpace();
            if (!_Equal(str))
            {
                _Finish(false);
                return false;
            }
            if (!_EqualSpace())
            {
                _Finish(false);
                return false;
            }
            _Finish(forward);
            return true;
        }

        public bool StartEndNoneSpace(string str_start, string str_end, bool forward)
        {
            if (!_Begin())
            {
                return false;
            }
            _EqualSpace();
            if (!_Equal(str_start))
            {
                _Finish(false);
                return false;
            }
            _EqualSpace();
            if (!_Equal(str_end))
            {
                _Finish(false);
                return false;
            }
            _Finish(forward);
            return true;
        }

        public string GetPathContent(bool forward)
        {
            if (!_Begin())
            {
                return null;
            }
            if (!Char.IsLetter(Text[pos]))
            {
                _Finish(false);
                return null;
            }
            pos++;
            while (pos < len)
            {
                if (!(Char.IsLetter(Text[pos]) || Char.IsDigit(Text[pos]) || Text[pos] == '.' || Text[pos] == '_'))
                {
                    break;
                }
                pos++;
            }
            string result = Text.Substring(start, pos - start);
            _Finish(forward);
            return result;
        }

        public string GetNameContent(bool forward)
        {
            if (!_Begin())
            {
                return null;
            }
            if (!Char.IsLetter(Text[pos]))
            {
                _Finish(false);
                return null;
            }
            pos++;
            while (pos < len)
            {
                if (!(Char.IsLetter(Text[pos]) || Char.IsDigit(Text[pos]) || Text[pos] == '_'))
                {
                    break;
                }
                pos++;
            }
            string result = Text.Substring(start, pos - start);
            _Finish(forward);
            return result;
        }

        public TextParseCharResult ParseCharContent(bool forward)
        {
            // check start
            if (!_Begin())
            {
                _Finish(false);
                return new TextParseCharResult(TextParseResultCode.END_OF_TEXT);
            }
            // check enclosing tag
            if (Text[pos++] != '\'')
            {
                _Finish(false);
                return new TextParseCharResult(TextParseResultCode.INVALID_ENCLOSING_TAG);
            }
            // check required length
            if (pos + 2 > len - 1)
            {
                _Finish(false);
                return new TextParseCharResult(TextParseResultCode.END_OF_TEXT);
            }
            string result = null;
            CharType charType = CharType.UNKNOWN;
            // check for uni-code
            if (Text[pos] == '\\' && Text[pos + 1] == 'u')
            {
                pos += 2;
                // check enclosing minimun content length
                if (pos + 4 > len - 1)
                {
                    _Finish(false);
                    return new TextParseCharResult(TextParseResultCode.END_OF_TEXT);
                }
                // must have 4 char-digit 
                for (int i = 0; i < 4; i++)
                {
                    if (!Char.IsDigit(Text[pos + i]) && !Char.IsLetter(Text[pos + i]))
                    {
                        _Finish(false);
                        return new TextParseCharResult(TextParseResultCode.INVALID_CONTENT_SYNTAX);
                    }
                }
                result = Text.Substring(pos-2, 6);
                charType = CharType.CHAR_CODE;
                pos += 4;
            }
            // else native-char content
            else
            {
                // must has 1 char
                result = Text.Substring(pos, 1);
                charType = CharType.CHAR_SYMBOL;
                pos += 1;
            }
            // check enclosing minimun content length
            if (pos + 1 > len - 1)
            {
                _Finish(false);
                return new TextParseCharResult(TextParseResultCode.END_OF_TEXT);
            }
            // break if declosing tag
            if (Text[pos++] != '\'')
            {
                _Finish(false);
                return new TextParseCharResult(TextParseResultCode.INVALID_DECLOSING_TAG);
            }
            // finish
            _Finish(forward);
            return new TextParseCharResult(result, charType);
        }

        public TextParseNumberResult ParseNumberContent(bool forward)
        {
            // check start
            if (!_Begin())
            {
                _Finish(false);
                return new TextParseNumberResult(TextParseResultCode.END_OF_TEXT);
            }
            // check required start number-digit
            if (!Char.IsDigit(Text[pos]))
            {
                _Finish(false);
                return new TextParseNumberResult(TextParseResultCode.INVALID_CONTENT_SYNTAX);
            }
            string result = Text[pos++]+"";
            // check integral-number content
            while (pos < len)
            {
                if (!Char.IsDigit(Text[pos]))
                {
                    break;
                }
                result += Text[pos++];
            }
            // check required length
            if (pos > len - 1)
            {
                _Finish(forward);
                return new TextParseNumberResult(result, NativeType.Int, NativeNumberGroup.Signed);
            }
            // check for possible floating-point
            bool isFloatingPoint = false;
            if (Text[pos] == '.')
            {
                isFloatingPoint = true;
                result += Text[pos++];
            }
            // check floating-number content
            while (pos < len)
            {
                if (!Char.IsDigit(Text[pos]))
                {
                    break;
                }
                result += Text[pos++];
            }
            // check possible number-format´-type
            NativeType nativeType = NativeUtils.GetNativeTypeEnum(Text[pos]);
            // integral-number can not be floating pointed
            if ((nativeType == NativeType.Int || nativeType == NativeType.Long) && isFloatingPoint)
            {
                _Finish(false);
                return new TextParseNumberResult(TextParseResultCode.INVALID_CONTENT_SYNTAX);
            }
            // use concret number-type
            else if(nativeType != NativeType.None)
            {
                pos++;
            }
            // or use default number-type
            else
            {
                // signed float
                if (isFloatingPoint)
                {
                    _Finish(forward);
                    return new TextParseNumberResult(result, NativeType.Float, NativeNumberGroup.Signed);
                }
                // signed integer
                else
                {
                    _Finish(forward);
                    return new TextParseNumberResult(result, NativeType.Int, NativeNumberGroup.Signed);
                }
            }
            // check required length
            if (pos > len - 1)
            {
                _Finish(false);
                return new TextParseNumberResult(TextParseResultCode.INVALID_CONTENT_SYNTAX);
            }
            // check possible number-range type
            NativeNumberGroup formatType = NativeUtils.GetNativeNumberTypeEnum(Text[pos]);
            if(formatType == NativeNumberGroup.None)
            {
                formatType = NativeNumberGroup.Signed;
            }
            else
            {
                pos++;
            }
            // return result
            _Finish(forward);
            return new TextParseNumberResult(result, nativeType, formatType);
        }

        public TextParseResult ParseStringContent(bool forward)
        {
            if (!_Begin())
            {
                _Finish(false);
                return new TextParseResult(TextParseResultCode.END_OF_TEXT);
            }
            if (Text[pos++] != '"')
            {
                _Finish(false);
                return new TextParseResult(TextParseResultCode.INVALID_ENCLOSING_TAG);
            }
            bool hasClosingTag = false;
            while(pos < len)
            {
                if (Text[pos++] == '"')
                {
                    hasClosingTag = true;
                    break;
                }
            }
            if (!hasClosingTag)
            {
                _Finish(false);
                return new TextParseResult(TextParseResultCode.INVALID_DECLOSING_TAG);
            }
            string result = Text.Substring(start + 1, pos - start - 2);
            _Finish(forward);
            return new TextParseResult(result);
        }
    }

    public enum TextParseResultCode
    {
        // success-code
        PARSE_SUCCESSED,
        // failed-codes
        END_OF_TEXT,
        INVALID_ENCLOSING_TAG,
        INVALID_DECLOSING_TAG,
        INVALID_CONTENT_SYNTAX,
    }

    public class TextParseResult
    {
        public TextParseResultCode ResultCode;
        public string Result;

        public TextParseResult(TextParseResultCode FailedResultCode)
        {
            this.ResultCode = FailedResultCode;
        }

        public TextParseResult(string Result)
        {
            this.ResultCode = TextParseResultCode.PARSE_SUCCESSED;
            this.Result = Result;
        }

        public bool IsSuccessed()
        {
            return (this.ResultCode == TextParseResultCode.PARSE_SUCCESSED);
        }

        public bool IsFailed()
        {
            return (this.ResultCode != TextParseResultCode.PARSE_SUCCESSED);
        }

        public string CauseString()
        {
            return Utils.EnumToString(ResultCode);
        }
    }

    public class TextParseNumberResult : TextParseResult
    {
        public NativeType NativeType;
        public NativeNumberGroup NativeNumberType;

        public TextParseNumberResult(string Result, NativeType NativeType, NativeNumberGroup NativeNumberType) : base(Result)
        {
            this.NativeType = NativeType;
            this.NativeNumberType = NativeNumberType;
        }

        public TextParseNumberResult(TextParseResultCode FailedResultCode) : base(FailedResultCode)
        { }
    }

    public class TextParseCharResult : TextParseResult
    {
        public CharType CharType;

        public TextParseCharResult(string Result, CharType CharType) : base(Result)
        {
            this.CharType = CharType;
        }

        public TextParseCharResult(TextParseResultCode FailedResultCode) : base(FailedResultCode)
        { }
    }
}
