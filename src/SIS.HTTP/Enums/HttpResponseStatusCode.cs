using System.ComponentModel.DataAnnotations;

namespace SIS.HTTP.Enums
{
    public enum HttpResponseStatusCode
    {
        Ok = 200,
        Created = 201,
        [Display(Name = "Permanent redirect")]
        PermanentRedirect = 301,
        Found = 302,
        [Display(Name = "See other")]
        SeeOther = 303,
        [Display(Name = "Temporary redirect")]
        TemporaryRedirect = 307,
        [Display(Name = "Bad request")]
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        [Display(Name = "Not found")]
        NotFound = 404,
        [Display(Name = "Internal server error")]
        InternalServerError = 500
    }
}
