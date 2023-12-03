namespace BffMicrosoftEntraID.Server;

/// <summary>
/// This exception class is used to pass HTTP CAE unauthorized responses from a Httpclient and 
/// return the WWW-Authenticate header with the required claims challenge. 
/// This is only required if using a downstream API
/// </summary>
public class WebApiMsalUiRequiredException(string message, HttpResponseMessage response) : Exception(message)
{
    public HttpStatusCode StatusCode
    {
        get { return response.StatusCode; }
    }

    public HttpResponseHeaders Headers
    {
        get { return response.Headers; }
    }

    public HttpResponseMessage HttpResponseMessage
    {
        get { return response; }
    }
}