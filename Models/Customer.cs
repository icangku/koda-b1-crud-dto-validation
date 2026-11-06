namespace CrudApi.Models;

public sealed class Customer(string code, string name, string email, string phone, string address)
{
  public string Code {get; init;} = code;
  public string Name {get; init;} = name;
  public string Email {get; init;} = email;
  public string Phone {get; init;} = phone;
  public string Address {get; init;} = address;
}