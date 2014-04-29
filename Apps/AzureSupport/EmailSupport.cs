using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AaltoGlobalImpact.OIP;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using TheBall.CORE;

namespace TheBall
{
    public static class EmailSupport
    {
        private static readonly string AWSAccessKey = InstanceConfiguration.AWSAccessKey;
        private static readonly string AWSSecretKey = InstanceConfiguration.AWSSecretKey;
        private static readonly string FromAddress = InstanceConfiguration.EmailFromAddress;

        static EmailSupport()
        {
        }

        public static Boolean SendEmail(String From, String To, String Subject, String Text = null, String HTML = null, String emailReplyTo = null, String returnPath = null)
        {
            if (Text != null || HTML != null)
            {
                try
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

                    if (Text != null)
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
                    AmazonSimpleEmailService ses = AWSClientFactory.CreateAmazonSimpleEmailServiceClient(awsAccessKey,
                                                                                                         awsSecretKey);

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
                    //throw;
                    return false;
                }
                finally
                {
                    String queueMessage = String.Format("From: {1}{0}To: {2}{0}Subject: {3}{0}Message:{0}{4}",
                                                        Environment.NewLine, From, To, Subject, Text ?? HTML);
                    QueueSupport.CurrStatisticsQueue.AddMessage(new CloudQueueMessage(queueMessage));
                }
            }

            Console.WriteLine("Specify Text and/or HTML for the email body!");

            return false;
        }

        public static void SendValidationEmail(TBEmailValidation emailValidation)
        {
            string urlLink = GetUrlLink(emailValidation.ID);
            string emailMessageFormat = InstanceConfiguration.EmailValidationMessageFormat;
#if never
#endif
            string message = string.Format(emailMessageFormat, emailValidation.Email, urlLink);
            SendEmail(FromAddress, emailValidation.Email, InstanceConfiguration.EmailValidationSubjectFormat, message);
        }

        public static void SendGroupJoinEmail(TBEmailValidation emailValidation, TBCollaboratingGroup collaboratingGroup)
        {
            string urlLink = GetUrlLink(emailValidation.ID);
            string emailMessageFormat = InstanceConfiguration.EmailGroupJoinMessageFormat;
            string message = String.Format(emailMessageFormat, collaboratingGroup.Title, urlLink);
            SendEmail(FromAddress, emailValidation.Email,
                String.Format(InstanceConfiguration.EmailGroupJoinSubjectFormat, collaboratingGroup.Title),
                      message);
        }

        public static void SendMergeAccountsConfirmationEmail(TBEmailValidation mergeAccountEmailConfirmation)
        {
            string urlLink = GetUrlLink(mergeAccountEmailConfirmation.ID);
            string emailMessageFormat = InstanceConfiguration.EmailAccountMergeValidationMessageFormat;
#if never
#endif
            string message = string.Format(emailMessageFormat, mergeAccountEmailConfirmation.Email, urlLink);
            SendEmail(FromAddress, mergeAccountEmailConfirmation.Email, InstanceConfiguration.EmailAccountMergeValidationSubjectFormat, message);
        }

        private static string GetUrlLink(string emailValidationID)
        {
            string urlLink = InstanceConfiguration.EmailValidationURLWithoutID + emailValidationID;
            return urlLink;
        }

        public static void SendDeviceJoinEmail(TBEmailValidation emailValidation, DeviceMembership deviceMembership, string[] ownerEmailAddresses)
        {
            string urlLink = GetUrlLink(emailValidation.ID);
            bool isAccount = emailValidation.DeviceJoinConfirmation.AccountID != null;
            string ownerID = isAccount
                                 ? emailValidation.DeviceJoinConfirmation.AccountID
                                 : emailValidation.DeviceJoinConfirmation.GroupID;
            string emailMessageFormat = InstanceConfiguration.EmailDeviceJoinMessageFormat;
            string message = String.Format(emailMessageFormat, deviceMembership.DeviceDescription,
                                           isAccount ? "account" : "collaboration group", ownerID, urlLink);
            string subject = String.Format(InstanceConfiguration.EmailDeviceJoinSubjectFormat, ownerID);
            foreach (string emailAddress in ownerEmailAddresses)
            {
                SendEmail(FromAddress, emailAddress, subject, message);
            }
        }

        public static void SendInputJoinEmail(TBEmailValidation emailValidation, InformationInput informationInput, string[] ownerEmailAddresses)
        {
            string urlLink = GetUrlLink(emailValidation.ID);
            bool isAccount = emailValidation.InformationInputConfirmation.AccountID != null;
            string ownerID = isAccount
                                 ? emailValidation.InformationInputConfirmation.AccountID
                                 : emailValidation.InformationInputConfirmation.GroupID;
            string emailMessageFormat = InstanceConfiguration.EmailInputJoinMessageFormat;
            string message = String.Format(emailMessageFormat, informationInput.InputDescription,
                                           isAccount ? "account" : "collaboration group", ownerID, urlLink);
            string subject = String.Format(InstanceConfiguration.EmailInputJoinSubjectFormat, ownerID);
            foreach (string emailAddress in ownerEmailAddresses)
            {
                SendEmail(FromAddress, emailAddress, subject, message);
            }
        }

        public static void SendOutputJoinEmail(TBEmailValidation emailValidation, InformationOutput informationOutput, string[] ownerEmailAddresses)
        {
            string urlLink = GetUrlLink(emailValidation.ID);
            var confirmation = emailValidation.InformationOutputConfirmation;
            bool isAccount = confirmation.AccountID != null;
            string ownerID = isAccount
                                 ? confirmation.AccountID
                                 : confirmation.GroupID;
            string emailMessageFormat = InstanceConfiguration.EmailOutputJoinMessageFormat;
            string message = String.Format(emailMessageFormat, informationOutput.OutputDescription,
                                           isAccount ? "account" : "collaboration group", ownerID, urlLink);
            string subject = String.Format(InstanceConfiguration.EmailOutputJoinSubjectFormat, ownerID);
            foreach (string emailAddress in ownerEmailAddresses)
            {
                SendEmail(FromAddress, emailAddress, subject, message);
            }
        }
    }
}