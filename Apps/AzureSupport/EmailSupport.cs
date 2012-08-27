using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.WindowsAzure;

namespace TheBall
{
    public static class EmailSupport
    {
        private static string AWSAccessKey;
        private static string AWSSecretKey;

        static EmailSupport()
        {

            const string SecretFileName = @"C:\work\abs\ConnectionStringStorage\amazonses.txt";
            string configString;
            if (File.Exists(SecretFileName))
                configString = File.ReadAllText(SecretFileName);
            else
                configString = CloudConfigurationManager.GetSetting("AmazonSESAccessInfo");
            string[] strValues = configString.Split(';');
            AWSAccessKey = strValues[0];
            AWSSecretKey = strValues[1];
        }

        public static Boolean SendEmail(String From, String To, String Subject, String Text = null, String HTML = null, String emailReplyTo = null, String returnPath = null)
        {
            if (Text != null || HTML != null)
            {
                String from = From;

                List<String> to
                    = To
                        .Replace(", ", ",")
                        .Split(',')
                        .ToList();

                Destination destination = new Destination();
                destination.WithToAddresses(to);
                //destination.WithCcAddresses(cc);
                //destination.WithBccAddresses(bcc);

                Content subject = new Content();
                subject.WithCharset("UTF-8");
                subject.WithData(Subject);

                Body body = new Body();


                if (HTML != null)
                {
                    Content html = new Content();
                    html.WithCharset("UTF-8");
                    html.WithData(HTML);
                    body.WithHtml(html);
                }

                if(Text != null)
                {
                    Content text = new Content();
                    text.WithCharset("UTF-8");
                    text.WithData(Text);
                    body.WithText(text);
                }

                Message message = new Message();
                message.WithBody(body);
                message.WithSubject(subject);

                string awsAccessKey = AWSAccessKey;
                string awsSecretKey = AWSSecretKey;
                //AmazonSimpleEmailService ses = AWSClientFactory.CreateAmazonSimpleEmailServiceClient(AppConfig["AWSAccessKey"], AppConfig["AWSSecretKey"]);
                AmazonSimpleEmailService ses = AWSClientFactory.CreateAmazonSimpleEmailServiceClient(awsAccessKey, awsSecretKey);

                SendEmailRequest request = new SendEmailRequest();
                request.WithDestination(destination);
                request.WithMessage(message);
                request.WithSource(from);

                if (emailReplyTo != null)
                {
                    List<String> replyto
                        = emailReplyTo
                            .Replace(", ", ",")
                            .Split(',')
                            .ToList();

                    request.WithReplyToAddresses(replyto);
                }

                if (returnPath != null)
                {
                    request.WithReturnPath(returnPath);
                }

                try
                {
                    SendEmailResponse response = ses.SendEmail(request);

                    SendEmailResult result = response.SendEmailResult;

                    Console.WriteLine("Email sent.");
                    Console.WriteLine(String.Format("Message ID: {0}",
                                                    result.MessageId));

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                    return false;
                }
            }

            Console.WriteLine("Specify Text and/or HTML for the email body!");

            return false;
        }
    }
}