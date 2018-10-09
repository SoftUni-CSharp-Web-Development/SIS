namespace SIS.HTTP.Enums
{
    public enum HttpResponseStatusCode
    {
        Ok = 200,
        Created = 201,
        Redirect = 301,
        Found = 302,
        SeeOther = 303,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        InternalServerError = 500
    }
}
