﻿using System;
using System.Collections.Generic;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using eg_03_csharp_auth_code_grant_core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using DocuSign.eSign.Client;

namespace eg_03_csharp_auth_code_grant_core.Controllers
{
    [Route("eg017")]
    public class Eg017SetTemplateTabValuesController : EgController
    {
        // Set up the Ping Url, signer client ID, and the return (callback) URL for embedded signing
        private string dsPingUrl;
        private readonly string signerClientId = "1000";
        private string dsReturnUrl;

        public Eg017SetTemplateTabValuesController(DSConfiguration config, IRequestItemsService requestItemsService)
            : base(config, requestItemsService)
        {
            ViewBag.title = "SetTabValues";
            dsPingUrl = config.AppUrl + "/";
            dsReturnUrl = config.AppUrl + "/dsReturn";
        }

        public override string EgName => "eg017";

        [HttpPost]
        public IActionResult Create(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            // Check the token with minimal buffer time
            bool tokenOk = CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation so it could be 
                // restarted automatically. But since it should be rare to have a token issue
                // here, we'll make the user re-enter the form data after authentication
                RequestItemsService.EgName = EgName;
                return Redirect("/ds/mustAuthenticate");
            }

            // The envelope will be sent first to the signer; after it is signed,
            // a copy is sent to the cc person
            //
            // Read files from a local directory
            // The reads could raise an exception if the file is not available!
            var basePath = RequestItemsService.Session.BasePath + "/restapi";

            // Step 1: Obtain your OAuth token
            var accessToken = RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            // Step 2: Construct your API headers
            var config = new Configuration(new ApiClient(basePath));
            config.AddDefaultHeader("Authorization", "Bearer " + accessToken);

            // Step 3: Create Tabs and CustomFields
            // Set the values for the fields in the template
            // List item
            List colorPicker = new List
            {
                Value = "green",
                DocumentId = "1",
                PageNumber = "1",
                TabLabel = "list"
            };

            // Checkboxes
            Checkbox ckAuthorization = new Checkbox
            {
                TabLabel = "ckAuthorization",
                Selected = "true"
            };
             Checkbox ckAgreement = new Checkbox
            {
                TabLabel = "ckAgreement",
                Selected = "true"
            };

            RadioGroup radioGroup = new RadioGroup
            {
                GroupName = "radio1",
                // You only need to provide the readio entry for the entry you're selecting
                Radios = new List<Radio> { new Radio { Value = "white", Selected = "true" } }
            };

            Text includedOnTemplate = new Text
            {
                TabLabel = "text",
                Value = "Jabberywocky!"
            };

            // We can also add a new tab (field) to the ones already in the template
            Text addedField = new Text
            {
                DocumentId = "1",
                PageNumber = "1",
                XPosition = "280",
                YPosition = "172",
                Font = "helvetica",
                FontSize = "size14",
                TabLabel = "added text field",
                Height = "23",
                Width = "84",
                Required = "false",
                Bold = "true",
                Value = signerName,
                Locked = "false",
                TabId = "name"
            };

            // Add the tabs model (including the SignHere tab) to the signer.
            // The Tabs object wants arrays of the different field/tab types
            // Tabs are set per recipient/signer
            Tabs tabs = new Tabs
            {
                CheckboxTabs = new List<Checkbox> { ckAuthorization, ckAgreement },
                RadioGroupTabs = new List<RadioGroup> { radioGroup },
                TextTabs = new List<Text> { includedOnTemplate, addedField },
                ListTabs = new List<List> { colorPicker }
            };

            // Create a signer recipient to sign the document, identified by name and email
            // We're setting the parameters via the object creation
            TemplateRole signer = new TemplateRole
            {
                Email = signerEmail,
                Name = signerName,
                RoleName = "signer",
                ClientUserId = signerClientId, // Change the signer to be embedded
                Tabs = tabs //Set tab values
            };

            TemplateRole cc = new TemplateRole
            {
                Email = ccEmail,
                Name = ccName,
                RoleName = "cc"
            };

            // Create an envelope custom field to save our application's
            // data about the envelope
            TextCustomField customField = new TextCustomField
            {
                Name = "app metadata item",
                Required = "false",
                Show = "true", // Yes, include in the CoC
                Value = "1234567"
            };

            CustomFields cf = new CustomFields
            {
                TextCustomFields = new List<TextCustomField> { customField }
            };

            // Step 4: Create the envelope definition
            EnvelopeDefinition envelopeAttributes = new EnvelopeDefinition
            {
                // Uses the template ID received from example 08
                TemplateId = RequestItemsService.TemplateId,
                Status = "Sent",
                // Add the TemplateRole objects to utilize a pre-defined
                // document and signing/routing order on an envelope.
                // Template role names need to match what is available on
                // the correlated templateID or else an error will occur
                TemplateRoles = new List<TemplateRole> { signer, cc },
                CustomFields = cf
            };

