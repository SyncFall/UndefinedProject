using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Runtime.Types
{
    public class VariableCollection : ListCollection<VariableType>
    {
        public bool EqualName(VariableType matchVariable)
        {
            for(int i=0; i < this.Size(); i++)
            {
                if (this.Get(i).EqualName(matchVariable))
                {
                    return true;
                }
            }
            return false;
        }

        public bool EqualName(string matchName)
        {
            for (int i = 0; i < this.Size(); i++)
            {
                if (this.Get(i).EqualName(matchName))
                {
                    return true;
                }
            }
            return false;
        }

        public VariableType GetByName(string matchName)
        {
            for (int i = 0; i < this.Size(); i++)
            {
                if (this.Get(i).EqualName(matchName))
                {
                    return this.Get(i);
                }
            }
            return null;
        }
    }

    public class VariableUtils
    {
        public static VariableCollection GetVariableCollection(StatementType statementType)
        {
            // get possible variable-declarations if matching statement-type
            VariableCollection localVariableCollection = null;
            // basic local variable declaration
            if (statementType.Type == StatementTypeEnum.DECLARATION)
            {
                localVariableCollection = (statementType as VariableDeclarationStatementType).VariableDeclarationCollection;
            }
            // for-loop scope
            else if (statementType.Type == StatementTypeEnum.FOR)
            {
                localVariableCollection = (statementType as ForLoopStatementType).VariableDeclarationCollection;
            }
            // for foreach-loop scope
            else if (statementType.Type == StatementTypeEnum.FOR_EACH)
            {
                // add to detection container
                localVariableCollection = new VariableCollection();
                localVariableCollection.Add((statementType as ForeachLoopStatementType).DeclarationVariable);
            }
            // for error-processing scope
            else if (statementType.Type == StatementTypeEnum.CATCH)
            {
                localVariableCollection = new VariableCollection();
                localVariableCollection.Add((statementType as ErrorProcessigStatementType).DeclarationVariable);
            }
            // return
            return localVariableCollection;
        }

        public static VariableType GetVariableType(CodeType codeType, StatementType statementType, string VariableName)
        {
            return GetVariableType(codeType.Statements, VariableName);
        }

        public static VariableType GetVariableType(StatementCollection statementCollection, string VariableName)
        {
            // foreach each statement from top-depth
            for (int i = 0; i < statementCollection.Size(); i++)
            {
                // check if variable declarated in statement-scope
                VariableCollection localVariableCollection = GetVariableCollection(statementCollection.Get(i));
                if (localVariableCollection != null)
                {
                    VariableType possibleVariableType = localVariableCollection.GetByName(VariableName);
                    if (possibleVariableType != null)
                    {
                        return possibleVariableType;
                    }
                }
                // check child-statements
                GetVariableType(statementCollection.Get(i).Statements, VariableName);
            }
            return null;
        }
    }

    public class VariableType
    {
        public bool IsConst;
        public string VariableName;
        public string TypeName;
        public ArrayType ArrayDeclarationType;
        public GenericType GenericDeclarationType;
        public ExpressionType InitialisationExpression;
        public ObjectSymbol ObjectType;

        public VariableType(string VariableName)
        {
            this.VariableName = VariableName;
        }

        public VariableType(string VariableName, string TypeName, bool IsConst)
        {
            this.VariableName = VariableName;
            this.TypeName = TypeName;
            this.IsConst = IsConst;
        }

        public VariableType(string VariableName, string TypeName, ExpressionType InitialisationExpression, bool IsConst)
        {
            this.VariableName = VariableName;
            this.TypeName = TypeName;
            this.InitialisationExpression = InitialisationExpression;
            this.IsConst = IsConst;
        }

        public bool EqualName(VariableType matchVariable)
        {
            return (
                this.VariableName.ToLower().Equals(matchVariable.VariableName.ToLower())
            );
        }

        public bool EqualName(string matchName)
        {
            return (
                this.VariableName.ToLower().Equals(matchName.ToLower())
            );
        }
    }
}
