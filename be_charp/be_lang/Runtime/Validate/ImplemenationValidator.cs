using System;
using Be.Runtime.Types;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Runtime.Validate
{
    // check for valid oop-implementation from object-hierachie
    public class ImplemenationValidator
    {
        private ObjectLoader objectLoader;
        private CodeValidator codeValidator;

        public ImplemenationValidator(ObjectLoader objectLoader)
        {
            this.objectLoader = objectLoader;
            this.codeValidator = new CodeValidator(this.objectLoader);
        }

        public void ValidateSourceType(SourceFile sourceType)
        {
            // check objects
            for(int i=0; i< sourceType.Namespaces.Size(); i++)
            {
                ObjectCollection objects = sourceType.Namespaces.Get(i).Objects;
                for(int j=0; j<objects.Size(); j++)
                {
                    ValidateObjectType(sourceType, objects.Get(j));
                }
            }
        }

        private void ValidateObjectType(SourceFile sourceType, ObjectSymbol objectType)
        {
            // check attribute-types
            ValidateAttributeTypes(sourceType, objectType.Attributes);
            // check extend-type
            ValidateObjectImplementation(sourceType, objectType);
            // check child objects
            for (int i = 0; i < objectType.Objects.Size(); i++)
            {
                ValidateObjectType(sourceType, objectType.Objects.Get(i));
            }
        }

        private void ValidateAttributeTypes(SourceFile sourceType, AttributeType attributeType)
        {
            if (attributeType == null)
            {
                return;
            }
            // check compilant native type matching
            for (int i = 0; i < attributeType.ElementCollection.Size(); i++)
            {
                AttributeItem attributeItem = attributeType.ElementCollection.Get(i);
                // check if match any constructor-signatur by native-operand-type (positional-parameter)
                if (!attributeItem.ObjectType.Constructors.EqualNativeSignatur(attributeItem.DeclarationType.ConstructorDeclarationList))
                {
                    throw new Exception("attribute-declaration match not any attribute-constructor");
                }
                // check if match all member names and types (named-parameter)
                else if (!attributeItem.ObjectType.Members.MatchNativeSignaturOrNone(attributeItem.DeclarationType.MemberDeclarationMap))
                {
                    throw new Exception("attribute-declaration match not any member-name, member-type or public-signatur");
                }
            }
        }

        private void ValidateEnumType(SourceFile sourceType, EnumType enumType)
        {
            if (enumType == null)
            {
                return;
            }
            // check compilant native type matching
            for (int i = 0; i < enumType.ItemCollection.Size(); i++)
            {
                EnumItemType enumItemType = enumType.ItemCollection.Get(i);
                // check for name-duplicates
                for(int j = i + 1; j < enumType.ItemCollection.Size(); j++)
                {
                    if (enumType.ItemCollection.Get(i).EnumItemName.ToLower().Equals(enumType.ItemCollection.Get(j).EnumItemName.ToLower()))
                    {
                        throw new Exception("duplicate enum-iten name");
                    }
                }
                // check if match any constructor-signatur by native-operand-type (positional-parameter)
                if (!enumType.Constructors.EqualNativeSignatur(enumItemType.DeclarationType.ConstructorDeclarationList))
                {
                    throw new Exception("enum-declaration match not any enum-constructor");
                }
                // check if match all member names and types (named-parameter)
                else if (!enumType.Members.MatchNativeSignaturOrNone(enumItemType.DeclarationType.MemberDeclarationMap))
                {
                    throw new Exception("enum-declaration match not any member-name, member-type or public-signatur");
                }
            }
        }

        private void ValidateObjectImplementation(SourceFile sourceType, ObjectSymbol objectType)
        {
            // check if possible generic-signatur match object and extend object-type
            if (objectType.ExtendType != null)
            {
                if (objectType.ExtendType.GenericType != null &&
                   !objectType.ExtendType.GenericType.EqualSignatur(objectType.ExtendType.ObjectType.GenericType))
                {
                    throw new Exception("extend type match not object generic signatur type");
                }
            }

            // check foreach possible generic-signatur match object and implements object-types
            if (objectType.ImplementType != null)
            {
                for (int i = 0; i < objectType.ImplementType.Size(); i++)
                {
                    ImplementSymbol implementType = objectType.ImplementType.Get(i);
                    if (implementType.GenericType != null &&
                       !implementType.GenericType.EqualSignatur(implementType.ObjectType.GenericType))
                    {
                        throw new Exception("implement type match not object generic signatur type");
                    }
                }
            }

            // check possible enum-type
            if (objectType.IsEnum)
            {
                ValidateEnumType(sourceType, objectType as EnumType);
            }

            // foreach implemented object-constructor
            ValidateMethodImplementation(sourceType, objectType, objectType.Constructors);

            // foreach implemented object-method
            ValidateMethodImplementation(sourceType, objectType, objectType.Methods);

            // foreach implemented object-properties
            ValidateMethodImplementation(sourceType, objectType, objectType.Properties);
        }

        public void ValidateMethodImplementation(SourceFile sourceType, ObjectSymbol objectType, MethodCollection methodCollection)
        {
            MapCollection<string, bool> nameMap = new MapCollection<string, bool>();

            // get member-names
            for (int i = 0; i < objectType.Members.Size(); i++)
            {
                MemberType memberType = objectType.Members.Get(i);
                if (nameMap.KeyExist(memberType.MemberName.ToLower()))
                {
                    throw new Exception("member-name already defined");
                }
                nameMap.Add(memberType.MemberName.ToLower(), true);
            }

            // for each object-implemented method
            for (int i=0; i < methodCollection.Size(); i++)
            {
                MethodType method = methodCollection.Get(i);

                // validate attribute-types
                ValidateAttributeTypes(sourceType, method.Attributes);

                // only one method/constructor/property can declareted, in extend or implement hierachie
                bool alreadyDefined = false; 

                // for each possible extend-hierachie from top-depth
                ExtendSymbol extendType = objectType.ExtendType;
                ObjectSymbol extendObjectType = (extendType != null ? extendType.ObjectType : null);
                while (extendObjectType != null)
                {
                    // foreach extend constructor/method/property
                    MethodCollection extendMethodCollection = MethodUtils.GetMethodCollectionByCategoryFromObjectType(method, extendObjectType);
                    for (int j = 0; j < extendMethodCollection.Size(); j++)
                    {
                        MethodType extendMethod = extendMethodCollection.Get(j);
                        // check for same name
                        if(nameMap.KeyExist(extendMethod.Name.ToLower()))
                        {
                            throw new Exception("extending-method-name already defined as member");
                        }
                        // check for same signatur
                        if (EqualExtendOrImplementMethodImplementation(method, extendMethod, extendType, null))
                        {
                            // check if already defined
                            if(alreadyDefined)
                            {
                                throw new Exception("constructor/method/property in extend-hiereachie already defined");
                            }
                            alreadyDefined = true;                            
                            // validate extend-rules
                            ValidateExtendOrImplementMethodRules(method, extendMethod);
                        }
                    }
                    // go-in-depth
                    extendObjectType = (extendObjectType.ExtendType != null ? extendObjectType.ExtendType.ObjectType : null);
                }

                // for each possible implement-hierachie from top-depth
                if (objectType.ImplementType != null)
                {
                    ImplementSymbol implementType;
                    ObjectSymbol implementObjectType;
                    // foreach implement-type from top hierachie
                    for (int j = 0; j < objectType.ImplementType.Size(); j++)
                    {
                        implementType = objectType.ImplementType.Get(j);
                        implementObjectType = implementType.ObjectType;
                        while (implementObjectType != null)
                        {
                            // foreach implement method
                            MethodCollection implementMethodCollection = MethodUtils.GetMethodCollectionByCategoryFromObjectType(method, implementObjectType);
                            for (int k = 0; k < implementMethodCollection.Size(); k++)
                            {
                                MethodType implementMethod = implementMethodCollection.Get(k);
                                // check for same name
                                if (nameMap.KeyExist(implementMethod.Name.ToLower()))
                                {
                                    throw new Exception("implementing-method-name already defined as member");
                                }
                                // check for same signatur
                                if (EqualExtendOrImplementMethodImplementation(method, implementMethod, null, implementType))
                                {
                                    // check if already defined
                                    if (alreadyDefined)
                                    {
                                        throw new Exception("constructor/method/property in implement-hiereachie already defined");
                                    }
                                    alreadyDefined = true;
                                    // validate implement-rules
                                    ValidateExtendOrImplementMethodRules(method, implementMethod);
                                }
                            }
                            // go-in-depth
                            implementObjectType = (implementObjectType.ExtendType != null ? implementObjectType.ExtendType.ObjectType : null);
                        }
                    }
                }
            }
        }

        public void ValidateExtendOrImplementMethodRules(MethodType method, MethodType extendOrImplementMethod)
        {
            // exist must marked as virtual and can not marked as overriden
            if (!extendOrImplementMethod.IsVirtual || extendOrImplementMethod.IsOverride)
            {
                throw new Exception("extend methods must marked as abstract and can not marked as overriden");
            }
            // exist can not marked as static
            else if (extendOrImplementMethod.IsStatic)
            {
                throw new Exception("extend methods can not marked as static");
            }
            // must match extern 
            else if (method.IsExtern != extendOrImplementMethod.IsExtern)
            {
                throw new Exception("extend method extern missmatch");
            }
            // exist can not private
            else if (extendOrImplementMethod.Accessor.Type == AccessorTypeEnum.PRIVATE)
            {
                throw new Exception("extend method can not marked as private");
            }
        }

        public bool EqualExtendOrImplementMethodImplementation(MethodType method, MethodType equalMethod, ExtendSymbol extendType, ImplementSymbol implementType)
        {
            // check name
            if (!method.Name.ToLower().Equals(equalMethod.Name.ToLower()))
            {
                return false;
            }

            // check method/property-return type
            if(method.IsMethod || method.IsProperty)
            {
                bool returnMatch = false;

                // must match possible return object-type
                if ((method.ReturnObjectType != null && equalMethod.ReturnObjectType != null) &&
                   (method.ReturnObjectType.Name.Equals(equalMethod.ReturnObjectType.Name))
                ){
                    returnMatch = true;
                }

                // or must match possible generic return placeholder per method or object declaration
                else if(method.ReturnGenericTypeName != null && equalMethod.ReturnGenericTypeName != null)
                {
                    // from same method generics
                    if (method.GenericType != null && equalMethod.GenericType != null)
                    {
                        if (method.GenericType.FindTypeIndex(method.ReturnGenericTypeName).EqualIndex(equalMethod.GenericType.FindTypeIndex(equalMethod.ReturnGenericTypeName)))
                        {
                            returnMatch = true;
                        }
                    }
                    // or from same object extend generics
                    else if (extendType != null && extendType.GenericType != null && equalMethod.ObjectType.GenericType != null)
                    {
                        if (extendType.GenericType.FindTypeIndex(method.ReturnGenericTypeName).EqualIndex(extendType.ObjectType.GenericType.FindTypeIndex(equalMethod.ReturnGenericTypeName)))
                        {
                            returnMatch = true;
                        }
                    }
                    // or from same object implement generics
                    else if (implementType != null && implementType.GenericType != null && equalMethod.ObjectType.GenericType != null)
                    {
                        if (implementType.GenericType.FindTypeIndex(method.ReturnGenericTypeName).EqualIndex(implementType.ObjectType.GenericType.FindTypeIndex(equalMethod.ReturnGenericTypeName)))
                        {
                            returnMatch = true;
                        }
                    }
                }

                // default is no-compare
                if(!returnMatch)
                {
                    return false;
                }
            }

            // check method-generic-signatur
            if((method.GenericType != null && equalMethod.GenericType != null) &&
               (method.GenericType.EqualSignatur(equalMethod.GenericType))
            ){
                ; // ok
            }
            else if(method.GenericType == null && equalMethod.GenericType == null)
            {
                ; // ok
            }
            // no-compare
            else
            {
                return false;
            }

            // property has no parameters
            if(method.IsProperty)
            {
                return true;
            }
            // must match constructor/method-parameters
            else
            {
                return EqualParameterImplementation(method, equalMethod, extendType, implementType);
            }
        }

        private bool EqualParameterImplementation(MethodType method, MethodType extendOrImplementMethod, ExtendSymbol extendType, ImplementSymbol implementType)
        {
            // must match parameter count
            if (method.ParameterCollection.Size() != extendOrImplementMethod.ParameterCollection.Size())
            {
                return false;
            }
            // foreach parameter
            for (int i = 0; i < method.ParameterCollection.Size(); i++)
            {
                ParameterType methodParameter = method.ParameterCollection.Get(i);
                ParameterType extendOrImplementParameter = extendOrImplementMethod.ParameterCollection.Get(i);
                bool matchParameters = false;

                // must match possible generic parameter-type by object or method signatur
                if (methodParameter.GenericTypeName != null && extendOrImplementParameter.GenericTypeName != null)
                {
                    // from same method generics
                    if (method.GenericType != null && extendOrImplementMethod.GenericType != null)
                    {
                        if (method.GenericType.FindTypeIndex(methodParameter.TypeName).EqualIndex(extendOrImplementMethod.GenericType.FindTypeIndex(extendOrImplementParameter.TypeName)))
                        {
                            matchParameters = true;
                        }
                    }
                    // and from same object extend generics
                    else if (extendType != null && extendType.GenericType != null && extendOrImplementMethod.ObjectType.GenericType != null)
                    {
                        if (extendType.GenericType.FindTypeIndex(methodParameter.TypeName).EqualIndex(extendType.ObjectType.GenericType.FindTypeIndex(extendOrImplementParameter.TypeName)))
                        {
                            matchParameters = true;
                        }
                    }
                    // and from same object implement generics
                    else if (implementType != null && implementType.GenericType != null && extendOrImplementMethod.ObjectType.GenericType != null)
                    {
                        if (implementType.GenericType.FindTypeIndex(methodParameter.TypeName).EqualIndex(implementType.ObjectType.GenericType.FindTypeIndex(extendOrImplementParameter.TypeName)))
                        {
                            matchParameters = true;
                        }
                    }
                }

                // or must match possible object parameter type
                else if (methodParameter.TypeName != null && extendOrImplementParameter.TypeName != null)
                {
                    if(methodParameter.TypeName.Equals(extendOrImplementParameter.TypeName))
                    {
                        matchParameters = true;
                    }
                }

                // no-compare
                if (!matchParameters)
                {
                    return false;
                }
            }

            // pass
            return true;
        }
    }
}
