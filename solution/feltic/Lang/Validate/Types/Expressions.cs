using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public class ExpressionResultType
    {
        public NativeSymbol NativeSymbol;
        public string ObjectName;

        public ExpressionResultType(NativeSymbol NativeSymbol)
        {
            this.NativeSymbol = NativeSymbol;
        }

        public ExpressionResultType(string ObjectName)
        {
            this.ObjectName = ObjectName;
        }

        public bool IsNative()
        {
            return (NativeSymbol != null);
        }

        public bool isObject()
        {
            return (ObjectName != null);
        }
    }

    public partial class Validator
    {
        public ExpressionResultType ValidateExpression(ExpressionSignature Expression)
        {
            ExpressionResultType left;
            if(Expression.ChildExpression != null)
            {
                left = ValidateExpression(Expression.ChildExpression);
            }
            else
            {
                left = ValidateOperand(Expression.Operand);
            }
            if(Expression.OperationList.Size == 0)
            {
                return left;
            }

            ExpressionResultType result = null;
            bool hasResult = false;
            for (int i=0; i<Expression.OperationList.Size; i++)
            {
                ExpressionOperationPair operationPair = Expression.OperationList.Get(i);
                OperationSymbol operation = operationPair.Operation.Token.Symbol as OperationSymbol;

                ExpressionResultType right = ValidateExpression(operationPair.ExpressionPair);

                if(operation.Group == OperationGroup.LogicAndOr)
                {
                    result = new ExpressionResultType(Natives.EnumMap.GetValue(NativeType.Bool));
                    hasResult = true;
                }
                else if(operation.Group == OperationGroup.LogicEqualNot)
                {
                    result = new ExpressionResultType(Natives.EnumMap.GetValue(NativeType.Bool));
                    hasResult = true;
                }
                else if(operation.Group == OperationGroup.LogicRelationCompare)
                {
                    result = new ExpressionResultType(Natives.EnumMap.GetValue(NativeType.Bool));
                    hasResult = true;
                }
                else if(operation.Group == OperationGroup.MathAssigment)
                {
                    if(!hasResult)
                    {
                        result = left;
                    }
                }
                else if(operation.Group == OperationGroup.Assigment)
                {
                    if (!hasResult)
                    {
                        result = left;
                    }
                }
                else if(operation.Group == OperationGroup.Math)
                {
                    if (!hasResult)
                    {
                        result = left;
                    }
                }
                else if(operation.Group == OperationGroup.Type)
                {
                    ;
                }
                else
                {
                    throw new Exception("invalid state");
                }

                left = right;
            }

            return result;
        }

        public ExpressionResultType ValidateOperand(OperandSignature Operand)
        {
            ExpressionResultType result = null;
            for(int i=0; i<Operand.AccessList.Size; i++)
            {
                AccessSignature accessSignature = Operand.AccessList.Get(i);
                if(accessSignature.Type == SignatureType.LiteralAccess)
                {
                    LiteralSymbol literalSymbol = (accessSignature as LiteralAccessSignature).Literal.Symbol as LiteralSymbol;
                    result = new ExpressionResultType(Natives.EnumMap.GetValue(NativeType.Number));
                }
                else if(accessSignature.Type == SignatureType.VariableAccess)
                {
                    string variableIdentifiere = (accessSignature as VariableAccessSignature).Identifier.String;
                }
                else if(accessSignature.Type == SignatureType.FunctionAccess)
                {
                    FunctionAccessSignature functionAccess = (accessSignature as FunctionAccessSignature);
                }
                else if(accessSignature.Type == SignatureType.ArrayAccess)
                {
                    ArrayAccessSignature arrayAccess = (accessSignature as ArrayAccessSignature);
                }
                else
                {
                    throw new Exception("invalid state");
                }

            }

            return result;
        }
    }
}
