namespace AFMA.ApplicationLayer.Model
{
  using System.ComponentModel.DataAnnotations;

  public class LoginModel
  {
    [Required(ErrorMessage = "The Username is required")]
    public string Username { get; set; }

    [Required(ErrorMessage = "The Password is required")]
    public string Password { get; set; }
  }
}
