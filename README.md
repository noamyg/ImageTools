# ImageTools
Rest API for image manipulations.

### Convertion Tools

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

