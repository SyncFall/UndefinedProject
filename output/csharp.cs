using System;
using System.IO;
using System.Collections;

namespace Scope
{
	public class Fucking
	{
		public int i = 0;
		public string str = "string";
		public bool b;

		public void method(int cnt, string text)
		{
			i  = (cnt % 2);
			str  = text;
			if(i > 0 && i < 100 && true)
			{
				for(int j = 0;j < 100;j += 1)
				{
					;
				}
			}
		}
	}
}
