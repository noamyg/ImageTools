# ImageTools
Rest API for image manipulations. Try it out at http://imagetools.noamyg.com.

### Convertion Tools (*route; /api/convert*)

##### PDF To Image

Use a post request to `pdfToJpg` or `pdfToPng` with Content-Type: `application/pdf` to convert a plain text Base64 pdf to a JPG/PNG.

<u>Example:</u>

    $.ajax({
        url: "http://server/api/convert/pdfToJpg", //You can either call pdfToJpg or pdfToPng
        method: "post",
        contentType: "application/pdf",
        data: "JVBERi0xLjUKJYCBgoMKMSAwIG9iago8PC9GaWx0ZXIvRmxhdGVEZWNvZGUvRmlyc3QgMTQxL04gMjAvTGVuZ3=="
    })
    .done(function (data) {
        console.log("done");
    })
    .fail(function (error) {
        console.log("Error occoured: " + error.responseText);
    });



##### One Image Format To Another

Use a post request to `imageToImage`. Set Content-Type as the request content type, and set the required extension by a custom header `Target-Content-Type`.
[See the standard, supported, image mime types](https://mzl.la/2WNMSAg).

Note: for security purposes, sending an equal Content-Type and Target-Content-Type will first convert the image to BMP, then re-convert it to the original type.

<u>Example:</u>

    $.ajax({
        url: "http://server/api/convert/imageToImage",
        method: "post",
        contentType: "image/jpeg",
        headers: { 'Target-Content-Type': "image/gif" },
        data: "JVBERi0xLjUKJYCBgoMKMSAwIG9iago8PC9GaWx0ZXIvRmxhdGVEZWNvZGUvRmlyc3QgMTQxL04gMjAvTGVuZ3=="
    })
    .done(function (data) {
        console.log("done");
    })
    .fail(function (error) {
        console.log("Error occoured: " + error.responseText);
    });

### Image Arrangement Tools (*route; /api/arrange*)

Use a post request to `combineHorizontally`, `combineVertically` or `mosaic` to combine pictures togeter.
Pictures should be posted as Base64, seperated by comma.

<u>Example:</u>

    $.ajax({
        url: "http://server/api/arrange/combineHorizontally",
        method: "post",
        contentType: "image/jpeg",
        data: "JVBERi0xLjUKJYCBgoMKMSAwIG9iago8PC9GaWx0ZXIvRmxhdGVEZWNvZGUvRmlyc3QgMTQxL04gMjAvTGVuZ3==,==Base64String==,==AnotherBase64=="
    })
    .done(function (data) {
        console.log("done");
    })
    .fail(function (error) {
        console.log("Error occoured: " + error.responseText);
    });




### Image Resize (*route; /api/resize*)

Use a post request to the root route.
Body should be Base64 String.
You shoud have at least one header: `Width` or `Height`. If you supply one, the other one will be determined and the scale will be preserved.

<u>Example:</u>

    $.ajax({
        url: "http://server/api/resize/",
        method: "post",
        headers: { 'Width': "300" }
        contentType: "image/jpeg",
        data: "JVBERi0xLjUKJYCBgoMKMSAwIG9iago8PC9GaWx0ZXIvRmxhdGVEZWNvZGUvRmlyc3QgMTQxL04gMjAvTGVuZ3=="
    })
    .done(function (data) {
        console.log("done");
    })
    .fail(function (error) {
        console.log("Error occoured: " + error.responseText);
    });

