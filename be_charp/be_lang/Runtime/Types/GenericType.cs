using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Runtime.Types
{
    public enum GenericsMode
    {
        DECLARATION,
        DEFINITION,
    }

    public enum GenericCategoryEnum
    {
        OBJECT,
        EXTEND,
        IMPLEMENT,
        MEMBER,
        METHOD,
        VARIABLE,
        PROPERTY,
        UNKNONW,
    }

    public class GenericType
    {
        public GenericCategoryEnum Category;
        public GenericElementCollection ElementCollection = new GenericElementCollection();
        public GenericSignaturType GenericSignaturType;

        public GenericType(GenericCategoryEnum Category)
        {
            this.Category = Category;
        }

        public bool FindTypeName(string genericTypeName)
        {
            GenericIndexType indexType = FindTypeIndex(genericTypeName);
            return (indexType.Depth != -1 && indexType.Position != -1);
        }

        public GenericIndexType FindTypeIndex(string genericTypeName)
        {
            return FindFirstIndexType(genericTypeName, 0);
        }

        private GenericIndexType FindFirstIndexType(string genericTypeName, int depth)
        {
            GenericIndexType indexType = new GenericIndexType();
            indexType.Depth = depth;
            for (int i = 0; i < this.ElementCollection.Size(); i++)
            {
                GenericElementType genericElement = this.ElementCollection.Get(i);
                if (genericElement.MatchTypeName(genericTypeName))
                {
                    indexType.Position = i;
                    break;
                }
                else if(genericElement.GenericType != null)
                {
                    indexType = genericElement.GenericType.FindFirstIndexType(genericTypeName, depth + 1);
                }
            }
            return indexType;
        }

        public void CreateSignatur()
        {
            this.GenericSignaturType = new GenericSignaturType(this, this.Category);
        }

        public bool EqualSignatur(GenericType genericType)
        {
            if(genericType == null)
            {
                return false;
            }
            else if(this.GenericSignaturType == null && genericType.GenericSignaturType == null)
            {
                return true;
            }
            return this.GenericSignaturType.EqualSignatur(genericType.GenericSignaturType);
        }
    }

    public class GenericElementCollection : ListCollection<GenericElementType>
    { }

    public class GenericElementType
    {
        public GenericType GenericType;

        public string TypeName;
        public string ExtendTypeName;
        public ObjectSymbol ObjectType;
        public ObjectSymbol ExtendObjectType;

        public GenericElementType(GenericType genericType, string TypeName, string ExtendTypeName)
        {
            this.TypeName = TypeName;
            this.ExtendTypeName = ExtendTypeName;
            this.GenericType = genericType;
        }

        public bool MatchTypeName(string genericTypeName)
        {
            if (genericTypeName == null)
            {
                return false;
            }
            else if (this.TypeName.Equals(genericTypeName))
            {
                return true;
            }
            return false;
        }
    }

    public class GenericIndexType
    {
        public int Depth = -1;
        public int Position = -1;

        public bool EqualIndex(GenericIndexType compare)
        {
            return (this.Depth == compare.Depth && this.Position == compare.Position);
        }
    }

    public class GenericSignaturType
    {
        public GenericCategoryEnum GenericCategory;
        public ListCollection<string> PlaceholderNames = new ListCollection<string>();
        public GenericSignaturElementType PlaceholderIndexPositionHierachie = new GenericSignaturElementType();

        public GenericSignaturType(GenericType genericType, GenericCategoryEnum genericCategory)
        {
            this.GenericCategory = genericCategory;
            this.CreateSignatur(genericType, this.PlaceholderIndexPositionHierachie);
        }

        public bool EqualSignatur(GenericSignaturType compare)
        {
            if(compare == null)
            {
                return false;
            }
            return PlaceholderIndexPositionHierachie.EqualSignatur(compare.PlaceholderIndexPositionHierachie);
        }

        private void CreateSignatur(GenericType genericType, GenericSignaturElementType parent)
        {
            if (genericType == null)
            {
                return;
            }
            // foreach generic-element on same hierachie
            for (int i = 0; i < genericType.ElementCollection.Size(); i++)
            {
                GenericElementType genericElement = genericType.ElementCollection.Get(i);

                // create node
                GenericSignaturElementType signaturElement = new GenericSignaturElementType();
                parent.Childs.Add(signaturElement);

                // get possible extend-object type
                signaturElement.ExtendObjectType = genericElement.ExtendObjectType;

                // get possible object-type
                signaturElement.ObjectType = genericElement.ObjectType;

                // else for named-placeholder definition
                if (genericElement.ObjectType == null)
                {
                    // get placeholder-name position
                    int placeholderNameIndexPosition = -1;
                    for (int j = 0; j < PlaceholderNames.Size(); j++)
                    {
                        if (this.PlaceholderNames.Get(j).Equals(genericElement.TypeName))
                        {
                            placeholderNameIndexPosition = j;
                            break;
                        }
                    }
                    // add new if name not exist in list
                    if (placeholderNameIndexPosition == -1)
                    {
                        PlaceholderNames.Add(genericElement.TypeName);
                        placeholderNameIndexPosition = (PlaceholderNames.Size() - 1);
                    }
                    // set signatur-named position
                    signaturElement.PlaceholderNameIndexPosition = placeholderNameIndexPosition;
                }

                // check possible child-elements
                CreateSignatur(genericElement.GenericType, signaturElement);
            }

        }
    }

    public class GenericSignaturElementCollection : ListCollection<GenericSignaturElementType>
    { }

    public class GenericSignaturElementType
    {
        public int PlaceholderNameIndexPosition = -1;
        public ObjectSymbol ObjectType;
        public ObjectSymbol ExtendObjectType;
        public GenericSignaturElementCollection Childs = new GenericSignaturElementCollection();

        public bool EqualSignatur(GenericSignaturElementType compare)
        {
            if(this.PlaceholderNameIndexPosition != compare.PlaceholderNameIndexPosition)
            {
                return false;
            }
            else if(!UtilType.EqualNullObject(this.ObjectType, compare.ObjectType))
            {
                return false;
            }
            else if (this.ObjectType != null && !this.ObjectType.Name.Equals(compare.ObjectType))
            {
                return false;
            }
            else if (!UtilType.EqualNullObject(this.ExtendObjectType, compare.ExtendObjectType))
            {
                return false;
            }
            else if (this.ExtendObjectType != null && !this.ExtendObjectType.Name.Equals(compare.ExtendObjectType))
            {
                return false;
            }
            else if (this.Childs.Size() != compare.Childs.Size())
            {
                return false;
            }
            for(int i=0; i<this.Childs.Size(); i++)
            {
                if (!this.Childs.Get(i).EqualSignatur(compare.Childs.Get(i)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
