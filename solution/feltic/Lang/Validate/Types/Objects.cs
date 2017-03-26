using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{ 
    public partial class Validator
    {
        public void ValidateObject(ObjectSymbol ObjectSymbol)
        {
            for (int i = 0; i < ObjectSymbol.MemberList.Size; i++)
            {
                MemberSymbol memberSymbol = ObjectSymbol.MemberList.Get(i);
                TypeDeclarationSignature typeDeclaration = memberSymbol.Signature.TypeDeclaration;
                if (typeDeclaration.TypeIdentifier != null)
                {
                    if (Registry.GetObjectSymbol(typeDeclaration.TypeIdentifier.String) == null)
                    {
                        ;
                    }
                    else if (memberSymbol.Signature.TypeDeclaration.AssigmentExpression != null)
                    {
                        ValidateExpression(memberSymbol.Signature.TypeDeclaration.AssigmentExpression);
                    }
                }
            }
            for (int i = 0; i < ObjectSymbol.MethodList.Size; i++)
            {
                MethodSymbol methodSymbol = ObjectSymbol.MethodList.Get(i);
                TypeDeclarationSignature typeDeclaration = methodSymbol.Signature.TypeDeclaration;
                if (typeDeclaration.TypeIdentifier != null)
                {
                    if (Registry.GetObjectSymbol(typeDeclaration.TypeIdentifier.String) == null)
                    {
                        ;
                    }
                }
                ParameterDeclarationSignature parameterDeclaration = methodSymbol.Signature.ParameterDeclaration;
                if (parameterDeclaration != null)
                {
                    for (int j = 0; j < parameterDeclaration.Elements.Size; j++)
                    {
                        ParameterSignature parameter = parameterDeclaration.Elements.Get(j);
                        TypeDeclarationSignature parameterTypeDeclaration = parameter.TypeDeclaration;
                        if (parameterTypeDeclaration.TypeIdentifier != null)
                        {
                            if (Registry.GetObjectSymbol(parameterTypeDeclaration.TypeIdentifier.String) == null)
                            {
                                ;
                            }
                        }
                    }
                }
                if (methodSymbol.Code != null)
                {
                    CodeSignature codeSignature = methodSymbol.Code.Signature;
                    ValidateStatementBlock(codeSignature.Elements);
                }
            }
        }

    }
}
