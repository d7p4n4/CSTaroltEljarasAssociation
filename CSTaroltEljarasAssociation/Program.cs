using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using CSLibAc4yObjectObjectService;
using CSLibAc4yObjectDBCap;
using System.Collections.Generic;
using d7p4n4Namespace.Final.Class;
using d7p4n4Namespace.EFMethods.Class;
//using CSAc4yObjectGetGuidCAP;

namespace CSAc4yObjectServiceTest
{
    class Program
    {

        #region konstans

        private static readonly log4net.ILog _naplo = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string APPSETTINGS_SERVER = "80.211.241.82";
        private const string APPSETTINGS_USER = "root";
        private const string APPSETTINGS_PASSWORD = "Sycompla9999*";
        private const string APPSETTINGS_DATABASE = "Ac4yXMLObjectDb";
        private const string APPSETTINGS_CONNECTIONPARAMETER = "CONNECTIONPARAMETER";

        #endregion // konstans

        public SqlConnection DatabaseConnection { get; set; }

        public static SqlConnection conn = new SqlConnection("Data Source=80.211.241.82;Integrated Security=False;uid=root;pwd=Sycompla9999*;Initial Catalog=ModulDb;");

        public static SqlConnection conn2 = new SqlConnection("Data Source=80.211.241.82;Integrated Security=False;uid=root;pwd=Sycompla9999*;Initial Catalog=Ac4yXMLObjectDb;");


        public Program()
        {

            try
            {

                DatabaseConnection = SetupDatabaseConnection();

                DatabaseConnection.Open();

                if (!DatabaseConnection.State.Equals(ConnectionState.Open))
                    throw new Exception("Nem kapcsolódik az adatbázishoz!");

            }
            catch (Exception exception)
            {
                _naplo.Error(exception.Message);
                //_naplo.Error(exception.Message+"\n"+ exception.StackTrace);

            }

        }

        public string GetAsXml(Object aObject)
        {

            string result = "";

            try
            {

                XmlSerializer xmlSerializer = new XmlSerializer(aObject.GetType());

                using (StringWriter stringWriter = new StringWriter())
                {
                    XmlWriterSettings settings = new XmlWriterSettings { Indent = true };

                    using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
                    {
                        xmlSerializer.Serialize(xmlWriter, aObject);
                    }

                    result = stringWriter.ToString();
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
            }

            return result;

        } // GetAsXml

        public Object deser(string xml, Type anyType)
        {
            Object result = null;

            XmlSerializer serializer = new XmlSerializer(anyType);
            using (TextReader reader = new StringReader(xml))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }

    private SqlConnection SetupDatabaseConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings[APPSETTINGS_CONNECTIONPARAMETER].ConnectionString);
        }
        static void Main(string[] args)
        {

            Console.WriteLine("Hello adfgarfdvWorld!");
            Program program = new Program();

            try
            {
                string[] files =
                    Directory.GetFiles("d:\\Server\\Visual_studio\\output_Xmls\\StorProcs\\", "*.xml", SearchOption.TopDirectoryOnly);

                string[] tablak =
                    Directory.GetFiles("d:\\Server\\Visual_studio\\output_Xmls\\arntesztTablak\\", "*.xml", SearchOption.TopDirectoryOnly);

                Console.WriteLine(files.Length);
                conn.Open();

                foreach (var tabla in tablak)
                {
                    string _tablaFilename = Path.GetFileNameWithoutExtension(tabla);
                    Console.WriteLine(_tablaFilename);
                    Tabla tablaObj = new Tabla();
                    TablaOszlop TablaOszlopObj = new TablaOszlop();

                    string textFile = Path.Combine("d:\\Server\\Visual_studio\\output_Xmls\\arntesztTablak\\", _tablaFilename + ".xml");

                    string text = File.ReadAllText(textFile);
                    tablaObj = (Tabla)program.deser(text, typeof(Tabla));
                    string originName = tablaObj.Megnevezes;
                    Ac4yXMLObjectEntityMethods ac4YXMLObjectEntityMethods = new Ac4yXMLObjectEntityMethods(APPSETTINGS_SERVER, APPSETTINGS_DATABASE, APPSETTINGS_USER, APPSETTINGS_PASSWORD);
                    Ac4yObjectObjectService ac4YObjectObjectService = new Ac4yObjectObjectService(conn);

                    foreach (TablaOszlop oszlop in tablaObj.TablaOszlopLista)
                    {
                        SetByNamesResponse response = ac4YObjectObjectService.SetByNames(
                            new SetByNamesRequest() { TemplateName = "tábla oszlop", Name = oszlop.Kod }
                            );

                        string argText = program.GetAsXml(oszlop);
                        ac4YXMLObjectEntityMethods.addNew(new Ac4yXMLObject() { serialization = argText, GUID = response.Ac4yObjectHome.GUID });
                      /*
                         Console.WriteLine(oszlop.Kod);
                         Ac4yAssociationObjectService.SetByNamesResponse setByNamesResponse =
                             new Ac4yAssociationObjectService(conn).SetByNames(
                                 new Ac4yAssociationObjectService.SetByNamesRequest() { originTemplateName = "tábla", originName = originName, targetTemplateName = "tábla oszlop", targetName = oszlop.Kod, associationPathName = "tábla.oszlop" }
                             );

                    */
                    }
                }

                foreach (var _file in files)
                {
                    string _filename = Path.GetFileNameWithoutExtension(_file);
                    Console.WriteLine(_filename);
                    TaroltEljaras taroltEljaras = new TaroltEljaras();
                    TaroltEljarasArgumentum taroltEljarasArg = new TaroltEljarasArgumentum();


                    string textFile = Path.Combine("d:\\Server\\Visual_studio\\output_Xmls\\StorProcs\\", _filename + ".xml");

                    string text = File.ReadAllText(textFile);
                    taroltEljaras = (TaroltEljaras) program.deser(text, typeof(TaroltEljaras));
                    string originName = taroltEljaras.Megnevezes;
                    Ac4yXMLObjectEntityMethods ac4YXMLObjectEntityMethods = new Ac4yXMLObjectEntityMethods(APPSETTINGS_SERVER, APPSETTINGS_DATABASE, APPSETTINGS_USER, APPSETTINGS_PASSWORD);
                    Ac4yObjectObjectService ac4YObjectObjectService = new Ac4yObjectObjectService(conn);

                    foreach (TaroltEljarasArgumentum arg in taroltEljaras.ArgumentumLista)
                    {/*
                        SetByNamesResponse response = ac4YObjectObjectService.SetByNames(
                            new SetByNamesRequest() { TemplateName = "tárolt eljárás argumentum", Name = arg.BelsoNev }
                            );

                        string argText = program.GetAsXml(arg);
                        ac4YXMLObjectEntityMethods.addNew(new Ac4yXMLObject() { serialization = argText, GUID = response.Ac4yObjectHome.GUID });

                        /* Console.WriteLine(arg.BelsoNev);
                         Ac4yAssociationObjectService.SetByNamesResponse setByNamesResponse =
                             new Ac4yAssociationObjectService(conn).SetByNames(
                                 new Ac4yAssociationObjectService.SetByNamesRequest() { originTemplateName = "tárolt eljárás", originName = originName, targetTemplateName = "tárolt eljárás argumentum", targetName = arg.BelsoNev, associationPathName = "tárolt eljárás.Argumentum" }
                             );*/

                    }


                }
            }catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

        }
    }
}
