using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Be.Runtime.Types;

namespace Be.Runtime.Parse
{
    public class ArrayParser
    {
        private TextParser textParser;
        private ExpressionParser expressionParser;

        public ArrayParser(TextParser textParser, ExpressionParser expressionParser)
        {
            this.textParser = textParser;
            this.expressionParser = expressionParser;
        }

        public ArrayParameterCollection ParseArrayAccessParameterCollection(SourceFile sourceType)
        {
            // check bracket start
            if (!textParser.EqualNoneSpace(ObjectConst.BracketEnclosing, true))
            {
                return null;
            }
#if (TRACK)
            Utils.LogItem("array_parameter_access_start");
#endif
            ArrayParameterCollection arrayParameters = new ArrayParameterCollection();

            // parse parameters
            while (true)
            {
#if (TRACK)
                Utils.LogItem("array_item_expression");
#endif
                // get expression parameter
                ExpressionType expressionParameter = expressionParser.ParseExpression(sourceType);
                ArrayParameter arrayParameter = new ArrayParameter(expressionParameter);
                arrayParameters.Add(arrayParameter);

                // check for next parameter
                if (textParser.EqualNoneSpace(ObjectConst.ParameterSeperator, true))
                {
                    continue;
                }
                // check for bracket-end
                else if (textParser.EqualNoneSpace(ObjectConst.BracketDeclosing, true))
                {
                    break;
                }
                else
                {
                    throw new Exception("invalid syntax in array access parameter");
                }
            }
#if (TRACK)
            Utils.LogItem("array_parameter_access_end");
#endif
            // return
            return arrayParameters;
        }

        public ArrayType ParseArrayDeclaration()
        {
            // check bracket start
            if (!textParser.EqualNoneSpace(ObjectConst.BracketEnclosing, true))
            {
                return null;
            }

            ArrayType arrayType = new ArrayType();

            // parse dimension declaration-head
            while (true)
            {
                arrayType.DimensionCount++;
                // check for next-dimenension
                if (textParser.EqualNoneSpace(ObjectConst.ParameterSeperator, true))
                {
                    continue;
                }
                // check for bracket-end
                else if (textParser.EqualNoneSpace(ObjectConst.BracketDeclosing, true))
                {
                    break;
                }
                else
                {
                    throw new Exception("invalid syntax in array declaration-head");
                }
            }
            // return array-declaration
            return arrayType;
        }

        public ArrayType ParseArrayInitiation(SourceFile sourceType)
        {
            // check bracket start
            if (!textParser.EqualNoneSpace(ObjectConst.BracketEnclosing, true))
            {
                return null;
            }

            ArrayType arrayType = new ArrayType();

            // parse dimension declaration-head
            while (true)
            {
                arrayType.DimensionCount++;
                // check for possible positiv integral dimension-number
                string numberStringContent = textParser.ParseNumberContent(true).Result;
                ulong dimensionSize = 0;
                if (numberStringContent != null)
                {
                    try
                    {
                        dimensionSize = ulong.Parse(numberStringContent);
                        if(dimensionSize <= 0)
                        {
                            throw new Exception("dimension-number must be an positive integral");
                        }
                    }
                    catch(Exception e)
                    {
                        throw new Exception("dimension-number must be an positive integral");
                    }
                }
                // add dimension-count to list (0-means = not specified)
                arrayType.DimensionDepthList.Add(dimensionSize);
                // check for next-dimenension
                if (textParser.EqualNoneSpace(ObjectConst.ParameterSeperator, true))
                {
                    continue;
                }
                // check for end
                else if (textParser.EqualNoneSpace(ObjectConst.BracketDeclosing, true))
                {
                    break;
                }
                else
                {
                    throw new Exception("invalid syntax in array declaration-head");
                }
            }

#if (TRACK)
            Utils.LogItem("array-declaration | dimension-count: " + arrayType.DimensionCount + " | dimensions_list: '" + arrayType.DimensionDepthList.ToString() + "'");
#endif
            // check for initiation-block
            if (!textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, false))
            {
                return arrayType;
            }

            // parse initialisation-block
            int depth = 0;
            ArrayNodeType currentNode = arrayType.InitialisationRootNode = new ArrayNodeType(null);
            while (true)
            {
                // go in depth
                if (textParser.EqualNoneSpace(ObjectConst.BlockEnclosing, true))
                {
                    ArrayNodeType newNode = new ArrayNodeType(currentNode);
                    currentNode.ChildNodes.Add(newNode);
                    currentNode = newNode;
                    depth++;
                    continue;
                }
                // go out depth
                else if (textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, true))
                {
                    currentNode = currentNode.ParentNode;
                    depth--;
                    // break if back to root
                    if (depth == 0)
                    {
                        break;
                    }
                    // next on parent
                    else
                    {
                        continue;
                    }
                }
                // check for next block
                else if (textParser.EqualNoneSpace(ObjectConst.ParameterSeperator, true))
                {
                    continue;
                }
#if (TRACK)
                Utils.LogItem("array-item | depth: " + depth + " | position: " + currentNode.InitialisationExpressionList.Size());
#endif
                // parse expression
                ExpressionType expressionType = expressionParser.ParseExpression(sourceType);
                currentNode.InitialisationExpressionList.Add(expressionType);

                // check for next expression
                if (textParser.EqualNoneSpace(ObjectConst.ParameterSeperator, true))
                {
                    continue;
                }
                // check for block end
                else if (textParser.EqualNoneSpace(ObjectConst.BlockDeclosing, false))
                {
                    continue;
                }
                // invalid
                else
                {
                    throw new Exception("invalid syntax in array initiation");
                }
            }

            // return array-initiation
            return arrayType;
        }
    }
}
