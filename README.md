# ImageTools
Rest API for image manipulations.

### Convert
Use a post request to `pdfToJpg` or `pdfToPng` with `content-type: application/pdf` to convert a plain text Base64 pdf to a JPG/PNG.

<u>Example:</u>

    $.ajax({
        url: "http://server/api/convert/pdfToJpg",
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