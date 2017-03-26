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
            if(Expression.OperationList == null)
            {
                return left;
            }

            ExpressionResultType result = null;
            bool hasResult = false;
            for (int i=0; i<Expression.OperationList.Size; i++)
            {
                ExpressionOperationPair operationPair = Expression.OperationList.Get(i);
                OperationSymbol operation = operationPair.Operation.Token as OperationSymbol;

                ExpressionResultType right = ValidateExpression(operationPair.ExpressionPair);

                if(operation.IsCategory(OperationCategory.LogicAndOr))
                {
                    result = null;
                    hasResult = true;
                }
                else if(operation.IsCategory(OperationCategory.LogicEqualNot))
                {
                    result = null;
                    hasResult = true;
                }
                else if(operation.IsCategory(OperationCategory.LogicRelationCompare))
                {
                    result = null;
                    hasResult = true;
                }
                else if(operation.IsCategory(OperationCategory.MathAssigment))
                {
                    if(!hasResult)
                    {
                        result = left;
                    }
                }
                else if(operation.IsCategory(OperationCategory.Assigment))
                {
                    if (!hasResult)
                    {
                        result = left;
                    }
                }
                else if(operation.IsCategory(OperationCategory.Math))
                {
                    if (!hasResult)
                    {
                        result = left;
                    }
                }
                else if(operation.IsCategory(OperationCategory.Type))
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
                OperandAccessSignature accessSignature = Operand.AccessList.Get(i);
                if(accessSignature.Type == SignatureType.LiteralOperand)
                {
                    LiteralSymbol literalSymbol = (accessSignature as LiteralOperand).Literal as LiteralSymbol;
                    result = null;
                }
                else if(accessSignature.Type == SignatureType.VariableOperand)
                {
                    string variableIdentifiere = (accessSignature as VariableOperand).Identifier.String;
                }
                else if(accessSignature.Type == SignatureType.FunctionOperand)
                {
                    FunctionOperand functionAccess = (accessSignature as FunctionOperand);
                }
                else if(accessSignature.Type == SignatureType.ArrayOperand)
                {
                    ArrayOperand arrayAccess = (accessSignature as ArrayOperand);
                }
                else
                {
                    //throw new Exception("invalid state");
                }

            }

            return result;
        }
    }
}
