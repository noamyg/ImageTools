using ImageMagick;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ImageTools.Controllers.Api
{
    public class ArrangeController : ApiController
    {
        [HttpPost]
        [Route("api/arrange/combineVertically")]
        [Route("api/arrange/combineHorizontally")]
        [Route("api/arrange/mosaic")]
        public async Task<HttpResponseMessage> combineImages()
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
                    response.Content = new StringContent(string.Format("data:image/{0};base64,{1}", "jpeg", combineBase64ImagesToOne(route, base64FromRequest)));
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
            return response;
        }



        private string combineBase64ImagesToOne(string route, string base64FromRequest)
        {
            byte[] byteArrImage;
            using (MagickImageCollection images = new MagickImageCollection())
            {
                // Add all the pages of the pdf file to the collection
                foreach (var singleBase64String in base64FromRequest.Split(','))
                {
                    var singleImageByteArray = Convert.FromBase64String(singleBase64String);
                    MagickImage newImage = new MagickImage(singleImageByteArray);
                    images.Add(newImage);
                }
                Func<string, IMagickImage> arrangementMethod = (r) => {
                    switch (route)
                    {
                        case "mosaic":
                            return images.Mosaic();
                        case "combineHorizontally":
                            return images.AppendHorizontally();
                        default:
                            return images.AppendVertically();
                        }
                };
                using (IMagickImage result = arrangementMethod(route))
                {
                    byteArrImage = result.ToByteArray();
                }
            }
            return Convert.ToBase64String(byteArrImage);
        }
    }
}
