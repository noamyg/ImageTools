using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ImageMagick;
using System.Threading.Tasks;
using ImageTools.Models;
using System.Collections.Generic;

namespace ImageTools.Controllers.Api
{
    public class ConvertController : ApiController
    {

        [HttpPost]
        [Route("api/convert/pdfToPng")]
        [Route("api/convert/pdfToJpg")]
        public async Task<HttpResponseMessage> PdfToImage()
        {
            HttpResponseMessage response;
            var base64FromRequest = await Request.Content.ReadAsStringAsync();
            if (String.IsNullOrEmpty(base64FromRequest) || base64FromRequest.Length == 0)
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.Content = new StringContent("Base 64 input cannot be empty");
            }
            else try
                {
                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    var route = Request.RequestUri.Segments.Last();
                    if (route == "pdfToPng")
                    {
                        response.Content = new StringContent(string.Format("data:{0};base64,{1}", "png", ConvertBase64PdfToBase64Image(MagickFormat.Png, base64FromRequest)));
                    }
                    else if (route == "pdfToJpg")
                    {
                        response.Content = new StringContent(string.Format("data:{0};base64,{1}", "jpeg", ConvertBase64PdfToBase64Image(MagickFormat.Jpg, base64FromRequest)));
                    }

                }
                catch (Exception e)
                {
                    if (e is MagickMissingDelegateErrorException || e is FormatException)
                    {
                        response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        response.Content = new StringContent("Not a valid PDF Base64 input");
                    }
                    else
                    {
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Server was unable to parse the input");
                    }
                }
            return response;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ImageToImage()
        {
            HttpResponseMessage response;
            var headers = Request.Headers;
            var sourceContentType = Request.Content.Headers.ContentType?.MediaType;
            var targetContentType = String.Empty;
            IEnumerable<string> targetContentTypeHeader;
            if (headers.TryGetValues("Target-Content-Type", out targetContentTypeHeader))
            {
                targetContentType = targetContentTypeHeader.FirstOrDefault();
            }
            var base64ImageFromRequest = await Request.Content.ReadAsStringAsync();
            if (base64ImageFromRequest == null || base64ImageFromRequest.Length == 0)
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.Content = new StringContent("You must supply an \"image\" parameter as Base64 string");
            }
            else if (sourceContentType == null || !MagickTools.ContentTypeToFormat.ContainsKey(sourceContentType))
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.Content = new StringContent("You must supply a \"Content-Type\" header with a standard MIME image type (<b><a href=\"https://mzl.la/2WNMSAg\" >See here</a></b>)");

            }
            else if (targetContentType == null || !MagickTools.ContentTypeToFormat.ContainsKey(targetContentType))
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.Content = new StringContent("You must supply a \"Target-Content-Type\" header with a standard MIME image type (<b><a href=\"https://mzl.la/2WNMSAg\" >See here</a></b>)");

            }
            else
            {
                try
                {
                    String convertedBase64Image = convertBase64ImageToBase64Image(MagickTools.ContentTypeToFormat[sourceContentType], MagickTools.ContentTypeToFormat[targetContentType], base64ImageFromRequest);
                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(string.Format("data:{0};base64,{1}", MagickTools.ContentTypeToFormat.FirstOrDefault(x => x.Value == MagickTools.ContentTypeToFormat[targetContentType]).Key, convertedBase64Image));
                }
                catch (Exception e)
                {
                    if (e is MagickMissingDelegateErrorException || e is FormatException)
                    {
                        response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        response.Content = new StringContent("Not a valid PDF Base64 input");
                    }
                    else
                    {
                        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        response.Content = new StringContent("Server was unable to parse the input");
                    }
                }
            }
            return response;
        }


        private string ConvertBase64PdfToBase64Image(MagickFormat format, string base64FromRequest)
        {
            byte[] byteArrImage;
            using (MagickImageCollection images = new MagickImageCollection())
            {
                // Add all the pages of the pdf file to the collection
                byte[] pdfBytes = Convert.FromBase64String(base64FromRequest);
                images.Read(pdfBytes, MagickTools.ReadSettings);
                //Reverse images for RTL languages (When using AppendHorizontally)
                //images.Reverse();
                using (IMagickImage vertical = images.AppendVertically())
                {
                    //For non-transparent image types - recolor Transparent as White
                    if (!MagickTools.TransparencySupportedFormats.Contains(format))
                    {
                        vertical.Opaque(MagickColors.Transparent, MagickColors.White);
                    }
                    vertical.Format = format;
                    byteArrImage = vertical.ToByteArray();

                }
            }
            return Convert.ToBase64String(byteArrImage);
        }

        private string convertBase64ImageToBase64Image(MagickFormat sourceFormat, MagickFormat targetFormat, string base64)
        {
            byte[] newByteArrImage;
            byte[] oldByteArrImage;
            //For security reasons - if source and target are the same, make dummy conversion first
            if (sourceFormat.Equals(targetFormat))
            {
                oldByteArrImage = Convert.FromBase64String(convertBase64ImageToBase64Image(sourceFormat, MagickFormat.Bmp, base64));
            }
            else oldByteArrImage = Convert.FromBase64String(base64);
            using (IMagickImage image = new MagickImage(oldByteArrImage))
            {
                image.Format = targetFormat;
                newByteArrImage = image.ToByteArray();
            }
            return Convert.ToBase64String(newByteArrImage);
        }
    }
}