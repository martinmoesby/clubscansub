using ClubScansub.Models;
using ClubScansub.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
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

            if (!roleManager.RoleExistsAsync(Userroles.Student).Result)
            {
                roleManager.CreateAsync(new IdentityRole(Userroles.Student)).GetAwaiter().GetResult();
            }

            var owner = db.Users.FirstOrDefault(x => x.UserName == "admin@scansub.dk");

            if (owner == null)
            {
                var user = new ApplicationUser
                {
                    Firstname = "Site",
                    Lastname = "Admin",
                    Email = "admin@site.local",
                    UserName = "admin@scansub.dk",
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

            var clubInfo = db.ClubSettings.SingleOrDefault();

            if (clubInfo == null)
            {
                clubInfo = new ClubSettings();
                clubInfo.Name = "Min Dykkerklub";
                clubInfo.Tagline = "gode oplevelser under overfladen - og over";
                clubInfo.Address = new Address();
                clubInfo.Address.Name = "Klubben";
                clubInfo.IsOldMemberDatabaseImported = false;

                db.ClubSettings.Add(clubInfo);
                await db.SaveChangesAsync();

            }

            if (!clubInfo.IsOldMemberDatabaseImported)
            {
                //importUsers();
                importKursister();
                clubInfo.IsOldMemberDatabaseImported = true;
                await db.SaveChangesAsync();
            }
        }

        private void importUsers()
        {

            var users = db.medlemsdata.AsNoTracking().ToList();

            foreach (var item in users)
            {
                //var names = item.Split(' ');
                var applicationUser = new ApplicationUser()
                {
                    Firstname = item.fornavn,
                    Lastname = item.efternavn,
                    Streetaddress = item.adresse,
                    PhoneNumber = item.telefonBil,
                    Email = item.eMail,
                    UserName = $"{item.dsfnr}@scansub.dk",
                    AccountNumber = item.dsfnr
                };

                if (userManager.FindByNameAsync(applicationUser.UserName).GetAwaiter().GetResult() == null)
                {
                    var createUserResult = userManager.CreateAsync(applicationUser, item.password).GetAwaiter().GetResult();
                    if (createUserResult.Succeeded)
                    {
                        if (string.IsNullOrEmpty(applicationUser.SecurityStamp))
                            applicationUser.SecurityStamp = System.Guid.NewGuid().ToString();

                        userManager.AddToRoleAsync(applicationUser, Userroles.User).GetAwaiter().GetResult();
                        userManager.AddClaimAsync(applicationUser, new System.Security.Claims.Claim("IsPremiumMember", item.status ? "true" : "false")).GetAwaiter().GetResult();

                        if (item.status)
                            userManager.AddToRoleAsync(applicationUser, Userroles.Member).GetAwaiter().GetResult();

                        db.ApplicationUserAccountEntry.Add(new ApplicationUserAccountEntry()
                        {
                            AccountType = AccountTypeEnum.EventAccountType,
                            Description = "Saldotransport fra gammelt system",
                            Amount = item.saldo,
                            PostingDate = DateTime.Now,
                            ApplicationUser = applicationUser,
                        });

                        applicationUser.OldAccountImported = true;

                        db.SaveChangesAsync().GetAwaiter().GetResult();
                    }
                }
                else
                {
                    //User already imported - only import new accountentries
                    var user = db.ApplicationUsers.FirstOrDefault(x=>x.UserName == applicationUser.UserName);

                    if (user != null)
                    {
                        var openingEntry = db.ApplicationUserAccountEntry.Any(x => x.ApplicationUser == user && x.Description == "Saldotransport fra gammelt system" && x.AccountType == AccountTypeEnum.EventAccountType);
                        if (!openingEntry)
                        {
                            var accountEntries = db.ApplicationUserAccountEntry.Where(x => x.ApplicationUser == user && x.AccountType == AccountTypeEnum.EventAccountType);
                            
                            if (accountEntries != null || accountEntries.Count() > 0) 
                                db.ApplicationUserAccountEntry.RemoveRange(accountEntries);


                            db.ApplicationUserAccountEntry.Add(new ApplicationUserAccountEntry()
                            {
                                AccountType = AccountTypeEnum.EventAccountType,
                                Description = "Saldotransport fra gammelt system",
                                Amount = item.saldo,
                                PostingDate = DateTime.Now,
                                ApplicationUser = user,
                            });

                            db.SaveChangesAsync().GetAwaiter().GetResult();
                        }
                    }
                }

            }

        }

        private void importKursister()
        {

            var users = db.kursistdata.AsNoTracking().ToList();

            foreach (var item in users)
            {
                //var names = item.Split(' ');
                var applicationUser = new ApplicationUser()
                {
                    Firstname = item.fornavn,
                    Lastname = item.efternavn,
                    Streetaddress = item.adresse,
                    PhoneNumber = item.telefonBil,
                    Email = item.eMail,
                    UserName = $"{item.dsfnr}@scansub.dk",
                    AccountNumber = item.dsfnr
                };

                if (userManager.FindByNameAsync(applicationUser.UserName).GetAwaiter().GetResult() == null)
                {
                    var createUserResult = userManager.CreateAsync(applicationUser, item.password).GetAwaiter().GetResult();
                    if (createUserResult.Succeeded)
                    {
                        if (string.IsNullOrEmpty(applicationUser.SecurityStamp))
                            applicationUser.SecurityStamp = System.Guid.NewGuid().ToString();

                        userManager.AddToRoleAsync(applicationUser, Userroles.Student).GetAwaiter().GetResult();
                        userManager.AddClaimAsync(applicationUser, new System.Security.Claims.Claim("IsPremiumMember", "false")).GetAwaiter().GetResult();


                        db.ApplicationUserAccountEntry.Add(new ApplicationUserAccountEntry()
                        {
                            AccountType = AccountTypeEnum.CourseAccountType,
                            Description = "Kursussaldo fra gammelt system",
                            Amount = item.saldo,
                            PostingDate = DateTime.Now,
                            ApplicationUser = applicationUser,
                        });

                        applicationUser.OldAccountImported = true;

                        db.SaveChangesAsync().GetAwaiter().GetResult();
                    }
                }
                else
                {
                    //User already imported - only import new accountentries
                    var user = db.ApplicationUsers.FirstOrDefault(x => x.UserName == applicationUser.UserName);

                    if (user != null)
                    {
                        userManager.AddToRoleAsync(user, Userroles.Student).GetAwaiter().GetResult();

                        var openingEntry = db.ApplicationUserAccountEntry.Any(x => x.ApplicationUser == user && x.Description == "Kursussaldo fra gammelt system" && x.AccountType == AccountTypeEnum.CourseAccountType);
                        if (!openingEntry)
                        {
                            var accountEntries = db.ApplicationUserAccountEntry.Where(x => x.ApplicationUser == user && x.AccountType == AccountTypeEnum.CourseAccountType);

                            if (accountEntries != null || accountEntries.Count() > 0)
                                db.ApplicationUserAccountEntry.RemoveRange(accountEntries);


                            db.ApplicationUserAccountEntry.Add(new ApplicationUserAccountEntry()
                            {
                                AccountType = AccountTypeEnum.CourseAccountType,
                                Description = "Kursussaldo fra gammelt system",
                                Amount = item.saldo,
                                PostingDate = DateTime.Now,
                                ApplicationUser = user,
                            });

                            db.SaveChangesAsync().GetAwaiter().GetResult();
                        }
                    }
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


//var users = new string[]
//{
//    "Urs Lancaster"
//    , "Duval Snape"
//    , "Jervis Lindsay"
//    , "Townsend Langdon"
//    , "Grischa Knottley"
//    , "Ramsey Thackeray"
//    , "Cheney Penny"
//    , "Aubry Landon"
//    , "Mayne Garrick"
//    , "Karl Whiteley"
//    , "Berdine Stevenson"
//    , "Elfie Paddley"
//    , "Heide Reeve"
//    , "Albertyna Payton"
//    , "Odila Snowdon"
//    , "Daniella Adlam"
//    , "Louanne Blackwood"
//    , "Norae Burton"
//    , "Isabella Fulton"
//    , "Maryam Swale"
//    , "Dueath Sparkwood"
//    , "Saetdis Boldcrown"
//    , "Pethemar Whiteburst"
//    , "Saeazhen Firelight"
//    , "Lelomir Cinderflare"
//    , "Kaenas Eagerstalker"
//    , "Erinian Strongveil"
//    , "Selron Darkswitch"
//    , "Noral Violetrest"
//    , "Lenin Richbane"
//    , "Zanuzen Sunstalker"
//    , "Inetven Phoenixlove"
//    , "Yaash Highkind"
//    , "Taeath Longrest"
//    , "Tymarrin Sparkflame"
//    , "Zantheol Azuresmile"
//    , "Artheon Highsense"
//    , "Perrodan Whitegaze"
//    , "Ithiran Violetbirth"
//    , "Celoedanis Highreaver"
//    , "Novidine Rightstrider"
//    , "Ellean Somberburst"
//    , "Narel Coldtwist"
//    , "Emenna Tindertrail"
//    , "Olisalia Sparkdepth"
//    , "Cainara Slimshield"
//    , "Samisa Grimveil"
//    , "Jisia Ancientburn"
//    , "Lyraden Goldtrick"
//    , "Emedana Lighthide"
//    , "Urom Doomblade"
//    , "Thegus Firstgem"
//    , "Drormolann Fusefall"
//    , "Bigarn Frozenshout"
//    , "Magnihm Highshaper"
//    , "Hugrik Dimgift"
//    , "Arginas Hardtoe"
//    , "Giliuth Caskkind"
//    , "Byndenn Cragshield"
//    , "Brognohr Truegrace"
//    , "Konmy Blankforce"
//    , "Mitass Slowbuster"
//    , "Dammegu Vaststone"
//    , "Yhmagus Warcave"
//    , "Nashi Cragbraid"
//    , "Anovio Shorttale"
//    , "Gyvinlen Bronzefield"
//    , "Azi Moltenhammer"
//    , "Uzua Olddust"
//    , "Nemdo Stoutcrag"
//};