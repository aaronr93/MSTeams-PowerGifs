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

        /// <summary>
        /// Implement this method to run diagnostics on the live version of your bot.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("healthcheck")]
        public async Task<string> HealthCheck()
        {
            return "OK";
        }

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity, CancellationToken cancellationToken)
        {
            using (var connector = new ConnectorClient(new Uri(activity.ServiceUrl)))
            {
                var response = HandleMessageExtensionQuery(connector, activity);
                return response != null
                    ? Request.CreateResponse(response)
                    : new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }


        public static ComposeExtensionResponse HandleMessageExtensionQuery(ConnectorClient connector, Activity activity)
        {
            var query = activity.GetComposeExtensionQueryData();
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

            var attachments = new ComposeExtensionAttachment[5];
            for (int i = 0; i < 5; i++)
            {
                attachments[i] = GetAttachment(title);
            }
            
            var response = new ComposeExtensionResponse(new ComposeExtensionResult
            {
                AttachmentLayout = "grid",
                Type = "result",
                Attachments = attachments.ToList()
            });

            return response;
        }

        private static ComposeExtensionAttachment GetAttachment(string title = null)
        {
            var imgUrl = "https://cultofthepartyparrot.com/parrots/hd/parrot.gif";
            var card = new ThumbnailCard
            {
                Title = !string.IsNullOrWhiteSpace(title) ? title : "hello there",
                Text = "foobar",
                Images = new List<CardImage> { new CardImage(imgUrl) }
            };

            //return new ComposeExtensionAttachment() { Content = img, ContentUrl = imgUrl, ContentType = "text/html", Name = imgUrl };
            return card
                .ToAttachment()
                .ToComposeExtensionAttachment();
        }

    }
}
