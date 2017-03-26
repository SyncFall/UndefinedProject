using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public class Symbol
    {
        public readonly int Group;
        public readonly int Type;
        public readonly int Category;
        public readonly string String;

        public Symbol(string String, int Group, int Type=0, int Category=0)
        {
            this.String = String;
            this.Group = Group;
            this.Type = Type;
            this.Category = Category;
        }

        public bool IsLineSpace()
        {
            return (Group == (int)TokenType.Structure && Type == (int)StructureType.LineSpace);
        }

        public bool IsSpace()
        {
            return (Group == (int)TokenType.Structure &&
                   (Type == (int)StructureType.WhiteSpace || Type == (int)StructureType.TabSpace || Type == (int)StructureType.LineSpace));
        }

        public bool IsTextContent()
        {
            return (Group == (int)TokenType.Comment ||
                   (Group == (int)TokenType.Literal && (Type == (int)LiteralType.String || Type == (int)LiteralType.Char)));
        }

        public bool IsType(TokenType TokenType)
        {
            return (Group == (int)TokenType);
        }

        public bool IsLiteral(LiteralType LiteralType)
        {
            return (Group == (int)TokenType.Literal && Type == (int)LiteralType);
        }

        public bool IsStructure(StructureType StructureType)
        {
            return (Group == (int)TokenType.Structure && Type == (int)StructureType);
        }

        public bool IsCategory(OperationCategory OperationCategory)
        {
            return (Group == (int)TokenType.Operation && Category == (int)OperationCategory);
        }

        public bool IsOperation(OperationType OperationType)
        {
            return (Group == (int)TokenType.Operation && Type == (int)OperationType);
        }

        public bool IsObject(ObjectType ObjectType)
        {
            return (Group == (int)TokenType.Object && Type == (int)ObjectType);
        }

        public bool IsNative(NativeType NativeType)
        {
            return (Group == (int)TokenType.Native && Type == (int)NativeType);
        }

        public bool IsVisual(VisualType VisualType)
        {
            return (Group == (int)TokenType.Visual && Type == (int)VisualType);
        }
    }
}
