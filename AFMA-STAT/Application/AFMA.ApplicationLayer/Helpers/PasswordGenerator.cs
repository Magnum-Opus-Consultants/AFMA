namespace AFMA.ApplicationLayer.Helpers
{
  using System;
  using System.Text;

  public static class PasswordGenerator
  {
    public static string CreatePassword(int length = 10)
    {
      const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
      var res = new StringBuilder();
      var rnd = new Random();
      while (0 < length--)
      {
        res.Append(valid[rnd.Next(valid.Length)]);
      }
      return res.ToString();
    }

    public static string GeneratePassword(bool includeLowercase, bool includeUppercase, bool includeNumeric, bool includeSpecial, bool includeSpaces, int lengthOfPassword)
    {
      if (lengthOfPassword < 8 || lengthOfPassword > 128)
      {
        return "Password length must be between 8 and 128.";
      }

      string text = "";
      if (includeLowercase)
      {
        text += "abcdefghijklmnopqrstuvwxyz";
      }

      if (includeUppercase)
      {
        text += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      }

      if (includeNumeric)
      {
        text += "0123456789";
      }

      if (includeSpecial)
      {
        text += "!#$%&*@\\";
      }

      if (includeSpaces)
      {
        text += " ";
      }

      char[] array = new char[lengthOfPassword];
      int length = text.Length;
      Random random = new Random();
      for (int i = 0; i < lengthOfPassword; i++)
      {
        array[i] = text[random.Next(length - 1)];
        if (i > 2 && array[i] == array[i - 1] && array[i - 1] == array[i - 2])
        {
          i--;
        }
      }

      return string.Join(null, array);
    }
  }
}
