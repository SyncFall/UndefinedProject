using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Runtime.Types;

namespace Be.Runtime.Validate
{
    public class GenericsValidator
    {
        private ObjectLoader objectLoader;

        public GenericsValidator(ObjectLoader objectLoader)
        {
            this.objectLoader = objectLoader;
        }

        public void ValidateGenericTypes(SourceFile sourceType, GenericType genericType, GenericsMode mode)
        {
            if (genericType == null)
            {
                return;
            }
            // check if any types exist
            if (genericType.ElementCollection.Size() == 0)
            {
                throw new Exception("no type in generic declaration");
            }
        }

        public void ValidateObjectDeclaration(SourceFile sourceType, ObjectSymbol objectType, GenericsMode mode)
        {
            // default generic validation
            ValidateGenericTypes(sourceType, objectType.GenericType, mode);
        }

        public void ValidateExtendGenericTypes(SourceFile sourceType, ObjectSymbol objectType, ExtendSymbol extendType, GenericsMode mode)
        {
            // default generic validation
            ValidateGenericTypes(sourceType, extendType.GenericType, mode);
            // check for possible object-types
            ValidateGenericObjectTypes(sourceType, objectType, extendType.GenericType);
        }

        public void ValidateImplementGenericTypes(SourceFile sourceType, ObjectSymbol objectType, ImplementCollection implementCollection, GenericsMode mode)
        {
            // check each implement-type
            for(int i=0; i< implementCollection.Size(); i++)
            {
                // default generic validation
                ValidateGenericTypes(sourceType, implementCollection.Get(i).GenericType, mode);
                // check for possible object-types
                ValidateGenericObjectTypes(sourceType, objectType, implementCollection.Get(i).GenericType);
            }
        }

        public void ValidateMemberGenericTypes(SourceFile sourceType, ObjectSymbol objectType, MemberCollection memberCollection, GenericsMode mode)
        {
            // check each member
            for (int i = 0; i < memberCollection.Size(); i++)
            {
                // default generic validation
                ValidateGenericTypes(sourceType, memberCollection.Get(i).GenericType, mode);
                // check for possible object-types
                ValidateGenericObjectTypes(sourceType, objectType, memberCollection.Get(i).GenericType);
            }
        }

        public void ValidateMethodGenericTypes(SourceFile sourceType, ObjectSymbol objectType, MethodCollection methodCollection, MethodCategory methodCategory, GenericsMode mode)
        {
            // check each method
            for (int i = 0; i < methodCollection.Size(); i++)
            {
                // default generic validation
                ValidateGenericTypes(sourceType, methodCollection.Get(i).GenericType, mode);
                // check for possible object-types
                ValidateGenericObjectAndMethodTypes(sourceType, objectType, methodCollection.Get(i), methodCategory, methodCollection.Get(i).GenericType);
            }
        }

        private void ValidateGenericObjectTypes(SourceFile sourceType, ObjectSymbol objectType, GenericType genericType)
        {
            if(genericType == null)
            {
                return;
            }
            // foreach generic element on same hiereachie
            for(int i=0; i<genericType.ElementCollection.Size(); i++)
            {
                GenericElementType genericElement = genericType.ElementCollection.Get(i);
                // check if possible placeholder-generic types exist in object-declaration
                if (objectType.GenericType != null && objectType.GenericType.FindTypeName(genericElement.TypeName))
                {
                    ; // ok
                }
                // else check if object declaration is an object-type
                else
                {
                    genericElement.ObjectType = objectLoader.GetObjectType(sourceType, genericElement.TypeName);
                    if (genericElement.ObjectType == null)
                    {
                        throw new Exception("generics-type not found");
                    }
                }
                // also check for possible extend types
                if (genericElement.ExtendTypeName != null)
                {
                    genericElement.ExtendObjectType = objectLoader.GetObjectType(sourceType, genericElement.ExtendTypeName);
                    if (genericElement.ExtendObjectType == null)
                    {
                        throw new Exception("generics extends object-type not found");
                    }
                }
                // check possible childs elements
                ValidateGenericObjectTypes(sourceType, objectType, genericElement.GenericType);
            }
        }

        private void ValidateGenericObjectAndMethodTypes(SourceFile sourceType, ObjectSymbol objectType, MethodType methodType, MethodCategory methodCategory, GenericType genericType)
        {
            if (genericType == null)
            {
                return;
            }
            // foreach generic element on same hiereachie
            for (int i = 0; i < genericType.ElementCollection.Size(); i++)
            {
                GenericElementType genericElement = genericType.ElementCollection.Get(i);
                // check if possible placeholder-generic types exist in object-declaration or method-declaration
                if (
                    (objectType.GenericType != null && objectType.GenericType.FindTypeName(genericElement.TypeName)) ||
                    (methodCategory != MethodCategory.CONSTRUCTOR && methodType.GenericType != null && methodType.GenericType.FindTypeName(genericElement.TypeName))
                ){
                    ; // ok
                }
                // else check if object declaration is an object-type
                else
                {
                    genericElement.ObjectType = objectLoader.GetObjectType(sourceType, genericElement.TypeName);
                    if (genericElement.ObjectType == null)
                    {
                        throw new Exception("generics-type not found");
                    }
                }
                // extends-types not allowed for constructors
                if (methodCategory == MethodCategory.CONSTRUCTOR && genericElement.ExtendTypeName != null)
                {
                    throw new Exception("generic-extend-types on object types not allowed for constructors");
                }
                // also check for possible extend types
                if (genericElement.ExtendTypeName != null)
                {
                    genericElement.ExtendObjectType = objectLoader.GetObjectType(sourceType, genericElement.ExtendTypeName);
                    if (genericElement.ExtendObjectType == null)
                    {
                        throw new Exception("generics extends object-type not found");
                    }
                }
                // check possible childs elements
                ValidateGenericObjectTypes(sourceType, objectType, genericElement.GenericType);
            }
        }
    }
}
