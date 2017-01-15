using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Runtime.Types;

namespace Be.Runtime.Validate
{
    // check for exist-types and duplication-signatures, some generics-type referencing from object-types
    public class InterfaceValidator
    {
        private ObjectLoader objectLoader;
        private GenericsValidator genericsValidator;

        public InterfaceValidator(ObjectLoader objectLoader)
        {
            this.objectLoader = objectLoader;
            this.genericsValidator = new GenericsValidator(this.objectLoader);
        }
        
        public void ValidateSourceType(SourceFile sourceType)
        {
            // check objects
            for(int i=0; i<sourceType.Namespaces.Size(); i++)
            {
                ObjectCollection objects = sourceType.Namespaces.Get(i).Objects;
                for (int j=0; j< objects.Size(); j++)
                {
                    ValidateObjectType(sourceType, objects.Get(j));
                }
            }
        }

        private void ValidateObjectType(SourceFile sourceType, ObjectSymbol objectType)
        {
            // check attributes
            ValidateAttributeTypes(sourceType, objectType.Attributes);
            // check generics
            genericsValidator.ValidateObjectDeclaration(sourceType, objectType, GenericsMode.DECLARATION);
            // check extends
            ValidateExtendType(sourceType, objectType, objectType.ExtendType);
            // check implements
            ValidateImplementTypes(sourceType, objectType, objectType.ImplementType);
            // check for duplicate name signaturs
            ValidateObjectNameSignaturs(sourceType, objectType);
            // check members
            ValidateMemberTypes(sourceType, objectType);
            // check constructors
            ValidateConstructorTypes(sourceType, objectType);
            // check methods
            ValidateMethodTypes(sourceType, objectType);
            // check properties
            ValidatePropertyTypes(sourceType, objectType);
            // check child objects
            for (int i = 0; i < objectType.Objects.Size(); i++)
            {
                ValidateObjectType(sourceType, objectType.Objects.Get(i));
            }
        }

        private void ValidateAttributeTypes(SourceFile sourceType, AttributeType attributeType)
        {
            if(attributeType == null)
            {
                return;
            }
            // check if any types exist
            if (attributeType.ElementCollection.Size() == 0)
            {
                throw new Exception("no type in attribute declaration");
            }
            for (int i=0; i<attributeType.ElementCollection.Size(); i++)
            {
                AttributeItem attributeItem = attributeType.ElementCollection.Get(i);
                // check if attribute object-type exist
                attributeItem.ObjectType = objectLoader.GetObjectType(sourceType, attributeItem.TypeName);
                if(attributeItem.ObjectType == null)
                {
                    throw new Exception("attribute-type not exist");
                }
            }
        }

        private void ValidateExtendType(SourceFile sourceType, ObjectSymbol objectType, ExtendSymbol extendType)
        {
            if (extendType == null)
            {
                return;
            }
            // set possible extend-type object-parent
            extendType.Parent = objectType;
            // check if extend-type exist
            extendType.ObjectType = objectLoader.GetObjectType(sourceType, extendType.TypeName);
            if (extendType.ObjectType == null)
            {
                throw new Exception("extend-type do not exist");
            }
            // check possible generics types
            if(extendType.GenericType != null)
            {
                genericsValidator.ValidateExtendGenericTypes(sourceType, objectType, extendType, GenericsMode.DECLARATION);
            }
        }

        private void ValidateImplementTypes(SourceFile sourceType, ObjectSymbol objectType, ImplementCollection implementCollection)
        {
            if (implementCollection == null)
            {
                return;
            }
            // check each implement-type
            for (int i = 0; i < implementCollection.Size(); i++)
            {
                ImplementSymbol implementType = implementCollection.Get(i);
                // set possible implement-type object-parent
                implementType.Parent = objectType;
                // check for duplicated signatures
                for (int j = i + 1; j < implementCollection.Size(); j++)
                {
                    if(implementType.Path.ToLower().Equals(implementCollection.Get(j).Path.ToLower()))
                    {
                        throw new Exception("duplicate implement signatur");
                    }
                }                
                // check if implement type exist
                implementType.ObjectType = objectLoader.GetObjectType(sourceType, implementType.Path);
                if (implementType.ObjectType == null)
                {
                    throw new Exception("implements-type do not exist");
                }
                // check possible generics types
                if (implementType.GenericType != null)
                {
                    genericsValidator.ValidateImplementGenericTypes(sourceType, objectType, implementCollection, GenericsMode.DECLARATION);
                }
            }
        }

