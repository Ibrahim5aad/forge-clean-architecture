/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using Autodesk.Forge;
using Autodesk.Forge.Model;
using ForgeSample.Application.DTOs;
using ForgeSample.Utils.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AuthService = ForgeSample.Application.Interfaces.IAuthenticationService;


namespace ForgeSample.Controllers
{

    [ApiController]
    public class OSSController : ControllerBase
    {
        #region Fields

        private IWebHostEnvironment _env;
        private AuthService _authenticationService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OSSController"/> class.
        /// </summary>
        /// <param name="env">The env.</param>
        /// <param name="authenticationService">The authentication service.</param>
        public OSSController(IWebHostEnvironment env, AuthService authenticationService)
        {
            _env = env;
            _authenticationService = authenticationService;
        }

        #endregion


        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public string ClientId { get { return Utils.Utils.GetAppSetting("FORGE_CLIENT_ID").ToLower(); } }


        /// <summary>
        /// Return list of buckets (id=#) or list of objects (id=bucketKey)
        /// </summary>
        [HttpGet]
        [Route("api/forge/oss/buckets")]
        public async Task<IList<TreeNode>> GetOSSAsync(string id)
        {
            IList<TreeNode> nodes = new List<TreeNode>();
            Token oauth = await _authenticationService.GetInternalTokenAsync();

            if (id == "#") // root
            {
                // in this case, let's return all buckets
                BucketsApi appBckets = new BucketsApi();
                appBckets.Configuration.AccessToken = oauth.AccessToken;

                // to simplify, let's return only the first 100 buckets
                dynamic buckets = await appBckets.GetBucketsAsync("US", 100);
                foreach (KeyValuePair<string, dynamic> bucket in new DynamicDictionaryItems(buckets.items))
                {
                    nodes.Add(new TreeNode(bucket.Value.bucketKey, bucket.Value.bucketKey.Replace(ClientId + "-", string.Empty), "bucket", true));
                }
            }
            else
            {
                // as we have the id (bucketKey), let's return all 
                ObjectsApi objects = new ObjectsApi();
                objects.Configuration.AccessToken = oauth.AccessToken;
                var objectsList = await objects.GetObjectsAsync(id, 100);
                foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(objectsList.items))
                {
                    nodes.Add(new TreeNode(((string)objInfo.Value.objectId).Base64Encode(),
                      objInfo.Value.objectKey, "object", false));
                }
            }
            return nodes;
        }


        /// <summary>
        /// Create a new bucket 
        /// </summary>
        [HttpPost]
        [Route("api/forge/oss/buckets")]
        public async Task<dynamic> CreateBucketAsync([FromBody] BucketDTO bucket)
        {
            BucketsApi buckets = new BucketsApi();
            Token token = await _authenticationService.GetInternalTokenAsync();
            buckets.Configuration.AccessToken = token.AccessToken;
            PostBucketsPayload bucketPayload = new PostBucketsPayload(string.Format("{0}-{1}", ClientId, bucket.bucketKey.ToLower()), null, bucket.policyKey);
            return await buckets.CreateBucketAsync(bucketPayload, "US");
        }


        [HttpDelete]
        [Route("api/forge/oss/buckets")]
        public async Task<IActionResult> DeleteBucketAsync([FromBody] BucketDTO bucket)
        {
            BucketsApi buckets = new BucketsApi();
            Token token = await _authenticationService.GetInternalTokenAsync();
            buckets.Configuration.AccessToken = token.AccessToken;
            await buckets.DeleteBucketAsync(bucket.bucketKey);
            return Ok();
        }


        [HttpDelete]
        [Route("api/forge/oss/objects")]
        public async Task<IActionResult> DeleteObjectAsync([FromBody] ForgeObject objectModel)
        {
            ObjectsApi objects = new ObjectsApi();
            Token token = await _authenticationService.GetInternalTokenAsync();
            objects.Configuration.AccessToken = token.AccessToken;
            string objectName = objectModel.objectName.Base64Decode().Split("/")[1];
            await objects.DeleteObjectAsync(objectModel.bucketKey, System.Web.HttpUtility.UrlDecode(objectName));
            return Ok();
        }


        /// <summary>
        /// Receive a file from the client and upload to the bucket
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/forge/oss/objects")]
        public async Task<dynamic> UploadObjectAsyc([FromForm] UploadFileDTO input)
        {

            // save the file on the server
            var fileSavePath = Path.Combine(_env.ContentRootPath, input.fileToUpload.FileName);

            using (var stream = new FileStream(fileSavePath, FileMode.Create))
            {
                await input.fileToUpload.CopyToAsync(stream);
            }

            // get the bucket...
            Token oauth = await _authenticationService.GetInternalTokenAsync();
            ObjectsApi objects = new ObjectsApi();
            objects.Configuration.AccessToken = oauth.AccessToken;

            // upload the file/object, which will create a new object
            dynamic uploadedObj;
            using (StreamReader streamReader = new StreamReader(fileSavePath))
            {
                uploadedObj = await objects.UploadObjectAsync(input.bucketKey,
                       input.fileToUpload.FileName, (int)streamReader.BaseStream.Length, streamReader.BaseStream,
                       "application/octet-stream");
            }

            // cleanup
            System.IO.File.Delete(fileSavePath);

            return uploadedObj;
        }

    }
}