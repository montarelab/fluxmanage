namespace Common.Exceptions;

public class ConcurrencyException : Exception;

public class AggregateNotFoundException(string message) : Exception(message);