        public void ValidateObjectNameSignaturs(SourceFile sourceType, ObjectSymbol objectType)
        {
            MapCollection<string, bool> nameMap = new MapCollection<string, bool>();

            // check members (against members)
            for(int i=0; i<objectType.Members.Size(); i++)
            {
                MemberType memberType = objectType.Members.Get(i);
                if (nameMap.KeyExist(memberType.MemberName.ToLower()))
                {
                    throw new Exception("member-name already defined");
                }
                nameMap.Add(memberType.MemberName.ToLower(), true);
            }

            // check properties (against members and properties)
            for (int i = 0; i < objectType.Properties.Size(); i++)
            {
                MethodType propertyType = objectType.Properties.Get(i);
                if (nameMap.KeyExist(propertyType.Name.ToLower()))
                {
                    throw new Exception("property-name already defined");
                }
                nameMap.Add(propertyType.Name.ToLower(), true);
            }

            // check constructors (against members and properties)
            for (int i=0; i<objectType.Constructors.Size(); i++)
            {
                MethodType constructorType = objectType.Constructors.Get(i);
                if(nameMap.KeyExist(constructorType.Name.ToLower()))
                {
                    throw new Exception("constructor-name already defined");
                }
            }

            // check methods (against members and properties)
            for (int i = 0; i < objectType.Methods.Size(); i++)
            {
                MethodType methodType = objectType.Methods.Get(i);
                if (nameMap.KeyExist(methodType.Name))
                {
                    throw new Exception("method-name already defined");
                }
            }
        }

        private void ValidateMemberTypes(SourceFile sourceType, ObjectSymbol objectType)
        {
            MemberType memberType;
            // check member types and names
            for (int i = 0; i < objectType.Members.Size(); i++)
            {
                memberType = objectType.Members.Get(i);
                // check attributes
                ValidateAttributeTypes(sourceType, memberType.Attributes);
                // check for member object-type
                memberType.ObjectType = objectLoader.GetObjectType(sourceType, memberType.TypeName);
                if(memberType.ObjectType == null)
                {
                    throw new Exception("member object-type not exit");
                }
                // check for generics member-type
                if(memberType.GenericType != null)
                {
                    genericsValidator.ValidateMemberGenericTypes(sourceType, objectType, objectType.Members, GenericsMode.DECLARATION);
                }
            }
        }

        private void ValidateConstructorTypes(SourceFile sourceType, ObjectSymbol objectType)
        {
            if (objectType.IsInterface && objectType.Constructors.Size() > 0)
            {
                throw new Exception("interfaces can not implement constructors");
            }
            MethodType constructorType;
            for (int i = 0; i < objectType.Constructors.Size(); i++)
            {
                constructorType = objectType.Constructors.Get(i);
                // check attributes
                ValidateAttributeTypes(sourceType, constructorType.Attributes);
                // check if name match object-type name
                if (!constructorType.Name.ToLower().Equals(objectType.String.ToLower()))
                {
                    throw new Exception("constructor name missmatch object-name");
                }
                // check for duplicate constructor signatur
                for (int j = i + 1; j < objectType.Constructors.Size(); j++)
                {
                    if (constructorType.EqualBaiscConstructorSignatur(objectType.Constructors.Get(j)))
                    {
                        throw new Exception("duplicate method-definition");
                    }
                }
                // check generics-types
                genericsValidator.ValidateMethodGenericTypes(sourceType, objectType, objectType.Constructors, MethodCategory.CONSTRUCTOR, GenericsMode.DECLARATION);
                // check if parameter-list object-types exists
                ValidateParamaterCollection(sourceType, objectType, constructorType, constructorType.ParameterCollection);
            }
        }

