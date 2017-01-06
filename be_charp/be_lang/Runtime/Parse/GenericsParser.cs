using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Runtime.Types;

namespace Be.Runtime.Parse
{
    public class GenericsParser
    {
        private TextParser textParser;

        public GenericsParser(TextParser textParser)
        {
            this.textParser = textParser;
        }

        public GenericType ParseGenericsDeclaration(GenericsMode genericsMode, GenericCategoryEnum genericsCategory)
        {
            // check for possible generics start
            if (!textParser.EqualNoneSpace(ObjectConst.RelationalEnclosing, true))
            {
                return null;
            }
#if(TRACK)
            Utils.LogItem("generic-type start");
#endif
            GenericType genericType = new GenericType(genericsCategory);

            // parse generic types
            while (true)
            {
                textParser.SkipSpace(true);

                // check for block-end
                if (textParser.EqualNoneSpace(ObjectConst.RelationalDeclosing, true))
                {
                    break;
                }

                // get generic-type
                string genericTypeName = textParser.GetNameContent(true);
                if (genericTypeName == null)
                {
                    throw new Exception("invalid generic-type-name");
                }
#if (TRACK)
                Utils.LogItem("generic-item | type-name: '" + genericTypeName + "'");
#endif
                // add to list
                GenericElementType genericElementType = new GenericElementType(null, genericTypeName, null);
                genericType.ElementCollection.Add(genericElementType);

                // check for extend-type if in declaration mode
                if (genericsMode == GenericsMode.DECLARATION && textParser.EqualNoneSpace(ObjectConst.Extends, true))
                {
                    string extendTypeName = textParser.GetNameContent(true);
                    if (extendTypeName == null)
                    {
                        throw new Exception("missing generics-type extend type-name");
                    }
                    genericElementType.ExtendTypeName = extendTypeName; // set variable
                }
                // check for child generics-types recusrive
                else if(textParser.EqualNoneSpace(ObjectConst.RelationalEnclosing, false))
                {
                    genericElementType.GenericType = ParseGenericsDeclaration(genericsMode, genericsCategory);
                }

                // check next
                if (textParser.EqualNoneSpace(ObjectConst.ParameterSeperator, true))
                {
                    continue;
                }
                // check block-end
                else if (textParser.EqualNoneSpace(ObjectConst.RelationalDeclosing, true))
                {
                    break;
                }
                // invalid
                else
                {
                    throw new Exception("invalid generic-type syntax");
                }
            }
#if (TRACK)
            Utils.LogItem("generic-type end");
#endif
            // return 
            return genericType;
        }
    }
}
