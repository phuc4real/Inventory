namespace Inventory.Core.Enums
{
    public enum ResponseCode
    {
        Success = 200,
        Created = 201,
        NoContent = 204,
        BadRequest = 400,
        Forbidden = 403,
        NotFound = 404,
        Conflict = 409,
        UnprocessableContent = 422
    }
}