        private void ValidateMethodTypes(SourceFile sourceType, ObjectSymbol objectType)
        {
            for (int i = 0; i < objectType.Methods.Size(); i++)
            {
                MethodType methodType = objectType.Methods.Get(i);
                // check attributes
                ValidateAttributeTypes(sourceType, methodType.Attributes);
                // check for dublicate method signatur definition
                for (int j = i + 1; j < objectType.Methods.Size(); j++)
                {
                    if (methodType.EqualBasicSignatur(objectType.Methods.Get(j)))
                    {
                        throw new Exception("duplicate method-definition");
                    }
                }
                // check generics-types
                genericsValidator.ValidateMethodGenericTypes(sourceType, objectType, objectType.Methods, MethodCategory.METHOD, GenericsMode.DECLARATION);
                // check for possible return object-type
                ObjectSymbol returnObjectType = objectLoader.GetObjectType(sourceType, methodType.ReturnType);
                if (returnObjectType != null)
                {
                    methodType.ReturnObjectType = returnObjectType;
                }
                // may an return generic-type 
                else
                {
                    // check for method or object declaration
                    if(!((methodType.GenericType != null && methodType.GenericType.FindTypeName(methodType.ReturnType)) ||
                        objectType.GenericType != null && objectType.GenericType.FindTypeName(methodType.ReturnType)))
                    {
                        throw new Exception("parameter-generic-type not exist in object-method-declaration");
                    }
                    // set generic-type-name from definition
                    methodType.ReturnGenericTypeName = methodType.ReturnType;
                }
                // check if parameter-list object-types exists
                ValidateParamaterCollection(sourceType, objectType, methodType, methodType.ParameterCollection);
            }
        }

        private void ValidatePropertyTypes(SourceFile sourceType, ObjectSymbol objectType)
        {
            for (int i = 0; i < objectType.Properties.Size(); i++)
            {
                PropertySymbol property = objectType.Properties.Get(i) as PropertySymbol;
                // check attributes
                ValidateAttributeTypes(sourceType, property.Attributes);
                // check for dublicate method signatur definition
                for (int j = i + 1; j < objectType.Properties.Size(); j++)
                {
                    if (property.EqualBasicSignatur(objectType.Properties.Get(j)))
                    {
                        throw new Exception("duplicate property-definition");
                    }
                }
                // check generics-types
                genericsValidator.ValidateMethodGenericTypes(sourceType, objectType, objectType.Properties, MethodCategory.PROPERTY, GenericsMode.DECLARATION);
                // check for possible return object-type
                ObjectSymbol returnObjectType = objectLoader.GetObjectType(sourceType, property.ReturnType);
                if (returnObjectType != null)
                {
                    property.ReturnObjectType = returnObjectType;
                }
                // may an return generic-type 
                else
                {
                    // check for method or object declaration
                    if (!((property.GenericType != null && property.GenericType.FindTypeName(property.ReturnType)) ||
                        objectType.GenericType != null && objectType.GenericType.FindTypeName(property.ReturnType)))
                    {
                        throw new Exception("parameter-generic-type not exist in object-method-declaration");
                    }
                    // set generic-type-name from definition
                    property.ReturnGenericTypeName = property.ReturnType;
                }
            }
        }

        private void ValidateParamaterCollection(SourceFile sourceType, ObjectSymbol objectType, MethodType methodType, ParameterCollection ParameterCollection)
        {
            // check for duplicate signatures
            for(int i=0; i<ParameterCollection.Size(); i++)
            {
                for(int j=i+1; j<ParameterCollection.Size(); j++)
                {
                    if (ParameterCollection.Get(i).EqualVariableName(ParameterCollection.Get(j)))
                    {
                        throw new Exception("duplicate parameter-variable declaration");
                    }
                }
            }
            // check if parameter-list object-types or generics-types exists
            ParameterType parameterType;
            for (int j = 0; j < ParameterCollection.Size(); j++)
            {
                parameterType = ParameterCollection.Get(j);
                ObjectSymbol parameterObjectType = objectLoader.GetObjectType(sourceType, parameterType.TypeName);
                // is an object-type
                if (parameterObjectType != null)
                {
                    parameterType.ObjectType = parameterObjectType;
                }
                // may an generic-type 
                else
                {
                    // check for method or object generic declaration exist
                    if (!((methodType.GenericType != null && methodType.GenericType.FindTypeName(parameterType.TypeName)) ||
                          (objectType.GenericType != null && objectType.GenericType.FindTypeName(parameterType.TypeName)))
                    ){
                        throw new Exception("parameter-generic-type not exist in object or method-declaration");
                    }
                    // set generic-type-name from declaration
                    parameterType.GenericTypeName = parameterType.TypeName;
                }
            }
        }
    }
}
