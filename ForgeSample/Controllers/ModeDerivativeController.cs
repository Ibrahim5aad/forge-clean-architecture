using Autodesk.Forge;
using Autodesk.Forge.Model;
using ForgeSample.Application.DTOs;
using ForgeSample.Application.Hubs;
using ForgeSample.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mUtils = ForgeSample.Utils.Utils;


namespace ForgeSample.Controllers
{
    [ApiController]
    public class ModelDerivativeController : ControllerBase
    {
        private IHubContext<ModelDerivativeHub> _hubContext;
        private ModelDerivativeHub _modelDerHub;
        private I2leggedAuthenticationService _authenticationService;


        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDerivativeController"/> class.
        /// </summary>
        /// <param name="hubContext">The hub context.</param>
        public ModelDerivativeController(IHubContext<ModelDerivativeHub> hubContext, 
                                         I2leggedAuthenticationService authenticationService,
                                         ModelDerivativeHub modelDerivativeHub)
        {
            _hubContext = hubContext;
            _modelDerHub = modelDerivativeHub;
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Start the translation job for a give bucketKey/objectName
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/forge/modelderivative/jobs")]
        public async Task<dynamic> TranslateObject([FromBody] ForgeObject objModel)
        {
            Token oauth = await _authenticationService.GetInternalTokenAsync();

            // prepare the webhook callback
            DerivativeWebhooksApi webhook = new DerivativeWebhooksApi();
             
            webhook.Configuration.AccessToken = oauth.access_token;
            dynamic existingHooks = await webhook.GetHooksAsync(DerivativeWebhookEvent.ExtractionFinished);

            // get the callback from your settings (e.g. web.config)
            string callbackUlr = mUtils.GetAppSetting("FORGE_WEBHOOK_URL") + "/api/forge/callback/modelderivative";

            bool createHook = true; // need to create, we don't know if our hook is already there...
            foreach (KeyValuePair<string, dynamic> hook in new DynamicDictionaryItems(existingHooks.data))
            {
                if (hook.Value.scope.workflow.Equals(objModel.connectionId))
                {
                    // ok, found one hook with the same workflow, no need to create...
                    createHook = false;
                    if (!hook.Value.callbackUrl.Equals(callbackUlr))
                    {
                        await webhook.DeleteHookAsync(DerivativeWebhookEvent.ExtractionFinished, new System.Guid(hook.Value.hookId));
                        createHook = true; // ops, the callback URL is outdated, so delete and prepare to create again
                    }
                }
            }

            // need to (re)create the hook?
            if (createHook) await webhook.CreateHookAsync(DerivativeWebhookEvent.ExtractionFinished, callbackUlr, objModel.connectionId);

            // prepare the payload
            List<JobPayloadItem> outputs = new List<JobPayloadItem>()
            {
            new JobPayloadItem(
              JobPayloadItem.TypeEnum.Svf,
              new List<JobPayloadItem.ViewsEnum>()
              {
                JobPayloadItem.ViewsEnum._2d,
                JobPayloadItem.ViewsEnum._3d
              })
            };
            JobPayload job;
            if (string.IsNullOrEmpty(objModel.rootFilename))
                job = new JobPayload(new JobPayloadInput(objModel.objectName), new JobPayloadOutput(outputs), new JobPayloadMisc(objModel.connectionId));
            else
                job = new JobPayload(new JobPayloadInput(objModel.objectName, true, objModel.rootFilename), new JobPayloadOutput(outputs), new JobPayloadMisc(objModel.connectionId));


            // start the translation
            DerivativesApi derivative = new DerivativesApi();
            derivative.Configuration.AccessToken = oauth.access_token;
            try
            {
                dynamic jobPosted = await derivative.TranslateAsync(job, true/* force re-translate if already here, required data:write*/);
            return jobPosted;

            }
            catch (Exception)
            {
                throw;
            } 
        }


        [HttpPost]
        [Route("/api/forge/callback/modelderivative")]
        public async Task<IActionResult> DerivativeCallback([FromBody] JObject body)
        {
            await _modelDerHub.ExtractionFinished(_hubContext, body);
            return Ok();
        }

        [HttpDelete]
        [Route("api/forge/modelderivative/manifest")]
        public async Task<IActionResult> DeleteObjectAsync([FromBody] ForgeObject objectModel)
        {
            Token token = await _authenticationService.GetInternalTokenAsync();
            DerivativesApi derivative = new DerivativesApi();
            derivative.Configuration.AccessToken = token.access_token;
            await derivative.DeleteManifestAsync(objectModel.objectName);
            return Ok();
        }
    }
     
}
