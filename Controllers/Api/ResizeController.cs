using ImageMagick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ImageTools.Controllers.Api
{
    [Route("api/resize")]
    public class ResizeController : ApiController
    {

        [HttpPost]
        public async Task<HttpResponseMessage> Resize()
        {
            HttpResponseMessage response;
            var headers = Request.Headers;
            var base64FromRequest = await Request.Content.ReadAsStringAsync();
            if (String.IsNullOrEmpty(base64FromRequest) || base64FromRequest.Length == 0)
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.Content = new StringContent("Base 64 input cannot be empty");
            }
            else
            {
                int height = 0;
                int width = 0;
                IEnumerable<string> headerHeightValue;
                if (headers.TryGetValues("Height", out headerHeightValue))
                {
                    Int32.TryParse(headerHeightValue.FirstOrDefault(), out height);
                }
                IEnumerable<string> headerWidthValue;
                if (headers.TryGetValues("Width", out headerWidthValue))
                {
                    Int32.TryParse(headerWidthValue.FirstOrDefault(), out width);
                }
                if (height == 0 && width == 0)
                {
                    response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    response.Content = new StringContent("You must supply at least one of the following headers: Height, Width");
                }
                else try
                {
                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(string.Format("data:image/{0};base64,{1}", "jpeg", this.ResizeBase64Image(height, width, base64FromRequest)));
                }
                catch (Exception e)
                {
                    if (e is MagickMissingDelegateErrorException || e is FormatException)
                    {
                        response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        response.Content = new StringContent("Not a valid Base64 Image input");
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



        private string ResizeBase64Image(int height, int width, string base64FromRequest)
        {
            var oldByteArrImage = Convert.FromBase64String(base64FromRequest);
            byte[] newByteArrImage;
            using (IMagickImage image = new MagickImage(oldByteArrImage))
            {
                MagickGeometry size = new MagickGeometry(width, height);
                if (height > 0 && width > 0)
                {
                    size.IgnoreAspectRatio = true;
                }
                image.Resize(size);
                newByteArrImage = image.ToByteArray();
            }
            return Convert.ToBase64String(newByteArrImage);
        }

    }
}
