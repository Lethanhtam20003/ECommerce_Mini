namespace AuthService.Infrastructure.Exceptions
{
    public class ExceptionCustoms
    {
        // 404 Not Found: The requested resource was not found.`    
        public class NotFoundException(string message) : Exception(message);
        // 400 Bad Request: The request was invalid or cannot be processed.
        public class BadRequestException(string message) : Exception(message);
        // 401 Unauthorized: Authentication is required and has failed or has not been provided.
        public class UnauthorizedException(string message) : Exception(message);
        // 403 Forbidden: The request was valid, but the server is refusing action.
        public class ForbiddenException(string message) : Exception(message);
        // 409 Conflict: The request could not be completed due to a conflict with the current state of the resource.
        public class ConflictException(string message) : Exception(message);
        /**
        // 500 Internal Server Error: An unexpected error occurred on the server.
         */
        public class InternalServerErrorException(string message) : Exception(message);
    }
}
