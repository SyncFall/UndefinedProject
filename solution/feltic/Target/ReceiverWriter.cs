using feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public class StateTarget
    {
        public static int IdCounter=0;
        public ObjectSymbol StateObject;
        public MethodSymbol StateMethod;
        public string IdentifierString;
        public StringBuilder Builder = new StringBuilder();

        public StateTarget(ObjectSymbol StateObject, MethodSymbol StateMethod)
        {
            this.StateObject = StateObject;
            this.StateMethod = StateMethod;
            this.BuildIdentifier();
        }

        public void BuildIdentifier()
        {
            IdCounter++;
            string str = IdCounter + "";
            if (StateObject != null)
                str += "_" + StateObject.Signature.Identifier.String;
            if (StateMethod != null)
                str += "_" + StateMethod.Signature.TypeDeclaration.NameIdentifier.String;
            this.IdentifierString = str;
        }

    }

    public partial class TargetWriter
    {
        public ListCollection<StateTarget> StateReceivers = new ListCollection<StateTarget>();

        public StateTarget AddStateTarget(ObjectSymbol StateObject, MethodSymbol StateMethod)
        {
            StateTarget target = new StateTarget(StateObject, StateMethod);
            WriteReceiverContainer(target);
            WriteStateReceiver(target);
            StateReceivers.Add(target);
            return target;
        }

        public StateTarget GetStateTarget(ObjectSymbol StateObject, string StateMethodIdentifier)
        {
            for(int i=0; i<StateReceivers.Size; i++)
            {
                if(StateReceivers[i].StateObject.Signature.Identifier.String == StateObject.Signature.Identifier.String)
                {
                    if(StateReceivers[i].StateMethod.Signature.TypeDeclaration.NameIdentifier.String == StateMethodIdentifier)
                    {
                        return StateReceivers[i];
                    }
                }
            }
            return null;
        }

        public void WriteReceiverContainer(StateTarget target)
        {
            target.Builder = PushBuilder();

            WriteLine(1, "public class ReceiverContainer_" + target.IdentifierString);
            WriteLine(1, "{");
            WriteLine(2, "public MapCollection<int, StateReceiver_" + target.IdentifierString+ "> Receivers = new MapCollection<int, StateReceiver_" + target.IdentifierString+">();");
            WriteLine();
            WriteTab(2);
            Write("public void EventState(");
            ParameterDeclarationSignature declaration = target.StateMethod.Signature.ParameterDeclaration;
            for (int i=0; i< declaration.Elements.Size; i++)
            {
                ParameterSignature parameter = declaration.Elements[i];
                Write(parameter.TypeDeclaration.TypeIdentifier.String + " " + parameter.TypeDeclaration.NameIdentifier.String);
                if (i < declaration.Elements.Size - 1)
                    Write(", ");
            }
            WriteLine(")");
            WriteLine(2, "{");
            WriteLine(3, "int[] keys = Receivers.Keys;");
            WriteLine(3, "for(int i=0; i<keys.Length; i++){");
            WriteTab(4);
            Write("Receivers[keys[i]].EventState(");
            for (int i = 0; i < declaration.Elements.Size; i++)
            {
                ParameterSignature parameter = declaration.Elements[i];
                Write(parameter.TypeDeclaration.NameIdentifier.String);
                if (i < declaration.Elements.Size - 1)
                    Write(", ");
            }
            WriteLine(");");
            WriteLine(3, "}");
            WriteLine(2, "}");
            WriteLine(1, "}");

            PopBuilder();
        }

        public void WriteStateReceiver(StateTarget target)
        {
            StringBuilder currentBuilder = Builder;
            Builder = target.Builder;

            WriteLine(1, "public abstract class StateReceiver_" + target.IdentifierString);
            WriteLine(1, "{");
            WriteLine(2, "private static int IdCounter = 0;");
            WriteLine(2, "public readonly int Id;");
            WriteLine(2, "public ReceiverContainer_" + target.IdentifierString + " Container;");
            WriteLine();
            WriteLine(2, "public StateReceiver_" + target.IdentifierString + "(ReceiverContainer_" + target.IdentifierString + " Container){");
            WriteLine(3, "this.Container = Container;");
            WriteLine(3, "this.Id = (++IdCounter);");
            WriteLine(3, "this.Container.Receivers.Put(this.Id, this);");
            WriteLine(2, "}");
            WriteTab(2);
            Write("public abstract void EventState(");
            ParameterDeclarationSignature declaration = target.StateMethod.Signature.ParameterDeclaration;
            for (int i = 0; i < declaration.Elements.Size; i++)
            {
                ParameterSignature parameter = declaration.Elements[i];
                Write(parameter.TypeDeclaration.TypeIdentifier.String + " " + parameter.TypeDeclaration.NameIdentifier.String);
                if (i < declaration.Elements.Size - 1)
                    Write(", ");
            }
            WriteLine(");");
            WriteLine(1, "}");

            Builder = currentBuilder;
        }

        public void WriteStateImplementation(StateTarget Target, ObjectSymbol Object, MethodSymbol Method, SignatureList SigList)
        {
            StringBuilder currentBuilder = Builder;
            Builder = Target.Builder;

            WriteLine(1, "public class TargetState_" + Target.IdentifierString + " : StateReceiver_" + Target.IdentifierString);
            WriteLine(1, "{");
            WriteLine(2, "public " + Object.Signature.Identifier.String + " Object;");
            WriteLine();
            WriteLine(2, "public TargetState_" + Target.IdentifierString + "(ReceiverContainer_" + Target.IdentifierString + " Container, "+Object.Signature.Identifier.String+ " Object) : base(Container){");
            WriteLine(3, "this.Container = Container;");
            WriteLine(3, "this.Object = Object;");
            WriteLine(2, "}");
            WriteTab(2);
            Write("public override void EventState(");
            ParameterDeclarationSignature declaration = Target.StateMethod.Signature.ParameterDeclaration;
            for (int i = 0; i < declaration.Elements.Size; i++)
            {
                ParameterSignature parameter = declaration.Elements[i];
                Write(parameter.TypeDeclaration.TypeIdentifier.String + " " + parameter.TypeDeclaration.NameIdentifier.String);
                if (i < declaration.Elements.Size - 1)
                    Write(", ");
            }
            WriteLine("){");
            WriteStatements(3, Object, Method, new VisualComponent(Object, Method, null), SigList);
            WriteLine(2, "}");
            WriteLine(1, "}");

            Builder = currentBuilder;
        }

    }
}
