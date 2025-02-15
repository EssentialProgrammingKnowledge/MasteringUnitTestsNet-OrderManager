namespace OrderManager.API.DTO
{
    public record CustomerDTO(
        int Id,
        string FirstName,
        string LastName,
        string Email
    );
}
