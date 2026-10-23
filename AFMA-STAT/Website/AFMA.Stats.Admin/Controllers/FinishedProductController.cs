namespace AFMA.Stats.Admin.Controllers
{
  using ApplicationLayer.Model;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Authorize(Roles = "Admin")]
  [Obsolete("huh?",true)]
  public class FinishedProductController : Controller
  {
    public IActionResult Index()
    {
      List<ProductGroupModel> productGroups = new List<ProductGroupModel>();

      productGroups.Add(new ProductGroupModel
      {
        Name = "Aquaculture Feed",
        Code = "011",
        VisibleToMembers = true,
        Active = true,
        Products = new List<ProductModel>
        {
          new ProductModel
          {
            Code = "095",
            Name = "Aquaculture Feed"
          }
        }
      });

      productGroups.Add(new ProductGroupModel
      {
        Name = "Beef & Sheep Feed",
        Code = "002",
        VisibleToMembers = true,
        Active = true,
        Products = new List<ProductModel>
        {
          new ProductModel
          {
            Code = "042",
            Name = "Cattle Finisher"
          },
          new ProductModel
          {
            Code = "135",
            Name = "Complete Cattle Feed"
          },
          new ProductModel
          {
            Code = "136",
            Name = "Complete Sheep Feed"
          },
          new ProductModel
          {
            Code = "047",
            Name = "Bull Rations"
          },
        }
      });

      productGroups.Add(new ProductGroupModel
      {
        Name = "Breeder Feed",
        Code = "010",
        VisibleToMembers = true,
        Active = true,
        Products = new List<ProductModel>
        {
          new ProductModel
          {
            Code = "076",
            Name = "Chicken Started Mash"
          },
          new ProductModel
          {
            Code = "077",
            Name = "Pullet Developer Mash"
          },
          new ProductModel
          {
            Code = "078",
            Name = "Poultry Growing Mash"
          },
          new ProductModel
          {
            Code = "079",
            Name = "All Mash for Layers"
          },
        }
      });


      return View(productGroups);
    }

    public IActionResult Details(int id)
    {
      ProductGroupModel productGroup = new ProductGroupModel
      {
        Name = "Beef & Sheep Feed",
        Code = "002",
        VisibleToMembers = true,
        HideOnFeedsales = false,
        HideOnSalmonella = false,
        SalmonellaProductGroupCategory = "Dairy, Beef and Sheep",
        Active = true,
        Products = new List<ProductModel>
        {
          new ProductModel
          {
            Code = "042",
            Name = "Cattle Finisher",
            Active = true
          },
          new ProductModel
          {
            Code = "135",
            Name = "Complete Cattle Feed",
            Active = true
          },
          new ProductModel
          {
            Code = "136",
            Name = "Complete Sheep Feed",
            Active = true
          },
          new ProductModel
          {
            Code = "047",
            Name = "Bull Rations",
            Active = true
          },
        }
      };

      return View(productGroup);
    }

    public IActionResult Edit(int id)
    {
      ProductGroupModel productGroup = new ProductGroupModel
      {
        Name = "Beef & Sheep Feed",
        Code = "002",
        VisibleToMembers = true,
        HideOnFeedsales = false,
        HideOnSalmonella = false,
        SalmonellaProductGroupCategory = "Dairy, Beef and Sheep",
        Active = true,
        Products = new List<ProductModel>
        {
          new ProductModel
          {
            Code = "042",
            Name = "Cattle Finisher",
            Active = true
          },
          new ProductModel
          {
            Code = "135",
            Name = "Complete Cattle Feed",
            Active = true
          },
          new ProductModel
          {
            Code = "136",
            Name = "Complete Sheep Feed",
            Active = true
          },
          new ProductModel
          {
            Code = "047",
            Name = "Bull Rations",
            Active = true
          },
        }
      };

      return View(productGroup);

    }
  }
}
