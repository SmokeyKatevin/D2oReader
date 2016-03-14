using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace D2oReader
{
    public class App
    {
        int classCount;

        D2OReader reader;
        JsonUnpacker unpacker;
        Dictionary<int, int> objectPointerTable;
        Dictionary<int, GameDataClassDefinition> classDefinitions;

        public App(string d2oFilePath)
        {
            objectPointerTable = new Dictionary<int, int>();
            classDefinitions = new Dictionary<int, GameDataClassDefinition>();

            using (FileStream d2oFile = File.Open(d2oFilePath, FileMode.Open))
            using (reader = new D2OReader(d2oFile))
            {
                string headerString = reader.ReadAscii(3);

                if (!headerString.Equals("D2O"))
                {
                    throw new InvalidDataException("Header doesn't equal the string 'D2O' : Corrupted file");
                }

                readIndexTable();
                //printIndexTable();
                readClassTable();
                printClassTable();
                readGameDataProcessor(); //TODO: implement
                unpackObjectsAsJson();
                //printAllObjects(); //call after  unpackObjectsAsJson(); 
                searchObjectById(); //call after  unpackObjectsAsJson(); 
                //writeJsonFile(); //TODO: implement
            }
        }

        private void writeJsonFile()
        {
            throw new NotImplementedException();
        }

        private void printAllObjects()
        {
            foreach (var objectPointer in objectPointerTable)
            {
                Console.WriteLine("Class {0}, Object Id {1}:", classDefinitions[getClassId(objectPointer.Key)].Name, objectPointer.Key);
                Console.WriteLine(unpacker.getObjectJsonString(objectPointer.Key));
                Console.WriteLine("Press any key to continue . . .");
                Console.ReadKey();
            }
        }

        private void unpackObjectsAsJson()
        {
            bool isValidJson;

            unpacker = new JsonUnpacker(reader,
                objectPointerTable,
                classDefinitions);

            isValidJson = unpacker.Unpack();
        }

        private int getClassId(int objectId)
        {
            int objectPointer = objectPointerTable[objectId];
            reader.Pointer = objectPointer;

           return reader.ReadInt();
        }

        private void searchObjectById()
        {
            int objectId;
            do
            {
                Console.Write("Search object id: ");
                objectId = Int32.Parse(Console.ReadLine());

                if (objectPointerTable.ContainsKey(objectId))
                {
                    Console.WriteLine("Class {0}, Object Id {1}:", classDefinitions[getClassId(objectId)].Name, objectId);
                    Console.WriteLine( unpacker.getObjectJsonString(objectId) );
                }
                else
                {
                    Console.WriteLine("Object of id: {0} is not present.",objectId);
                }

            } while(objectId != 0) ;
        }

        private void printClassTable()
        {
            if (classDefinitions.Count > 0)
            {
                Console.WriteLine("Printing {0} class tables.", classDefinitions.Count);
                Console.WriteLine();
                foreach (var @class in classDefinitions)
                {
                    Console.WriteLine("Class id:{0} - name {1}",@class.Key,@class.Value.Name);
                    Console.WriteLine();

                    foreach (GameDataField field in @class.Value.Fields)
                    {
                        printField(getFieldString(field));
                    }
                    Console.WriteLine();
                }
            }
        }

        private string getFieldString(GameDataField field)
        {
            StringBuilder fieldBuilder = new StringBuilder();

            fieldBuilder
                .Append("public")
                .Append(" ")
                .Append(getFieldTypeString(field))
                .Append(" ")
                .Append(getFieldNameString(field));

            return fieldBuilder.ToString();
        }

        private void printField(string fieldString)
        {
            Console.WriteLine(fieldString);
        }

        private string getFieldTypeString(GameDataField field)
        {
            if (isPrimitiveFieldType(field))
            {
                return getPrimitiveFieldTypeString(field);
            }
            else
            {
                return getCompositeFieldTypeString(field);
            }
        }

        private string getCompositeFieldTypeString(GameDataField field)
        {
            StringBuilder compositeFieldTypeBuilder = new StringBuilder();

            compositeFieldTypeBuilder
                .Append("vector")
                .Append("<")
                .Append(getFieldTypeString(field.innerField))
                .Append(">");

            return compositeFieldTypeBuilder.ToString();
        }

        private string getPrimitiveFieldTypeString(GameDataField field)
        {
            return field.fieldType > 0 ? classDefinitions[(int)field.fieldType].Name : field.fieldType.ToString();
        }

        private string getFieldNameString(GameDataField field)
        {
            return field.fieldName;
        }

        private static bool isPrimitiveFieldType(GameDataField field)
        {
            return field.innerField == null;
        }

        private void readGameDataProcessor()
        {
            if (reader.BytesAvailable > 0)
            {
                //GameDataProcess(stream);
            };
        }

        private void readClassTable()
        {
            classCount = reader.ReadInt();
            int classId;

            int j = 0;
            while (j < classCount)
            {
                classId = reader.ReadInt();
                readClassDefinition(classId);

                j++;
            }
        }

        private void readClassDefinition(int classId)
        {
            string className = reader.ReadUtf8();
            string packageName = reader.ReadUtf8();
            GameDataClassDefinition classDefinition = new GameDataClassDefinition(packageName, className);
            Console.WriteLine("ClassId: {0} ClassMemberName: {1} ClassPkgName {2}", classId, className, packageName);
            int fieldsCount = reader.ReadInt();
            uint i=0;
            while(i < fieldsCount)
            {
                classDefinition.AddField(reader);
                i++;
            }
            classDefinitions.Add(classId, classDefinition);
        }

        private void printObjectPointerTable()
        {
            if (objectPointerTable.Count > 0)
            {
                foreach (var objectPointer in objectPointerTable)
                {
                    Console.WriteLine("{0}: {1}", objectPointer.Key, objectPointer.Value);
                }
            }
        }

        private void readIndexTable()
        {
            int headerOffset = reader.ReadInt();
            reader.Pointer = headerOffset;

            int objectPointerTableLen = reader.ReadInt();

            int key;
            int pointer;

            for (uint i = 0; i < objectPointerTableLen; i += sizeof(int) * 2)
            {
                key = reader.ReadInt();
                pointer = reader.ReadInt();

                objectPointerTable.Add(key, pointer);
            }
        }
    }
}

