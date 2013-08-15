using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AaltoGlobalImpact.OIP;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.WindowsAzure;
using TheBall.CORE;

namespace TheBall
{
    public static class EmailSupport
    {
        private static string AWSAccessKey;
        private static string AWSSecretKey;
        private const string FromAddress = "no-reply-theball@msunit.citrus.fi";

        static EmailSupport()
        {

            const string SecretFileName = @"C:\users\kalle\work\ConnectionStringStorage\amazonses.txt";
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

        public static void SendValidationEmail(TBEmailValidation emailValidation)
        {
            string urlLink = GetUrlLink(emailValidation.ID);
            string emailMessageFormat =
                @"Welcome to The Open Innovation Platform!

You have just joined the collaboration platform by Aalto Global Impact. Your email address '{0}' has been registered on the OIP system. Before you start your collaboration we simply need to confirm that you did register your email. Please follow the link below during which you might be redirected to perform the authentication on OIP.

Use the following link to complete your registration (the link is valid for 30 minutes after which you need to resend the validation):
{1}

Wishing you all the best from OIP team!";
            string message = string.Format(emailMessageFormat, emailValidation.Email, urlLink);
            SendEmail(FromAddress, emailValidation.Email, "Welcome to The Open Innovation Platform!", message);
        }

        public static void SendGroupJoinEmail(TBEmailValidation emailValidation, TBCollaboratingGroup collaboratingGroup)
        {
            string urlLink = GetUrlLink(emailValidation.ID);
            string emailMessageFormat =
                @"You have been invited to join in the collaboration platform by Aalto Global Impact to collaborate in the group: {0}. 

Use the following link to accept the invitation and join the group:
{1}

The link is valid for 14 days, after which you need to request new invitation.";
            string message = String.Format(emailMessageFormat, collaboratingGroup.Title, urlLink);
            SendEmail(FromAddress, emailValidation.Email,
                      "Invitation to join collaboration group: " + collaboratingGroup.Title, message);
        }

        private static string GetUrlLink(string emailValidationID)
        {
            string urlLink = "http://demooip.aaltoglobalimpact.org/emailvalidation/" + emailValidationID;
            return urlLink;
        }

        public static void SendDeviceJoinEmail(TBEmailValidation emailValidation, DeviceMembership deviceMembership, string[] ownerEmailAddresses)
        {
            string urlLink = GetUrlLink(emailValidation.ID);
            bool isAccount = emailValidation.DeviceJoinConfirmation.AccountID != null;
            string ownerID = isAccount
                                 ? emailValidation.DeviceJoinConfirmation.AccountID
                                 : emailValidation.DeviceJoinConfirmation.GroupID;
            string emailMessageFormat =
                @"Your confirmation is required to trust the following device '{0}' to be joined to trust within {1} ID {2}. 

Click the following link to confirm this action:
{3}";
            string message = String.Format(emailMessageFormat, deviceMembership.DeviceDescription,
                                           isAccount ? "account" : "collaboration group", ownerID, urlLink);
            foreach (string emailAddress in ownerEmailAddresses)
            {
                SendEmail(FromAddress, emailAddress, "Device Join Confirmation", message);
            }
        }
    }
}