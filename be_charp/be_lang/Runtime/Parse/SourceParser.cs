using Be.Runtime.Types;
using System.IO;
using System;

namespace Be.Runtime.Parse
{ 
    public class SourceParser
    { 
        private TextParser textParser;
        private CodeParser codeParser;
        private ExpressionParser expressionParser;
        private GenericsParser genericsParser;
        private SourceFile sourceFile;
        private NamespaceSymbol currentNamespace;

        public SourceParser(SourceFile _sourceType)
        {  
            this.sourceFile = _sourceType;
            this.textParser = new TextParser(sourceFile.Source);
            this.codeParser = new CodeParser(textParser);
            this.expressionParser = new ExpressionParser(textParser);
            this.genericsParser = new GenericsParser(textParser);
        }

        public void ParseSource()
        { 
            textParser.SetPosition(0);
#if(TRACK)
            Utils.LogBranch("Perform Source-Text Parsing: Filename: '"+ Path.GetFileName(sourceFile.Filepath)+"'");
#endif
            while (true)
            {
                textParser.SkipSpace(true);
                                
                // check eof
                if (textParser.GetPosition() == textParser.GetLength())
                {
                    break;
                }
                // check namespace-directive in top-depth
                else if (textParser.EqualEndSpace(ObjectConst.Namespace, true))
                {
                    string namespacePath = textParser.GetPathContent(true);
                    if (namespacePath == null)
                    {
                        throw new Exception("namespace-pathname invalid");
                    }
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
                    {
                        throw new Exception("namespace missing enclosing-tag");
                    }
                    currentNamespace = new NamespaceSymbol(namespacePath);
                    sourceFile.Namespaces.Add(currentNamespace);
#if(TRACK)
                    Utils.LogItem("namespace: '" + namespacePath + "'");
#endif
                    // parse possible objects
                    ObjectSymbol objectSymbol = null;
                    while ((objectSymbol = ParseObject(0)) != null)
                    {
                        objectSymbol.SourceFile = sourceFile;
                        objectSymbol.Namespace = currentNamespace;
                        currentNamespace.Objects.Add(objectSymbol);
                    }
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                    {
                        throw new Exception("namespace missing enclosing-tag");
                    }
                }
                // check using-directive statement if in top-depth
                else if (textParser.EqualEndSpace(ObjectConst.Using, true))
                {
                    string usingPath = textParser.GetPathContent(true);
                    if (usingPath == null)
                    {
                        throw new Exception("using-directive is invalid");
                    }
                    if (!textParser.EqualNoneSpace(ObjectConst.StatementComplete, true))
                    {
                        throw new Exception("using-directive is not completed");
                    }
                    sourceFile.Usings.Add(new UsingSymbol(usingPath));
#if(TRACK)
                    Utils.LogItem("using: '" + usingPath + "'");
#endif
                }
                // error
                else
                {
                    throw new Exception("invalid source-file");
                }
            }
        }
        
