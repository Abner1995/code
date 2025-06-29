namespace Todo.Core.Exceptions;

public class FoundException(string resourceType, string resourceIdentifier) : Exception($"{resourceType} with : {resourceIdentifier} exist")
{

}
