using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Runtime.Types
{
    public class ArrayType
    {
        public string TypeName;
        public ObjectSymbol ObjectType;
        public int DimensionCount = 0;
        public ListCollection<ulong> DimensionDepthList = new ListCollection<ulong>();
        public ArrayNodeType InitialisationRootNode = null;
    }

    public class ArrayNodeType
    {
        public ArrayNodeType ParentNode;
        public ArrayNodeCollection ChildNodes = new ArrayNodeCollection();
        public ExpressionCollection InitialisationExpressionList = new ExpressionCollection();

        public ArrayNodeType(ArrayNodeType Parent)
        {
            this.ParentNode = Parent;
        }
    }

    public class ArrayNodeCollection : ListCollection<ArrayNodeType>
    { }

    public class ArrayParameterCollection : ListCollection<ArrayParameter>
    { }

    public class ArrayParameter
    {
        public ExpressionType ParameterExpression;

        public ArrayParameter(ExpressionType ParameterExpression)
        {
            this.ParameterExpression = ParameterExpression;
        }
    }
}
