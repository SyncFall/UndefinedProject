using Be.Runtime.Types;
using System;
using System.Reflection;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using Be.Runtime.Validate;
using Be.Runtime.Convert;

namespace Be.Runtime
{ 
    public class ObjectLoader
    {
        private SourceIndexCollection sourceIndex = new SourceIndexCollection();
        private ObjectIndexCollection objectIndex = new ObjectIndexCollection();

        private InterfaceValidator interfaceValidator;
        private ImplemenationValidator implementationValidator;
        private CodeValidator codeValidator;

        private int ValidatedLength = 0;

        private bool IsStarted = false;
        private Thread ProgrammThread = null;
        private ListCollection<ObjectSymbol> ProgrammEntryPoints = new ListCollection<ObjectSymbol>();

        private SourceFileList temporarySourceCollection;

        public ObjectLoader()
        {
            this.interfaceValidator = new InterfaceValidator(this);
            this.implementationValidator = new ImplemenationValidator(this);
            this.codeValidator = new CodeValidator(this);
        }

        public void Add(SourceFileList sourceCollection)
        {
            this.temporarySourceCollection = sourceCollection;
            sourceIndex.Clear();
            objectIndex.Clear();
            SourceFile sourceType;
            for (int i = 0; i < sourceCollection.Size(); i++)
            {
                sourceType = sourceCollection.Get(i);
                if (!sourceType.isParsed)
                {
                    sourceType.Parse();
                }
                AddSourceObjectsToIndex(sourceType);
            }
            ProcessNewObjects();
        }

        public void Add(SourceFile sourceType)
        {
            sourceIndex.Clear();
            objectIndex.Clear();
            if (!sourceType.isParsed)
            {
                sourceType.Parse();
            }
            AddSourceObjectsToIndex(sourceType);
            ProcessNewObjects();
        }

        private void AddSourceObjectsToIndex(SourceFile sourceType)
        {
            ObjectSymbol objectType;
            SourceIndexEntry sourceEntry;
            for(int i=0; i< sourceType.Namespaces.Size(); i++)
            {
                ObjectCollection objects = sourceType.Namespaces.Get(i).Objects;
                for (int j = 0; j < objects.Size(); j++)
                {
                    // prepare
                    objectType = objects.Get(j);
                    // naming
                    objectType.ObjectPath = objectType.String;
                    objectType.AbsolutePath = (objectType.Namespace.Path + "." + objectType.ObjectPath);
                    // add
                    objectIndex.Add(new ObjectIndexEntry(objectType.AbsolutePath, objectType));
                    // childs
                    AddChildObjectsToIndex(objectType.Namespace, objectType.String, objectType);
                }
            }
            sourceEntry = new SourceIndexEntry(sourceType);
            sourceIndex.Add(sourceEntry);
        }

        private void AddChildObjectsToIndex(NamespaceSymbol NamespaceType, string ParentObjectPath, ObjectSymbol baseObject)
        {
            string objectPath;
            ObjectSymbol objectType;
            for (int i = 0; i < baseObject.Objects.Size(); i++)
            {
                // prepare
                objectType = baseObject.Objects.Get(i);
                objectPath = ParentObjectPath + "." + objectType.String;
                // naming
                objectType.ObjectPath = objectPath;
                objectType.AbsolutePath = (objectType.Namespace.Path + "." + objectType.ObjectPath);
                // add
                objectIndex.Add(new ObjectIndexEntry(objectType.AbsolutePath, objectType));
                // childs
                AddChildObjectsToIndex(NamespaceType, objectPath, objectType);
            }
        }

        private void ProcessNewObjects()
        {
            // check objects, method, member interfaces and those types, if exist (per object oop-interface)
            for (int i=ValidatedLength; i<sourceIndex.Size(); i++)
            {
                interfaceValidator.ValidateSourceType(sourceIndex.Get(i).SourceType);
            }
            // check object-interface implementation for object, method and member properties (per object-hierachie-context - like implemented methods, ..)
            for (int i = ValidatedLength; i < sourceIndex.Size(); i++)
            {
                implementationValidator.ValidateSourceType(sourceIndex.Get(i).SourceType);
            }
            // check code and referencing contexts
            for (int i = ValidatedLength; i < sourceIndex.Size(); i++)
            {
                codeValidator.ValidateSourceType(sourceIndex.Get(i).SourceType);
            }
            // add to valid objects
            ValidatedLength = sourceIndex.Size();
        }

