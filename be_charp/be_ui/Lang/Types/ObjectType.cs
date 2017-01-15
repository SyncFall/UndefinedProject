using System;
using System.Text;

namespace Be.Runtime.Types
{
    public enum ObjectCategory
    {
        OBJECT,
        INTERFACE,
        ENUM,
        ATTR,
        EXCEPTION,
        NONE,
    }

    public class ObjectConst
    {
        public static readonly string Namespace = "namespace";
        public static readonly string Using = "using";
        public static readonly string Object = "object";
        public static readonly string Interface = "interface";
        public static readonly string Enum = "enum";
        public static readonly string Attr = "attr";
        public static readonly string Exception = "exception";
        public static readonly string Implement = "implement";
        public static readonly string Extends = "extends";
        public static readonly string Abstract = "abstract";
        public static readonly string Override = "override";
        public static readonly string Static = "static";
        public static readonly string Readonly = "readonly";
        public static readonly string Native = "native";
        public static readonly string Const = "const";
        public static readonly string BlockEnclosing = "{";
        public static readonly string BlockDeclosing = "}";
        public static readonly string FunctionEnclosing = "(";
        public static readonly string FunctionDeclosing = ")";
        public static readonly string BracketEnclosing = "[";
        public static readonly string BracketDeclosing = "]";
        public static readonly string RelationalEnclosing = "<";
        public static readonly string RelationalDeclosing = ">";
        public static readonly string StatementComplete = ";";
        public static readonly string ParameterSeperator = ",";
        public static readonly string NewType = "new";
        public static readonly string Assigment = "=";
        public static readonly string Colon = ":";
        public static readonly string Get = "get";
        public static readonly string Set = "set";
    }

    public class ObjectCollection : ListCollection<ObjectSymbol>
    { }

    public class ObjectSymbol
    {
        public string AbsolutePath;
        public string ObjectPath;
        public NamespaceSymbol Namespace;

        public string String;
        public AttributeType Attributes;
        public AccessorType Accessor;
        
        public bool IsAbstract;
        public bool IsInterface;
        public bool IsEnum;
        public bool IsAttribute;
        public bool IsException;
        public bool IsNative;
        public bool IsAnonymous;
        public GenericType GenericType;
        public ExtendSymbol ExtendType;
        public ImplementCollection ImplementType;

        public ObjectCollection Objects = new ObjectCollection();
        public MethodCollection Constructors = new MethodCollection();
        public MethodCollection Methods = new MethodCollection();
        public MethodCollection Properties = new MethodCollection();
        public MemberCollection Members = new MemberCollection();
      
        public SourceFile SourceFile;
        public ObjectSymbol ParentObject;

        public ObjectSymbol(string Name, AttributeType Attribute, AccessorType Accessor, GenericType Generics, ExtendSymbol Extend, ImplementCollection Implement, bool IsVirtual)
        {
            this.String = Name;
            this.Attributes = Attribute;
            this.Accessor = Accessor;
            this.GenericType = Generics;
            if(this.GenericType != null)
            {
                this.GenericType.CreateSignatur();
            }
            this.ExtendType = Extend;
            this.ImplementType = Implement;
            this.IsAbstract = IsVirtual;
        }

        public bool IsCompilantWith(ObjectSymbol compare)
        {
            // check for object and native missmatch
            if (this.IsNative != compare.IsNative)
            {
                return false;
            }
            // void-null-type on strings and objects are right
            else if (
               ((!this.IsNative || (this.IsNative && (this as NativeSymbol).Type == NativeType.String)) &&
               (compare.IsNative && (compare as NativeSymbol).Type == NativeType.Void))
            ){
                return true;
            }
            // check for two objects if from same type
            else if (!this.IsNative && this.String.Equals(compare.String))
            {
                return true;
            }
            // check for two natives 
            else if (this.IsNative)
            {
                // if from same category
                if (((this as NativeSymbol).Group != (compare as NativeSymbol).Group))
                {
                    return false;
                }
                // and possible compilant number-group
                else if((this as NativeSymbol).Group == NativeGroup.Number && (this as NativeSymbol).NumberGroup != (compare as NativeSymbol).NumberGroup)
                {
                    return false;
                }
                // pass
                else
                {
                    return true;
                }
            }
            // invalid
            throw new Exception("invalid state");
        }
    }

    public class ObjectInitialisationType
    {
        public ExpressionCollection ConstructorDefinitionList = new ExpressionCollection();
        public MapCollection<string, ExpressionType> MemberDefinitionMap = new MapCollection<string, ExpressionType>();
        public AnonObjectType AnonymousObjectType;
    }
}
