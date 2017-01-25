
namespace Bee.Library
{
    public static class StringUtils
    {
        public static int CountSearchString(string str, string search)
        {
            if(str == null || str.Length == 0)
            {
                return 0;
            }
            int count = 0;
            for(int i=0; i<str.Length;)
            {
                if(i + search.Length > str.Length)
                {
                    break;
                }
                bool match = true;
                for(int j=0; j<search.Length; i++, j++)
                {
                    if(str[i] != search[j])
                    {
                        match = false;
                        break;
                    }
                }
                if(match)
                {
                    count++;
                }
                else
                {
                    i++;
                }
            }
            return count;
        }

        public static string[] SplitSearchString(string str, string search)
        {
            if(str == null || str.Length == 0)
            {
                return new string[]{};
            }
            int count = CountSearchString(str, search);
            if(count == 0)
            {
                return new string[] { str };
            }
            string[] splitArray = new string[count];
            int idx = 0;
            int begin = 0;
            for (int i = 0; i < str.Length;)
            {
                if (i + search.Length > str.Length)
                {
                    break;
                }
                bool match = true;
                begin = i;
                for (int j = 0; j < search.Length; i++, j++)
                {
                    if (str[i] != search[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    splitArray[idx] = str.Substring(begin, search.Length);
                    idx++;
                }
                else
                {
                    i++;
                }
            }
            return splitArray;
        }
    }
}