        public void StartProgramm()
        {
            /*
            if (IsStarted)
            {
                throw new Exception("programm already started");
            }
            // check available programm entry-points
            if (ProgrammEntryPoints.Size() == 0)
            {
                throw new Exception("no programm-entry point, can not start");
            }
            else if (ProgrammEntryPoints.Size() > 1)
            {
                throw new Exception("multipe programm-entry points, can not start");
            }
            */
            // run programm in thread
            //ProgrammThread = new Thread(_RuntimeThread);
            //ProgrammThread.Start();
            _RuntimeThread();
        }

        private void _RuntimeThread()
        {
            // run entry-point
            IsStarted = true;

            ObjectConverter objectConverter = new ObjectConverter();
            objectConverter.WriteSourcesAndCompile(this.temporarySourceCollection);


            Assembly assembly = Assembly.LoadFile(@"D:\dev\UndefinedProject\be-output\be-csharp-project.dll");
            Type objType = assembly.GetType("AA.TestObject");
            object objInstance = Activator.CreateInstance(objType);
            object result = objType.InvokeMember("testMethod", BindingFlags.InvokeMethod, null, objInstance, null);

            /*
            Assembly assembly = Assembly.LoadFile(@"D:\dev\BeProject\be_charp\be_ui\bin\Debug\be_ui.exe");
            Type objType = assembly.GetType("Example.MyApplication");
            object objInstance = Activator.CreateInstance(objType);
            object result = objType.InvokeMember("RenderCycle", BindingFlags.InvokeMethod, null, objInstance, null);
            */ 

            Console.WriteLine();
            Console.WriteLine("Result:");
            Console.WriteLine(result);

            IsStarted = false;
        }

        public ObjectSymbol GetObjectType(SourceFile SourceType, string NamespacePath, string ObjectPath)
        {
            if (string.IsNullOrEmpty(NamespacePath))
            {
                return GetObjectType(SourceType, ObjectPath);
            }
            else
            {
                return GetObjectType(SourceType, NamespacePath + "." + ObjectPath);
            }
        }

        public ObjectSymbol GetObjectType(SourceFile SourceType, string ObjectPath)
        {
            // check if native
            NativeSymbol nativeType = Natives.GetTypeByName(ObjectPath);
            if (nativeType != null)
            {
                return nativeType;
            }
            // check if in own package
            ObjectIndexEntry objectEntry = objectIndex.Get(new ObjectIndexEntry(ObjectPath));
            if (objectEntry != null)
            {
                return objectEntry.ObjectType;
            }
            // check if type in object-index through all using-packages
            UsingSymbol usingType;
            for (int i = 0; i < SourceType.Usings.Size(); i++)
            {
                usingType = SourceType.Usings.Get(i);
                objectEntry = objectIndex.Get(new ObjectIndexEntry(ObjectPath));
                if(objectEntry != null)
                {
                    return objectEntry.ObjectType;
                }
            }
            return null;
        }
    }

    public class ObjectIndexCollection : MapCollection<string, ObjectIndexEntry>
    {
        public virtual void Add(ObjectIndexEntry entry)
        {
            if(this.GetValue(entry.AbsolouteObjectPath) != null)
            {
                throw new Exception("duplicate object-index-entry");
            }
            base.Add(entry.AbsolouteObjectPath, entry);
        }
     
        public ObjectIndexEntry Get(ObjectIndexEntry entry)
        {
            return this.GetValue(entry.AbsolouteObjectPath);
        }
    }

    public class ObjectIndexEntry
    {
        public string AbsolouteObjectPath;
        public ObjectSymbol ObjectType;

        public ObjectIndexEntry(string AbsolouteObjectPath)
        {
            this.AbsolouteObjectPath = AbsolouteObjectPath;
        }

        public ObjectIndexEntry(string AbsolouteObjectPath, ObjectSymbol ObjectType)
        {
            this.AbsolouteObjectPath = AbsolouteObjectPath;
            this.ObjectType = ObjectType;
        }

        public bool Validate()
        {
            if (string.IsNullOrEmpty(AbsolouteObjectPath))
            {
                throw new Exception("invalid absolute-object-path-index");
            }
            return true;
        }
    }

    public class SourceIndexCollection : ListCollection<SourceIndexEntry>
    { }

    public class SourceIndexEntry
    {
        public SourceFile SourceType;

        public SourceIndexEntry(SourceFile sourceType)
        {
            this.SourceType = sourceType;
        }
    }
}
