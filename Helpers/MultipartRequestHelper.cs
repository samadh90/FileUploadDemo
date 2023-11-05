using Microsoft.Net.Http.Headers;

namespace FileUploadDemo.Helpers;

public static class MultipartRequestHelper
{
    // Gets whether this is a multipart/form-data request
    public static bool IsMultipartContentType(string contentType)
    {
        return !string.IsNullOrEmpty(contentType) && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    // Gets the boundary of the multipart content
    public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
    {
        var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;
        
        if (string.IsNullOrWhiteSpace(boundary)) throw new InvalidDataException("Missing content-type boundary.");

        if (boundary.Length > lengthLimit) throw new InvalidDataException($"Multipart boundary length limit {lengthLimit} exceeded.");

        return boundary;
    }

    // Checks if the content disposition header represents file data
    public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
    {
        // Returns true if the disposition is 'form-data' and a filename is present
        return contentDisposition != null
               && contentDisposition.DispositionType.Equals("form-data", StringComparison.OrdinalIgnoreCase)
               && (!string.IsNullOrEmpty(contentDisposition.FileName.Value) || !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
    }

    // Checks if the content disposition header represents form data
    public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue contentDisposition)
    {
        // Returns true if the disposition is 'form-data' and a filename is not present
        return contentDisposition != null
               && contentDisposition.DispositionType.Equals("form-data", StringComparison.OrdinalIgnoreCase)
               && string.IsNullOrEmpty(contentDisposition.FileName.Value)
               && string.IsNullOrEmpty(contentDisposition.FileNameStar.Value);
    }
}
