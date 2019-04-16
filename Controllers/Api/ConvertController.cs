using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using ImageMagick;
using System.IO;
using System.Threading.Tasks;
using ImageTools.Models;

namespace ImageTools.Controllers.Api
{
    public class ConvertController : ApiController
    {
        
        [HttpPost]
        [Route("api/convert/pdfToPng")]
        [Route("api/convert/pdfToJpg")]
        public async Task<HttpResponseMessage> PdfToPng()
        {
            HttpResponseMessage response;
            var base64FromRequest = await Request.Content.ReadAsStringAsync();
            if (base64FromRequest == null || base64FromRequest.Length == 0)
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
                    response.Content = new StringContent(string.Format("data:image/{0};base64,{1}", "png", Base64PdfToImage(MagickFormat.Png, base64FromRequest)));
                }
                else if (route == "pdfToJpg")
                {
                    response.Content = new StringContent(string.Format("data:image/{0};base64,{1}", "jpeg", Base64PdfToImage(MagickFormat.Jpg, base64FromRequest)));
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
        

        private string Base64PdfToImage(MagickFormat format, string base64FromRequest)
        {
            byte[] byteArrImage;
            using (MagickImageCollection images = new MagickImageCollection())
            {
                // Add all the pages of the pdf file to the collection
                string pdfContent = base64FromRequest;
                byte[] pdfBytes = Convert.FromBase64String(pdfContent);
                images.Read(pdfBytes, MagickTools.ReadSettings);
                //Reverse images for RTL languages
                images.Reverse();
                using (IMagickImage horizontal = images.AppendHorizontally())
                {
                    //For non-transparent image types - recolor Transparent as White
                    if (!MagickTools.TransparencySupportedFormats.Contains(format))
                    {
                        horizontal.Opaque(MagickColors.Transparent, MagickColors.White);
                    }
                    horizontal.Format = format;
                    byteArrImage = horizontal.ToByteArray();

                }
            }
            return Convert.ToBase64String(byteArrImage);
        }
    }
}