        public ObjectSymbol ParseObject(int depth)
        {
            // get position
            int startPosition = textParser.GetPosition();
            // get posibble attributes
            AttributeType attribute = ParseAttribute();

            // get possible accessor
            AccessorType accessor = ParseAccessor();

            // object for possible definition-types
            bool isVirtual = false;
            if(textParser.EqualEndSpace(ObjectConst.Abstract, true))
            {
                isVirtual = true;
            }

            // check for class-statement
            ObjectCategory objectCategory = ObjectCategory.NONE;
            if (textParser.EqualEndSpace(ObjectConst.Object, true))
            {
                objectCategory = ObjectCategory.OBJECT;
            }
            else if(textParser.EqualEndSpace(ObjectConst.Interface, true))
            {
                objectCategory = ObjectCategory.INTERFACE;
            }
            else if(textParser.EqualEndSpace(ObjectConst.Enum, true))
            {
                objectCategory = ObjectCategory.ENUM;
            }
            else if(textParser.EqualEndSpace(ObjectConst.Attr, true))
            {
                objectCategory = ObjectCategory.ATTR;
            }
            else if(textParser.EqualEndSpace(ObjectConst.Exception, true))
            {
                objectCategory = ObjectCategory.EXCEPTION;
            }
            // no object-type
            else
            {
                // rewind position
                textParser.SetPosition(startPosition);
                // return none
                return null;
            }

            // get object-name
            string objectName = textParser.GetPathContent(true);
            if (objectName == null)
            {
                throw new Exception("invalid object-name");
            }

            // get possible generics
            GenericType generics = genericsParser.ParseGenericsDeclaration(GenericsMode.DECLARATION, GenericCategoryEnum.OBJECT);

            // check for possible extend-type
            ExtendSymbol extend = null;
            if (textParser.EqualEndSpace(ObjectConst.Extends, true))
            {
                string extendsTypeName = textParser.GetPathContent(true);
                if (extendsTypeName == null)
                {
                    throw new Exception("invalid object-extend type-name");
                }
                // parse possible generics
                GenericType extendGenericType = genericsParser.ParseGenericsDeclaration(GenericsMode.DECLARATION, GenericCategoryEnum.EXTEND);
                extend = new ExtendSymbol(extendsTypeName, extendGenericType);
            }

            // check for implement-list
            ImplementCollection implementList = ParseImplementCollection();

            // create specific object type
            ObjectSymbol objectItem = null;
            if (objectCategory == ObjectCategory.OBJECT)
            {
                objectItem = new ObjectSymbol(objectName, attribute, accessor, generics, extend, implementList, isVirtual);
            }
            else if (objectCategory == ObjectCategory.INTERFACE)
            {
                objectItem = new InterfaceType(objectName, attribute, generics, extend, isVirtual);
            }
            else if (objectCategory == ObjectCategory.ENUM)
            {
                objectItem = new EnumType(objectName, attribute, accessor, extend, implementList, isVirtual);
            }
            else if (objectCategory == ObjectCategory.ATTR)
            {
                objectItem = new AttrType(objectName, attribute, accessor, generics, extend, implementList, isVirtual);
            }
            else if(objectCategory == ObjectCategory.EXCEPTION)
            {
                objectItem = new ExcpetionType(objectName, attribute, accessor, generics, extend, implementList, isVirtual);
            }
#if (TRACK)
            Utils.LogItem(Utils.EnumToString(objectCategory) + ": '" + objectName + "' | accessor: '" + accessor + "' | virtual: '" + isVirtual + "' | extend: '" + extend + "' | implement: '" + implementList + "' | depth: " + (depth) + " |  attribute: '" + (attribute != null) + "'");
#endif   
            // get class-block start
            if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
            {
                throw new Exception("invalid object-block start");
            }

            // parse object-block items
            while (true)
            {
                textParser.SkipSpace(true);

                // parse possible enum-items
                if (objectCategory == ObjectCategory.ENUM)
                {
                    ParseEnumItems(objectItem as EnumType, textParser.GetPosition(), depth);
                }

                // check class-block end
                if (textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                {
                    return objectItem;
                }

                int itemStartPosition = textParser.GetPosition();

                // forward possible item-attribute
                ParseAttribute();

                // forward possible item-accessor
                ParseAccessor();

                // forward possible virtual
                textParser.EqualEndSpace(ObjectConst.Abstract, true);

                // forward possible override
                textParser.EqualEndSpace(ObjectConst.Override, true);
                
                // forward possible static
                textParser.EqualEndSpace(ObjectConst.Static, true);

                // forward possible extern
                textParser.EqualEndSpace(ObjectConst.Native, true);

                // forward possible readonky
                textParser.EqualEndSpace(ObjectConst.Readonly, true);

                // parse possible child object
                ObjectSymbol childObjectItem = ParseObject(depth + 1);
                if(childObjectItem != null)
                {
                    childObjectItem.SourceFile = sourceFile;
                    childObjectItem.Namespace = currentNamespace;
                    currentNamespace.Objects.Add(childObjectItem);
                    objectItem.Objects.Add(childObjectItem);
                }
                // or check for constructor/method/member/property
                else
                {
                    // check for valid-qualifier, must have one
                    string qualifierPath = textParser.GetPathContent(true);
                    if (qualifierPath == null)
                    {
                        throw new Exception("invalid object-item type-name");
                    }

                    // check for constructor
                    if (textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
                    {
                        ParseConstructor(itemStartPosition, objectItem);
                    }
                    // check if member/method/property
                    else
                    {
                        // check for space-elements, to exec further
                        if (!textParser.EqualSpace(true))
                        {
                            throw new Exception("invalid object-item parser-state (space missing)");
                        }
                        // get member/method/property - name
                        string qualifierPathSecond = textParser.GetPathContent(true);
                        if (qualifierPathSecond == null)
                        {
                            throw new Exception("invalid object-item member or method name");
                        }
                        
                        // forward possible generics
                        genericsParser.ParseGenericsDeclaration(GenericsMode.DECLARATION, GenericCategoryEnum.UNKNONW);
                        
                        // check for method
                        if (textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
                        {
                            ParseMethod(itemStartPosition, objectItem);
                        }
                        // check for property
                        else if(textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
                        {
                            ParseProperty(itemStartPosition, objectItem);
                        }
                        // check for member
                        else if (textParser.EqualNoneSpace(ObjectConst.StatementComplete, true) || textParser.EqualNoneSpace(ObjectConst.Assigment, true))
                        {
                            ParseMember(itemStartPosition, objectItem);
                        }
                        // invalid
                        else
                        {
                            throw new Exception("invalid object-item parser-state (syntax)");
                        }
                    }
                }
            }
        }

        public AnonObjectType ParseAnonymousObject(int startPosition)
        {
            // rewind position
            textParser.SetPosition(startPosition);

            // check for possible anonymous-type implementation
            if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, false))
            {
                return null;
            }

            AnonObjectType objectItem = new AnonObjectType();

            // parse object-block items
            while (true)
            {
                textParser.SkipSpace(true);

                // check class-block end
                if (textParser.Equal(ObjectConst.BlockDeclosing, true))
                {
                    return objectItem;
                }

                int itemStartPosition = textParser.GetPosition();

                // forward possible item-attribute
                ParseAttribute();

                // forward possible item-accessor
                ParseAccessor();

                // forward possible virtual
                textParser.EqualEndSpace(ObjectConst.Abstract, true);

                // forward possible override
                textParser.EqualEndSpace(ObjectConst.Override, true);

                // forward possible static
                textParser.EqualEndSpace(ObjectConst.Static, true);

                // forward possible extern
                textParser.EqualEndSpace(ObjectConst.Native, true);

                // forward possible readonky
                textParser.EqualEndSpace(ObjectConst.Readonly, true);

                // check for valid-qualifier, must have one
                string qualifierPath = textParser.GetPathContent(true);
                if (qualifierPath == null)
                {
                    throw new Exception("invalid object-item type-name");
                }
                // check for space-elements, to exec further
                if (!textParser.EqualSpace(true))
                {
                    throw new Exception("invalid object-item parser-state (space missing)");
                }
                // check for constructor
                if (textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
                {
                    ParseConstructor(itemStartPosition, objectItem);
                }
                // else check member/method/property name
                else
                {
                    string qualifierPathSecond = textParser.GetPathContent(true);
                    if (qualifierPathSecond == null)
                    {
                        throw new Exception("invalid object-item member or method name");
                    }
                    // forward possible generics
                    genericsParser.ParseGenericsDeclaration(GenericsMode.DECLARATION, GenericCategoryEnum.OBJECT);
                    // check for method
                    if (textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
                    {
                        ParseMethod(itemStartPosition, objectItem);
                    }
                    // check for member
                    else if (textParser.EqualNoneSpace(ObjectConst.StatementComplete, true) || textParser.EqualNoneSpace(ObjectConst.Assigment, true))
                    {
                        ParseMember(itemStartPosition, objectItem);
                    }
                    // check for property
                    else if (textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
                    {
                        ParseProperty(itemStartPosition, objectItem);
                    }
                    // invalid
                    else
                    {
                        throw new Exception("invalid object-item parser-state (syntax)");
                    }
                }
            }
        }

        public void ParseConstructor(int start_pos, ObjectSymbol objectType)
        {
            // rewind position
            textParser.SetPosition(start_pos);

            // get possible attribute
            AttributeType attribute = ParseAttribute();

            // get possible accessor
            AccessorType accessor = ParseAccessor();

            // get constructor type-name
            string typeName = textParser.GetPathContent(true);
            if (typeName == null)
            {
                throw new Exception("invalid constructor type-name");
            }
            // get possible generics
            GenericType generics = genericsParser.ParseGenericsDeclaration(GenericsMode.DECLARATION, GenericCategoryEnum.METHOD);
#if (TRACK)
            Utils.LogItem("constructor: '" + typeName + "' | accessor: '" + accessor + "' | attribute: '" + (attribute != null) + "'");
#endif
            // check function-start
            if (!textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
            {
                throw new Exception("invalid constructor parameter-list start");
            }

            // parse parameters
            ParameterCollection methodParameters = ParseFunctionParameterCollection();

            // create constructor-type
            ConstructorType constructorType = new ConstructorType(typeName, objectType, attribute, accessor, generics, null, methodParameters);
            objectType.Constructors.Add(constructorType);

            // check for function-code-block start
            if (!textParser.StartEndNoneSpace(ObjectConst.FunctionDeclosing, ObjectConst.BlockEnclosing, true))
            {
                throw new Exception("invalid constructor-block start");
            }

            // parse code-syntax
            codeParser.ParseCode(sourceFile, textParser.GetPosition(), constructorType.Code);

            // check block end
            if(!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
            {
                throw new Exception("invalid constructur-block end");
            }
        }

        public void ParseMember(int start_pos, ObjectSymbol objectType)
        {
            // rewind position
            textParser.SetPosition(start_pos);

            // get possible attribute
            AttributeType memberAttribute = ParseAttribute();

            // get possible accessor
            AccessorType memberAccessor = ParseAccessor();

            // check if types
            bool isStatic = false;
            bool isReadonly = false;
            if (textParser.EqualEndSpace(ObjectConst.Static, true))
            {
                isStatic = true;
            }
            if (textParser.EqualEndSpace(ObjectConst.Readonly, true))
            {
                isReadonly = true;
            }

            // get member-type
            string memberType = textParser.GetPathContent(true);
            if (memberType == null)
            {
                throw new Exception("invalid member type-name");
            }

            // get possible generics
            GenericType memberGenerics = genericsParser.ParseGenericsDeclaration(GenericsMode.DECLARATION, GenericCategoryEnum.MEMBER);

            // check for space-elements, to exec further
            if (!textParser.EqualSpace(true))
            {
                throw new Exception("invalid member parser-state (member-space missing)");
            }
            // check for member-name
            string memberName = textParser.GetPathContent(true);
            if (memberName == null)
            {
                throw new Exception("invalid member-name");
            }

            // create member-type
            MemberType member = new MemberType(memberType, memberName, memberAttribute, memberAccessor, memberGenerics);
            member.isStatic = isStatic;
            member.isReadonly = isReadonly;
            objectType.Members.Add(member);

            // check for possible member-initialisation
            if (textParser.EqualNoneSpace(ObjectConst.Assigment, true))
            {
                // parse initilatisation-expression
                ExpressionType MemberExpression = ParseMemberInitialisation();
                member.InitialisationExpression = MemberExpression;
            }

            // forward to complete-statement argument
            if (!textParser.EqualNoneSpace(ObjectConst.StatementComplete, true))
            {
                throw new Exception("invalid member-definition complete-statement");
            }
#if (TRACK)
            Utils.LogItem("member: '" + memberName + "' | type: '" + memberType + "' | accessor: '" + memberAccessor + "' | static: '"+isStatic+"' | attribute: '" + (memberAttribute != null) + "'");
#endif
        }

        public void ParseMethod(int start_pos, ObjectSymbol objectType)
        {
            // rewind position
            textParser.SetPosition(start_pos);

            // get possible attribute
            AttributeType methodAttribute = ParseAttribute();

            // get possible accessor
            AccessorType methodAccessor = ParseAccessor();

            // check call-type
            bool isStatic = false;
            bool isAbstrat = false;
            bool isOverride = false;
            if (textParser.EqualEndSpace(ObjectConst.Static, true))
            {
                isStatic = true;
            }
            else  if (textParser.EqualEndSpace(ObjectConst.Abstract, true))
            {
                isAbstrat = true;
            }
            else if (textParser.EqualEndSpace(ObjectConst.Override, true))
            {
                isOverride = true;
            }

            // check for extern-type
            bool isExtern = false;
            if(textParser.EqualEndSpace(ObjectConst.Native, true))
            {
                isExtern = true;
            }

            // get return-type
            string methodReturn = textParser.GetPathContent(true);
            if (methodReturn == null)
            {
                throw new Exception("invalid method return-type");
            }
            // check for space-elements, to exec further
            if (!textParser.EqualSpace(true))
            {
                throw new Exception("invalid method parser-state (method-space missing)");
            }
            // get method-name
            string methodName = textParser.GetPathContent(true);
            if (methodName == null)
            {
                throw new Exception("invalid method method-name");
            }
            // get possible generics
            GenericType methodGenerics = genericsParser.ParseGenericsDeclaration(GenericsMode.DECLARATION, GenericCategoryEnum.METHOD);
#if (TRACK)
            Utils.LogItem("method: '" + methodName + "' | accessor: '" + methodAccessor + "' | static: '"+isStatic+"' | abstract: '"+isAbstrat+"' | override: '"+isOverride+"' | returnType: '" + methodReturn + "' | attribute: '" + (methodAttribute!= null) +"'");
#endif
            // check parameter-list start
            if (!textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
            {
                throw new Exception("invalid method parameter-list start");
            }

            // parse method-parameters
            ParameterCollection methodParameters = ParseFunctionParameterCollection();

            // create method-type
            MethodType method = new MethodType(methodName, objectType, methodAttribute, methodAccessor, methodGenerics, methodReturn, methodParameters);
            method.IsMethod = true;
            method.IsStatic = isStatic;
            method.IsExtern = isExtern;
            method.IsVirtual = isAbstrat;
            method.IsOverride = isOverride;
            objectType.Methods.Add(method);

            // none-static interface-face methods has no code implementation
            if (objectType.IsInterface && !isStatic)
            {
                // check complete-statement, has no code-block - only declaration
                if (!textParser.StartEndNoneSpace(ObjectConst.FunctionDeclosing, ObjectConst.StatementComplete, true))
                {
                    throw new Exception("invalid interface-method complete-statement");
                }
            }
            // other object-types has a code implementation
            else
            {
                // search method-code-block start
                if (!textParser.StartEndNoneSpace(ObjectConst.FunctionDeclosing, ObjectConst.BlockEnclosing, true))
                {
                    throw new Exception("invalid method-code block-start");
                }

                // parse code-syntax
                codeParser.ParseCode(sourceFile, textParser.GetPosition(), method.Code);

                // search code-block end
                if (!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                {
                    throw new Exception("invalid method-code block-end");
                }
            }
        }

        public void ParseProperty(int start_pos, ObjectSymbol objectType)
        {
            // rewind position
            textParser.SetPosition(start_pos);

            // get possible attribute
            AttributeType propertyAttributes = ParseAttribute();

            // get possible accessor
            AccessorType propertyAccessor = ParseAccessor();

            // check call-type
            bool isStatic = false;
            bool isAbstrat = false;
            bool isOverride = false;
            if (textParser.EqualEndSpace(ObjectConst.Static, true))
            {
                isStatic = true;
            }
            else if (textParser.EqualEndSpace(ObjectConst.Abstract, true))
            {
                isAbstrat = true;
            }
            else if (textParser.EqualEndSpace(ObjectConst.Override, true))
            {
                isOverride = true;
            }

            // get return-type
            string propertyReturn = textParser.GetPathContent(true);
            if (propertyReturn == null)
            {
                throw new Exception("invalid property return-type");
            }
            // check for space-elements, to exec further
            if (!textParser.EqualSpace(true))
            {
                throw new Exception("invalid property parser-state (space missing)");
            }
            // get property-name
            string propertyName = textParser.GetPathContent(true);
            if (propertyName == null)
            {
                throw new Exception("invalid property name");
            }
            // get possible generics
            GenericType propertyGenerics = genericsParser.ParseGenericsDeclaration(GenericsMode.DECLARATION, GenericCategoryEnum.PROPERTY);
#if (TRACK)
            Utils.LogItem("property: '" + propertyName + "' | accessor: '" + propertyAccessor + "' | static: '" + isStatic + "' | abstract: '" + isAbstrat + "' | override: '" + isOverride + "' | returnType: '" + propertyReturn + "' | attribute: '" + (propertyAttributes != null) + "'");
#endif
            // check property block-start
            if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
            {
                throw new Exception("invalid property block-start");
            }

            // create property-type
            PropertySymbol property = new PropertySymbol(propertyName, objectType, propertyAttributes, propertyAccessor, propertyGenerics, propertyReturn);
            property.IsStatic = isStatic;
            property.IsVirtual = isAbstrat;
            property.IsOverride = isOverride;
            objectType.Properties.Add(property);

            // check get/set property code blocks
            while(true)
            {
                textParser.SkipSpace(true);

                // check for possible get
                if (textParser.StartEndNoneSpace(ObjectConst.Get, ObjectConst.BlockEnclosing, true))
                {
                    // check duplicate get-block
                    if(property.GetCode != null)
                    {
                        throw new Exception("only one get-context for property can exist");
                    }
                    
                    // parse code-block
                    CodeType codeType = new CodeType(property);
                    codeParser.ParseCode(sourceFile, textParser.GetPosition(), codeType);
                    property.GetCode = codeType;
                    
                    // check block end
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                    {
                        throw new Exception("invalid property get block end");
                    }
                }
                // check for possible set
                else if (textParser.StartEndNoneSpace(ObjectConst.Set, ObjectConst.BlockEnclosing, true))
                {
                    // check duplicate set-block
                    if (property.SetCode != null)
                    {
                        throw new Exception("only one set-context for property can exist");
                    }

                    // parse code-block
                    CodeType codeType = new CodeType(property);
                    codeParser.ParseCode(sourceFile, textParser.GetPosition(), codeType);
                    property.SetCode = codeType;

                    // check block end
                    if (!textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                    {
                        throw new Exception("invalid property set block end");
                    }
                }
                // check empty block end
                else if (textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                {
                    break;
                }
                // invalid
                else
                {
                    throw new Exception("invalid property content or end");
                }
            }
        }

        public AttributeType ParseAttribute()
        {
            // check for attribute start
            if (!textParser.EqualNoneSpace(ObjectConst.BracketEnclosing, true))
            {
                return null;
            }

            AttributeType attributeType = new AttributeType();
#if (TRACK)
            Utils.LogItem("attribute_start");
#endif
            // parse attribute items
            while(true)
            {
                textParser.SkipSpace(true);

                // check attribute block end
                if (textParser.EqualNoneSpace(ObjectConst.BracketDeclosing, true))
                {
                    break;
                }

                AttributeItem attributeItem = new AttributeItem();
                attributeType.ElementCollection.Add(attributeItem);

                // get type-name
                attributeItem.TypeName = textParser.GetPathContent(true);
                if(attributeItem.TypeName == null)
                {
                    throw new Exception("invalid attribute-item type-name");
                }
#if (TRACK)
                Utils.LogItem("attribute_item_start: type-name: '" + attributeItem.TypeName);
#endif
                // parse declaration
                AttributeItemDeclaratioType declaration = new AttributeItemDeclaratioType();
                ParseAttributeOrEnumItem(declaration);

                // add
                attributeItem.DeclarationType = declaration;

                // check for next parameter
                if (textParser.EqualNoneSpace(ObjectConst.ParameterSeperator, true))
                {
                    continue;
                }
                // check attribute block end
                else if (textParser.EqualNoneSpace(ObjectConst.BracketDeclosing, true))
                {
                    break;
                }
                // invalid
                else
                {
                    throw new Exception("invalid attribute-item block-end (syntax)");
                }
            }
            textParser.SkipSpace(true);
#if (TRACK)
            Utils.LogItem("attribute_end");
#endif
            // return result
            return attributeType;
        }


        public void ParseEnumItems(EnumType enumItem, int startPosition, int depth)
        {
            // check end of declaration
            if (textParser.EqualNoneSpace(ObjectConst.StatementComplete, true))
            {
                return;
            }

            EnumItemType enumElementType = new EnumItemType();

            // get enum-item-name
            enumElementType.EnumItemName = textParser.GetNameContent(true);
            if (enumElementType.EnumItemName == null)
            {
                throw new Exception("invalid enum-item-name");
            }

            // parse items
            EnumItemDeclaratioType declaration = new EnumItemDeclaratioType();
            ParseAttributeOrEnumItem(declaration);
            enumElementType.DeclarationType = declaration;

            // add
            enumItem.ItemCollection.Add(enumElementType);

            // check end of declaration
            if (!textParser.EqualNoneSpace(ObjectConst.StatementComplete, true))
            {
                throw new Exception("invalid declaration end");
            }
        }

        public void ParseAttributeOrEnumItem(NativeDeclarationType declarationType)
        {
            // check for initializer
            if (!textParser.EqualNoneSpace(ObjectConst.FunctionEnclosing, true))
            {
                throw new Exception("no declaration enclosing");
            }
            // get positional parameters
            while (true)
            {
                textParser.SkipSpace(true);

                // get possible literal-operand
                OperandType literalOperad = expressionParser.ParseLiteralOperand(sourceFile);
                // or break
                if (literalOperad == null)
                {
                    break;
                }
                // get native-type
                NativeSymbol nativeType = literalOperad.GetNativeType();
                if (nativeType == null)
                {
                    throw new Exception("invalid attribute-item native-type");
                }
#if (TRACK)
                Utils.LogItem("positional-parameter | " + literalOperad.GetDebugText());
#endif
                declarationType.ConstructorDeclarationList.Add(literalOperad);
                // check for next parameter or break
                if (!textParser.EqualNoneSpace(ObjectConst.ParameterSeperator, true))
                {
                    break;
                }
            }

            // get named parameters
            while (true)
            {
                textParser.SkipSpace(true);

                // get possible name-identifier
                string itemIdentifier = textParser.GetNameContent(true);
                // or break
                if (itemIdentifier == null)
                {
                    break;
                }
                // check assigment operatiion
                if (!textParser.EqualNoneSpace(ObjectConst.Assigment, true))
                {
                    throw new Exception("invalid attribute-item state, missing assigment operation");
                }
                // get literal-operand
                OperandType literalOperand = expressionParser.ParseLiteralOperand(sourceFile);
                if (literalOperand == null)
                {
                    throw new Exception("invalid attribute-item named-literal-operand");
                }
#if (TRACK)
                Utils.LogItem("named-parameter | identifier: '" + itemIdentifier + "' | " + literalOperand.GetDebugText());
#endif
                // get native-type from literal-operand
                NativeSymbol nativeType = literalOperand.GetNativeType();
                if (nativeType == null)
                {
                    throw new Exception("invalid attribute-item native-type");
                }
                declarationType.MemberDeclarationMap.Add(itemIdentifier, literalOperand);
                // check for next parameter or break
                if (!textParser.EqualNoneSpace(ObjectConst.ParameterSeperator, true))
                {
                    break;
                }
            }

            // check for function-end
            if (!textParser.EqualNoneSpace(ObjectConst.FunctionDeclosing, true))
            {
                throw new Exception("invalid attribute-item-initializer block-end");
            }
        }

        public AccessorType ParseAccessor()
        {
            if (textParser.EqualEndSpace(AccessorConst.PublicStr, true))
            {
                return AccessorConst.PublicType;
            }
            else if (textParser.EqualEndSpace(AccessorConst.PrivateStr, true))
            {
                return AccessorConst.PrivateType;
            }
            else
            {
                return AccessorConst.NoneType;
            }
        }

        public ImplementCollection ParseImplementCollection()
        {
            if(textParser.EqualEndSpace(ObjectConst.Implement, true))
            {
                ImplementCollection implementCollection = new ImplementCollection();
                string implementTypeName;
                while (true)
                {
                    textParser.SkipSpace(true);

                    // get implemenet name
                    implementTypeName = textParser.GetPathContent(true);
                    if (implementTypeName == null)
                    {
                        throw new Exception("no implement type-name");
                    }

                    // parse possible generics
                    GenericType genericsType = genericsParser.ParseGenericsDeclaration(GenericsMode.DECLARATION, GenericCategoryEnum.IMPLEMENT);

                    ImplementSymbol implement = new ImplementSymbol(implementTypeName, genericsType);
                    implementCollection.Add(implement);

                    // check for next
                    if (!textParser.EqualNoneSpace(ObjectConst.ParameterSeperator, true))
                    {
                        break;
                    }
                }
                return implementCollection;
            }
            // none
            return null;
        }

        public ParameterCollection ParseFunctionParameterCollection()
        {
            ParameterCollection parameters = new ParameterCollection();
            while (true)
            {
                textParser.SkipSpace(true);

                // check for function-end
                if (textParser.Equal(ObjectConst.FunctionDeclosing, false))
                {
                    return parameters;
                }
                // get parameter-type
                string parameterType = textParser.GetPathContent(true);
                if (parameterType == null)
                {
                    throw new Exception("invalid parameter-type");
                }
                // check space
                if (!textParser.EqualSpace(true))
                {
                    throw new Exception("invalid parameter-expression (missing space)");
                }
                // get parameter-name
                string parameterName = textParser.GetPathContent(true);
                if (parameterName == null)
                {
                    throw new Exception("invalid parameter-name");
                }
#if (TRACK)
                Utils.LogItem("parameter: '" + parameterType + "' '" + parameterName + "'");
#endif
                ParameterType parameter = new ParameterType(parameterType, parameterName);
                parameters.Add(parameter);

                // check for next parameter or break
                if (!textParser.EqualNoneSpace(ObjectConst.ParameterSeperator, true))
                {
                    return parameters;
                }
            }
        }

        public ExpressionType ParseMemberInitialisation()
        {
#if (TRACK)
            Utils.LogItem("member_initialisation_start");
#endif
            // parse expression
            ExpressionType expression = expressionParser.ParseExpression(sourceFile);
#if (TRACK)
            Utils.LogItem("member_initialisation_end");
#endif
            return expression;
        }
    }
}
