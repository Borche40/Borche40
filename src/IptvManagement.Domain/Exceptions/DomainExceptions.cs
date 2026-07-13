namespace IptvManagement.Domain.Exceptions;
public class DomainException(string message) : Exception(message);
public class NotFoundException(string message) : DomainException(message);
public class ValidationException(string message) : DomainException(message);
public class ConflictException(string message) : DomainException(message);
public class ForbiddenException(string message) : DomainException(message);
public class UnauthorizedException(string message) : DomainException(message);
public class SubscriptionExpiredException(string message) : DomainException(message);
public class DeviceLimitExceededException(string message) : DomainException(message);
