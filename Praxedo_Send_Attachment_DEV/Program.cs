using Praxedo_Send_Attachment_DEV.AttachmentManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Praxedo_Send_Attachment_DEV
{
    internal class Program
    {
        /// <summary>
        /// sends doc to corresponding Praxedo Ticket
        /// </summary>

        // please add a test document in the dir below
        /// "C:\Praxedo\_Test_Add_Attachments\TEST.txt"

        static void Main(string[] args)
        {
            int Amount = 1;
            var ticket = new Ticket();
            string wo_to_test = "259885-1"; //needs to be a ticket in Qualified state

            ticket.API_UserName = "drf.webservice";//replace with yours
            ticket.API_Password = "DRFPraxedo01!";// same

            ticket.TEST_MODE_ON = true;
            ticket.TEST_WORKORDERS = new string[Amount];
            ticket.TEST_WORKORDERS[0] = wo_to_test;
            ticket.File_Path = @"C:\Praxedo\_Test_Add_Attachments\TEST.txt";
            ticket.File_Name = @"C:\Praxedo\_Test_Add_Attachments\TEST.txt";

            try
            {
                SendAttachment_Dev(ticket);
            }
            catch (Exception)
            {
                Console.WriteLine("ha nope");
                throw;
            }
            //Process_Notes(Amount);
            //Process_Attachments(Amount);

            Console.WriteLine();
        }
        public static bool SendAttachment_Dev(Ticket t)
        {
            string ALTERNATEKEYSUFFIX = t.TEST_WORKORDERS[0];
            string FILENAME = Path.GetFileName(t.File_Path);
            string PATH = t.File_Path;

            if (File.Exists(@PATH))
            {
                Console.WriteLine("PATH:" + PATH);
                byte[] fileContent = System.IO.File.ReadAllBytes(@PATH);

                Console.WriteLine(fileContent.Length);


                var clientAttach = new BusinessEventAttachmentManagerClient();
                clientAttach.ClientCredentials.UserName.UserName = t.API_UserName;
                clientAttach.ClientCredentials.UserName.Password = t.API_Password;


                clientAttach.Open();
                OperationContextScope scope = new OperationContextScope(clientAttach.InnerChannel);
                var httpRequestProperty = new System.ServiceModel.Channels.HttpRequestMessageProperty();
                httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(clientAttach.ClientCredentials.UserName.UserName + ":" + clientAttach.ClientCredentials.UserName.Password));
                OperationContext.Current.OutgoingMessageProperties[System.ServiceModel.Channels.HttpRequestMessageProperty.Name] = httpRequestProperty;

                string id = ALTERNATEKEYSUFFIX;

                var attachment = new AttachmentManager.attachment
                {
                    entityId = id,
                    name = "test.png",
                    extensions = null

                };
                attachment.name = FILENAME;

                try
                {
                    var res = clientAttach.createAttachment(attachment, fileContent, new AttachmentManager.wsEntry[1]);
                    if (res.resultCode.Equals(0))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                catch (Exception e)
                {

                    return false;
                    throw e;
                }

            }
            else
            {
                Console.WriteLine("path not found");
                return false;
            }

            return false;
        }
        public class Ticket
        {
            public string API_UserName { get; set; }
            public string API_Password { get; set; }

            public bool TEST_MODE_ON { get; set; }
            public string TEST_WORKORDER { get; set; }
            public string[] TEST_WORKORDERS { get; set; }

            public string uuid { get; set; }
            public string wo { get; set; }
            public string wot { get; set; }
            public string oppid { get; set; }
            public string customer { get; set; }
            public string customerF { get; set; }

            public string Download_Path { get; set; }
            public string Download_FileName { get; set; }
            public string Unparsed_Document_Path { get; set; }
            public string File_Path { get; set; }
            public string File_Name { get; set; }
            public string Parsed_Path { get; set; }

            public string StagingFileName { get; set; }
            public string StagingFilePath { get; set; }
            public string ParsedFileName { get; set; }
            public string ParsedFilePath { get; set; }
            public string Current_ParsedFileName { get; set; }
            public string FileServer_BasePath { get; set; }
            public string FileServer_ParsedFilePath { get; set; }
            public string FullFileSize { get; set; }





        }
    }
}