            // Added by DT to popuplate own template
            Text tfName = new Text
            {
                TabLabel = "firstName",
                Value = "Jabberywocky First Name"
            };
            Text tSurname = new Text
            {
                TabLabel = "lastName",
                Value = "Suriname"
            };
            Text tResAdd = new Text
            {
                TabLabel = "residentialAddress",
                Value = "57 Grange Road"
            };
            Text tDOB = new Text
            {
                TabLabel = "dob",
                Value = "08/08/1888"
            };
            Text tmobNo = new Text
            {
                TabLabel = "mobileNo",
                Value = "1234567890"
            };
            Text tDenticareID = new Text
            {
                TabLabel = "dentiCareAccNo",
                Value = "111112222233333"
            };
            Text tSuburb = new Text
            {
                TabLabel = "residentialSuburb",
                Value = "Tanjung Katong"
            };
            Text tPostCode = new Text
            {
                TabLabel = "residentialPostCode",
                Value = "437111"
            };

            // Add the tabs model (including the SignHere tab) to the signer.
            // The Tabs object wants arrays of the different field/tab types
            // Tabs are set per recipient/signer
            Tabs tabsCoD = new Tabs
            {
                //CheckboxTabs = new List<Checkbox> { ckAuthorization, ckAgreement },
                //RadioGroupTabs = new List<RadioGroup> { radioGroup },
                TextTabs = new List<Text> { tfName, tSurname, tResAdd, tDOB, tmobNo , tDenticareID, tSuburb, tPostCode},
                //ListTabs = new List<List> { colorPicker }
            };

            // Create a signer recipient to sign the document, identified by name and email
            // We're setting the parameters via the object creation
            TemplateRole responsibleParty = new TemplateRole
            {
                Email = signerEmail,
                Name = signerName,
                RoleName = "Responsible Party",
                ClientUserId = signerClientId, // Change the signer to be embedded
                Tabs = tabsCoD //Set tab values
            };

            // Step 4: Create the envelope definition
            EnvelopeDefinition envelopeAttributesCoD = new EnvelopeDefinition
            {
                // Uses the template ID received from example 08
                TemplateId = RequestItemsService.TemplateId,
                Status = "Sent",
                // Add the TemplateRole objects to utilize a pre-defined
                // document and signing/routing order on an envelope.
                // Template role names need to match what is available on
                // the correlated templateID or else an error will occur
                TemplateRoles = new List<TemplateRole> { responsibleParty },
            };
            // End of addition by DT

            // Step 5: Call the eSignature REST API
            EnvelopesApi envelopesApi = new EnvelopesApi(config);
            //EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, envelopeAttributes);
            EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, envelopeAttributesCoD); // Added by DT

            // Step 6: Create the View Request
            RequestItemsService.EnvelopeId = results.EnvelopeId;
            RecipientViewRequest viewRequest = new RecipientViewRequest();
            // Set the URL where you want the recipient to go once they are done signing;
            // this should typically be a callback route somewhere in your app.
            // The query parameter is included as an example of how
            // to save/recover state information during the redirect to
            // the DocuSign signing ceremony. It's usually better to use
            // the session mechanism of your web framework. Query parameters
            // can be changed/spoofed very easily
            viewRequest.ReturnUrl = dsReturnUrl + "?state=123";

            // How has your app authenticated the user? In addition to your app's authentication,
            // you can include authentication steps from DocuSign; e.g., SMS authentication
            viewRequest.AuthenticationMethod = "none";

            // Recipient information must match the embedded recipient info
            // that we used to create the envelope
            viewRequest.Email = signerEmail;
            viewRequest.UserName = signerName;
            viewRequest.ClientUserId = signerClientId;

            // DocuSign recommends that you redirect to DocuSign for the
            // signing ceremony. There are multiple ways to save state.
            // To maintain your application's session, use the PingUrl
            // parameter. It causes the DocuSign Signing Ceremony web page
            // (not the DocuSign server) to send pings via AJAX to your app
            viewRequest.PingFrequency = "600"; // seconds
                                               // NOTE: The pings will only be sent if the pingUrl is an HTTPS address
            viewRequest.PingUrl = dsPingUrl; // Optional setting

            ViewUrl results1 = envelopesApi.CreateRecipientView(accountId, results.EnvelopeId, viewRequest);
            //***********
            // Don't use an iframe with embedded signing requests!
            //***********
            // State can be stored/recovered using the framework's session or a
            // query parameter on the return URL (see the makeRecipientViewRequest method)
            string redirectUrl = results1.Url;
            return Redirect(redirectUrl);
        }
    }
}
