namespace AFMA.ApplicationLayer.ViewModels.Website
{
  /// <summary>
  /// The Main Menu Model class
  /// </summary>
  public class MainMenuViewModel
  {
    /// <summary>
    /// Gets or sets the Id of the Main Menu
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Menu Text
    /// </summary>
    public string MenuText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Friendly Url
    /// </summary>
    public string FriendlyUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets checked indicator
    /// </summary>
    public bool IsChecked { get; set; }

    /// <summary>
    /// Gets or sets the Font Awesome Icon class name
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the dropdown indicator
    /// </summary>
    public bool IsDropdown => MenuItems.Any();

    /// <summary>
    /// Gets or sets the collection of Sub Menu Items
    /// </summary>
    public ICollection<SubMenuViewModel> MenuItems { get; set; } = new List<SubMenuViewModel>();
  }

  /// <summary>
  /// The Sub Menu Model class
  /// </summary>
  public class SubMenuViewModel
  {
    /// <summary>
    /// Gets or sets the Id of the Sub Menu
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Menu Text
    /// </summary>
    public string MenuText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Friendly Url
    /// </summary>
    public string FriendlyUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets checked indicator
    /// </summary>
    public bool IsChecked { get; set; }

    /// <summary>
    /// Gets or sets the sub items check indicator
    /// </summary>
    public bool HasSubItems { get; set; }

    /// <summary>
    /// Gets or sets the collection of Sub Menu Items
    /// </summary>
    public ICollection<SubMenuViewModel> MenuItems { get; set; } = new List<SubMenuViewModel>();
  }
}
