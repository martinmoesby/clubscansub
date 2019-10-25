using ClubScansub.Models;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ClubScansub.Data
{
    public class DbInitializer : IDbInitializer
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async void Initialize()
        {
            if (db.Database.GetPendingMigrations().Count() > 0)
            {
                db.Database.Migrate();
            }

            if (!roleManager.RoleExistsAsync(Userroles.Owner).Result)
            {
                roleManager.CreateAsync(new IdentityRole(Userroles.Owner)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(Userroles.Administrator)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(Userroles.Divepro)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(Userroles.Member)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(Userroles.User)).GetAwaiter().GetResult();
            }

            var owner = db.Users.FirstOrDefault(x => x.UserName == "admin@site.local");

            if ( owner == null)
            {
                var user = new ApplicationUser
                {
                    Firstname = "Site",
                    Lastname = "Admin",
                    Email = "admin@site.local",
                    UserName = "admin@site.local",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = false
                };

                userManager.CreateAsync(user, "Admin123*").GetAwaiter().GetResult();

                userManager.AddToRoleAsync(user, Userroles.Owner).GetAwaiter().GetResult();
                userManager.AddToRoleAsync(user, Userroles.Administrator).GetAwaiter().GetResult();
                userManager.AddToRoleAsync(user, Userroles.Divepro).GetAwaiter().GetResult();
                userManager.AddToRoleAsync(user, Userroles.Member).GetAwaiter().GetResult();
                userManager.AddToRoleAsync(user, Userroles.User).GetAwaiter().GetResult();


                await db.SaveChangesAsync();
            }

            //seedUsers();
        }


        private void seedUsers()
        {
            var users = new string[]
            {
                "Urs Lancaster"
                , "Duval Snape"
                , "Jervis Lindsay"
                , "Townsend Langdon"
                , "Grischa Knottley"
                , "Ramsey Thackeray"
                , "Cheney Penny"
                , "Aubry Landon"
                , "Mayne Garrick"
                , "Karl Whiteley"
                , "Berdine Stevenson"
                , "Elfie Paddley"
                , "Heide Reeve"
                , "Albertyna Payton"
                , "Odila Snowdon"
                , "Daniella Adlam"
                , "Louanne Blackwood"
                , "Norae Burton"
                , "Isabella Fulton"
                , "Maryam Swale"
                , "Dueath Sparkwood"
                , "Saetdis Boldcrown"
                , "Pethemar Whiteburst"
                , "Saeazhen Firelight"
                , "Lelomir Cinderflare"
                , "Kaenas Eagerstalker"
                , "Erinian Strongveil"
                , "Selron Darkswitch"
                , "Noral Violetrest"
                , "Lenin Richbane"
                , "Zanuzen Sunstalker"
                , "Inetven Phoenixlove"
                , "Yaash Highkind"
                , "Taeath Longrest"
                , "Tymarrin Sparkflame"
                , "Zantheol Azuresmile"
                , "Artheon Highsense"
                , "Perrodan Whitegaze"
                , "Ithiran Violetbirth"
                , "Celoedanis Highreaver"
                , "Novidine Rightstrider"
                , "Ellean Somberburst"
                , "Narel Coldtwist"
                , "Emenna Tindertrail"
                , "Olisalia Sparkdepth"
                , "Cainara Slimshield"
                , "Samisa Grimveil"
                , "Jisia Ancientburn"
                , "Lyraden Goldtrick"
                , "Emedana Lighthide"
                , "Urom Doomblade"
                , "Thegus Firstgem"
                , "Drormolann Fusefall"
                , "Bigarn Frozenshout"
                , "Magnihm Highshaper"
                , "Hugrik Dimgift"
                , "Arginas Hardtoe"
                , "Giliuth Caskkind"
                , "Byndenn Cragshield"
                , "Brognohr Truegrace"
                , "Konmy Blankforce"
                , "Mitass Slowbuster"
                , "Dammegu Vaststone"
                , "Yhmagus Warcave"
                , "Nashi Cragbraid"
                , "Anovio Shorttale"
                , "Gyvinlen Bronzefield"
                , "Azi Moltenhammer"
                , "Uzua Olddust"
                , "Nemdo Stoutcrag"
            };

            foreach (var item in users)
            {
                var names = item.Split(' ');
                var applicationUser = new ApplicationUser()
                {
                    Firstname = names[0],
                    Lastname = names[1],
                    UserName = $"{names[0]}@{names[1]}.tst",
                    Email = $"{names[0]}@{names[1]}.tst"
                };

                if (userManager.FindByEmailAsync(applicationUser.Email).GetAwaiter().GetResult() == null)
                {
                    userManager.CreateAsync(applicationUser, "User123*").GetAwaiter().GetResult();
                    userManager.AddToRoleAsync(applicationUser, Userroles.User).GetAwaiter().GetResult();
                    userManager.AddClaimAsync(applicationUser, new System.Security.Claims.Claim("IsPremiumMember", "false")).GetAwaiter().GetResult();
                }
            }

        }

        //private void seedDB()
        //{

        //    var diveOrgs = db.Diveorganizations.ToList();
        //    if (diveOrgs.Count() == 0)
        //    {
        //        var ssiDO = new Diveorganization
        //        {
        //            Name = "Scuba Schools International",
        //            ShortName = "SSI",
        //            Description = "Providing training, scuba diving certification, and educational resources for divers, dive instructors, dive centers and resorts around the world."
        //        };
        //        var padiDO = new Diveorganization
        //        {
        //            Name = "Professional Association of Diving Instructors.",
        //            ShortName = "PADI",
        //            Description = "PADI is the world’s leading scuba diver training organization."
        //        };
        //        var cmasDO = new Diveorganization
        //        {
        //            Name = "Confédération Mondiale des Activités Subaquatiques",
        //            ShortName = "CMAS",
        //            Description = "An international federation that represents underwater activities in underwater sport and underwater sciences, and oversees an international system of recreational snorkel and scuba diver training and recognition."
        //        };

        //        var ssiCERT1 = new Certificate
        //        {
        //            Name = "Open Water Diver",
        //            ShortName = "OW",

        //        };
        //        var ssiCERT2 = new Certificate
        //        {
        //            Name = "Advanced Open Water Diver",
        //            ShortName = "AOW",

        //        };
        //        var ssiCERT3 = new Certificate
        //        {
        //            Name = "Master Diver",
        //            ShortName = "MD",

        //        };
        //        var ssiCERT4 = new Certificate
        //        {
        //            Name = "Divemaster",
        //            ShortName = "DM",

        //        };

        //        ssiDO.Certificates.Add(ssiCERT1);
        //        ssiDO.Certificates.Add(ssiCERT2);
        //        ssiDO.Certificates.Add(ssiCERT3);
        //        ssiDO.Certificates.Add(ssiCERT4);

        //        var padiCERT1 = new Certificate
        //        {
        //            Name = "Open Water Diver",
        //            ShortName = "OW",

        //        };
        //        var padiCERT2 = new Certificate
        //        {
        //            Name = "Advanced Open Water Diver",
        //            ShortName = "AOW",

        //        };
        //        var padiCERT3 = new Certificate
        //        {
        //            Name = "Master Diver",
        //            ShortName = "MD",

        //        };
        //        var padiCERT4 = new Certificate
        //        {
        //            Name = "Divemaster",
        //            ShortName = "DM",

        //        };

        //        padiDO.Certificates.Add(padiCERT1);
        //        padiDO.Certificates.Add(padiCERT2);
        //        padiDO.Certificates.Add(padiCERT3);
        //        padiDO.Certificates.Add(padiCERT4);

        //        var cmasCERT1 = new Certificate
        //        {
        //            Name = "CMAS 1-star",
        //            ShortName = "CMAS*",

        //        };
        //        var cmasCERT2 = new Certificate
        //        {
        //            Name = "CMAS 2-star",
        //            ShortName = "CMAS**",

        //        };
        //        var cmasCERT3 = new Certificate
        //        {
        //            Name = "CMAS 3-start",
        //            ShortName = "CMAS***",

        //        };

        //        cmasDO.Certificates.Add(cmasCERT1);
        //        cmasDO.Certificates.Add(cmasCERT2);
        //        cmasDO.Certificates.Add(cmasCERT3);

        //        diveOrgs.Add(ssiDO);
        //        diveOrgs.Add(padiDO);
        //        diveOrgs.Add(cmasDO);

        //        db.SaveChanges();
        //    }
        //}
    }
}
