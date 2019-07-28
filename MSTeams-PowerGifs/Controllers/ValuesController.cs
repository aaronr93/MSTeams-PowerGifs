using Microsoft.Bot.Connector;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;

namespace MSTeams.PowerGifs.Controllers
{
    [BotAuthentication(MicrosoftAppIdSettingName = "MicrosoftAppId", MicrosoftAppPasswordSettingName = "MicrosoftAppPassword")]
    public class ValuesController : ApiController
    {
        private static ILogger Logger => LogManager.GetCurrentClassLogger();

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity, CancellationToken cancellationToken)
        {
            if (activity.Type == ActivityTypes.Invoke && activity.IsComposeExtensionQuery())
            {
                //using (var connector = new ConnectorClient(new Uri(activity.ServiceUrl)))
                //{
                    //var response = activity.CreateReply();

                    //response.Attachments.Add(new Attachment("video/mp4", "https://i.imgur.com/2vNlcpP.mp4", null, "reaction gif"));
                    //response.Attachments.Add(new Attachment("image/gif", "https://i.imgur.com/krsxy6a.gif", null, "reaction gif"));

                    //await connector.Conversations.ReplyToActivityAsync(response, cancellationToken);

                    var response = HandleMessageExtensionQuery(activity);
                    return response != null
                        ? Request.CreateResponse(HttpStatusCode.OK, response)
                        : new HttpResponseMessage(HttpStatusCode.InternalServerError);
                //}
            }

            return Request.CreateResponse(HttpStatusCode.NotImplemented);
        }


        public static ComposeExtensionResponse HandleMessageExtensionQuery(Activity activity)
        {
            var query = activity.GetComposeExtensionQueryData();
            Newtonsoft.Json.Linq.JObject data = activity.Value as Newtonsoft.Json.Linq.JObject;
            if (query == null || query.CommandId != "partyParrot")
            {
                // We only process the 'partyParrot' queries with this message extension
                return null;
            }

            var title = "";
            var titleParam = query.Parameters?.FirstOrDefault(p => p.Name == "searchKeyword");
            if (titleParam != null)
            {
                title = titleParam.Value.ToString();
            }

            
            var response = new ComposeExtensionResponse(new ComposeExtensionResult
            {
                AttachmentLayout = "list",
                Type = "result",
                Attachments = new List<ComposeExtensionAttachment>(),
                Text = "Response text"
            });
            
            for (int i = 0; i < 5; i++)
            {
                response.ComposeExtension.Attachments.Add(GetAttachment(title));
            }

            return response;
        }

        private static ComposeExtensionAttachment GetAttachment(string title = null)
        {
            var imgUrl = "https://cultofthepartyparrot.com/parrots/hd/parrot.gif";
            var card = new Attachment("image/gif", "https://i.imgur.com/krsxy6a.gif", null, "reaction gif");

            return card.ToComposeExtensionAttachment();
        }

    }
}
