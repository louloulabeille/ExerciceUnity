using ComptoirAnglaisEntities;
using ExerciceUnity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExerciceUnity
{
    class Program
    {
        static void Main(string[] args)
        {
            //AfficheClientCommence("s");
            //AfficheFirstClientCommence("s");
            //AfficheFirstClientCommenceOrder("s");
            //ListeEmployeCommande(2009,"USA");
            //ListeEmployeCommandeGroupeJoin(2009, "USA");
            //ListeCategorieProduitRupture();
            //ListeAllClientAllCommande();
            //ListeClientNoCommand();
            //ListeClientNoCommandDate(2010);
            //ListeProduitCategorieNoRupture();
            //ChiffreAffaireClient("France");
            ClientMoreRemise();
        }

        internal static void AfficheClientCommence (string name)
        {
            using (ComptoirAnglais_V1Context dbcontext = new ComptoirAnglais_V1Context())
            {
                var client = dbcontext.Customers.Where(x => x.CompanyName.StartsWith(name.ToUpper()));
                foreach(Customer item in client )
                {
                    Console.WriteLine(item.CompanyName+" "+item.City);
                }
            }
        }

        internal static void AfficheFirstClientCommence(string name)
        {
            using (ComptoirAnglais_V1Context dbcontext = new ComptoirAnglais_V1Context())
            {
                var client = dbcontext.Customers.Where(x => x.CompanyName.StartsWith(name.ToUpper())).First();
                Console.WriteLine(client.CompanyName + " " + client.City);
            }
        }

        internal static void AfficheFirstClientCommenceOrder(string name)
        {
            using (ComptoirAnglais_V1Context dbcontext = new ComptoirAnglais_V1Context())
            {
                var client = dbcontext.Customers.Where(x => x.CompanyName.StartsWith(name.ToUpper())).OrderBy(y => y.CompanyName).First();
                Console.WriteLine(client.CompanyName + " " + client.City);
            }
        }

        internal static void ListeEmployeCommande(int year, string country)
        {
            using (ComptoirAnglais_V1Context dbcontext = new ComptoirAnglais_V1Context())
            {
                var employeesOrders = dbcontext.Employees.Where(x=>x.Country.Contains(country.ToUpper()))
                    .Join(dbcontext.Orders, employe => employe.EmployeeId, order => order.EmployeeId, (employe, order) => new {name=employe.FirstName,idOrder=order.OrderId,date=order.OrderDate})
                    .Where(y=>y.date.Value.Year==year);

                foreach (var item in employeesOrders)
                {
                    Console.WriteLine(item.name + " " + item.idOrder+" "+item.date);
                }
            }
        }

        internal static void ListeEmployeCommandeGroupeJoin(int yearSearch, string country)
        {
            using (ComptoirAnglais_V1Context dbcontext = new ComptoirAnglais_V1Context())
            {
                /*var employees = dbcontext.Employees.Where(x => x.Country.Contains(country.ToUpper()))
                    .GroupJoin(dbcontext.Orders, employe => employe.EmployeeId, order => order.EmployeeId, (employe, order) => new { orders = order
                    .Where(y => y.OrderDate.Value.Year == yearSearch), empl = employe });

                foreach(var item in employees)
                {
                    Console.WriteLine(item.empl.Country+" "+item.empl.City+" "+item.empl.FirstName );
                    foreach(var ord in item.orders)
                    {
                        Console.WriteLine(ord.EmployeeId + " " + ord.OrderId + " " + ord.OrderDate);
                    }
                }*/

                var employees = dbcontext.Employees.Where(y => y.Country.Contains(country.ToUpper())).Include(x=>x.Orders.Where(y => y.OrderDate.Value.Year == yearSearch));
                foreach (var item in employees)
                {
                    Console.WriteLine(item.Country + " " + item.City + " " + item.FirstName+" "+item.LastName);
                    foreach (var ord in item.Orders)
                    {
                        Console.WriteLine(ord.EmployeeId + " " + ord.OrderId + " " + ord.OrderDate);
                    }
                }

            }
        }

        internal static void ListeCategorieProduitRupture()
        {
            using (ComptoirAnglais_V1Context dbcontext = new ComptoirAnglais_V1Context())
            {
                var query = dbcontext.Categories.Join(dbcontext.Products.Where(x=>x.Discontinued == true), cat => cat.CategoryId, prod => prod.CategoryId, (cat, prod) => new { catId = cat.CategoryId,catName=cat.CategoryName }).Distinct();
                foreach (var item in query)
                {
                    Console.WriteLine(item.catId+" "+item.catName);
                }
            }
        }

        internal static void ListeAllClientAllCommande()
        {
            using (ComptoirAnglais_V1Context dbcontext = new ComptoirAnglais_V1Context())
            {
                var query = dbcontext.Customers.ToList().GroupJoin(dbcontext.Orders, c => c.CustomerId, o => o.CustomerId, (c, o) => new { customer = c, nb = o.Count() });
                foreach(var item in query )
                {
                    Console.WriteLine(item.customer.CompanyName + " : " + item.nb);
                }

                var query2 = dbcontext.Customers.Include(x => x.Orders).Select(s =>new { custom = s, nb =s.Orders.Count()});
                foreach (var item in query2)
                {
                    Console.WriteLine(item.custom.CompanyName + " nombre de commande : " + item.nb);
                }

                var query3 = dbcontext.Customers.Join(dbcontext.Orders, c => c.CustomerId, o => o.CustomerId, (o, c) => new {c.Customer.CompanyName })
                    .GroupBy(x=>x.CompanyName,(name,nombre)=> new {key=name,count=nombre.Count()}).OrderBy(t=>t.key);
                foreach (var item in query3 )
                {
                    Console.WriteLine(item.key+" : "+item.count);
                }
            }
        }

        internal static void ListeClientNoCommand()
        {
            using (ComptoirAnglais_V1Context dbcontext = new ComptoirAnglais_V1Context())
            {
                var query = from customer in dbcontext.Customers
                            join order in
                            dbcontext.Orders on customer.CustomerId equals order.CustomerId into orderss
                            from m in orderss.DefaultIfEmpty()
                            select new
                            {
                                client = customer,
                                idorder = (int?)m.OrderId
                            };
                //query.Where(x=>x.idorder == null).ToList();

                foreach(var item in query.Where(x => x.idorder == null).ToList())
                {
                    Console.WriteLine(item.client.CompanyName + " " + item.client.Country);
                }

                var query2 = dbcontext.Customers.Include(t=>t.Orders).Where(x=>x.Orders.Count == 0);
                foreach (var item in query2)
                {
                    Console.WriteLine(item.CompanyName + " " + item.Country);
                }
            }
        }

        internal static void ListeClientNoCommandDate(int year)
        {
            using (ComptoirAnglais_V1Context dbcontext = new ComptoirAnglais_V1Context())
            {

                var query = dbcontext.Customers.Where(t => t.Orders.Max(u => u.OrderDate).Value.Year <= year);

                foreach (var item in query)
                {
                    Console.WriteLine(item.CompanyName+" "+item.Country);
                }
            }
        }

        internal static void ListeProduitCategorieNoRupture ()
        {
            using (ComptoirAnglais_V1Context dbcontext = new ComptoirAnglais_V1Context())
            {
                var query = dbcontext.Categories.Where(x => x.Products.All(x => x.Discontinued==false));
                foreach(var item in query)
                {
                    Console.WriteLine(item.CategoryName);
                }
            }
        }

        internal static void ChiffreAffaireClient(string country)
        {
            using (ComptoirAnglais_V1Context dbcontext = new ComptoirAnglais_V1Context())
            {
                //var query = dbcontext.Customers.Where(x => x.Country.Contains(country))//.Include(x => x.Orders).ThenInclude(x => x.OrderDetails)
                //    .Select(x => new
                //    {
                //        nom = x.CompanyName,
                //        chiffre = x.Orders.Select(y => y.OrderDetails.Sum(y => y.Quantity * (y.UnitPrice * (y.Discount == 0 ? (decimal)1 : (decimal)(1 - y.Discount)))))
                //    });

                //foreach(var item in query)
                //{
                //    Console.WriteLine(item.nom);
                //    decimal chiffre = 0;
                //    foreach(decimal x in item.chiffre )
                //    {
                //        chiffre += x;
                //    }
                //    Console.WriteLine(chiffre);
                //}

                var query2 = dbcontext.Customers.Where(x => x.Country.Contains(country))
                    .Join(dbcontext.Orders, c => c.CustomerId, o => o.CustomerId, (c, o) => new { cus = c, ord = o})
                    .Join(dbcontext.OrderDetails, p => p.ord.OrderId, t => t.OrderId, (p, t) => new { cusOr = p, orde = t })
                    .GroupBy(or=> new { or.cusOr.cus.CustomerId, or.cusOr.cus.CompanyName } )
                    .Select(y=>new { name=y.Key.CompanyName, chiffre= y
                    .Sum(t=>t.orde.Quantity*( t.orde.UnitPrice*(t.orde.Discount==0?(decimal)1:(decimal)(1-t.orde.Discount)) ) ) }) ;

                /*var query3 = from customer in dbcontext.Customers
                             join order in
                             dbcontext.Orders on customer.CustomerId equals order.CustomerId into orderss
                             from o in orderss.DefaultIfEmpty()
                             join orderdetail in
                             dbcontext.OrderDetails on o.OrderId equals orderdetail.OrderId into orderdetails
                             from or in orderdetails.DefaultIfEmpty()
                             where customer.Country.StartsWith(country)
                             group customer by new { name = customer.CompanyName, id = customer.CustomerId } into c
                             select new
                             {
                                 name = c.Key.name,
                                 id = c.Key.id,
                                 chiffre = c.Sum(x=>x.Orders.Count())
                             };*/

                foreach (var item in query2)
                {
                    Console.WriteLine(item.name+" "+item.chiffre);
                }
            }
        }

        internal static void ClientMoreRemise()
        {
            using (ComptoirAnglais_V1Context dbcontext = new ComptoirAnglais_V1Context())
            {
                var query2 = dbcontext.Customers
                   .Join(dbcontext.Orders, c => c.CustomerId, o => o.CustomerId, (c, o) => new { cus = c, ord = o })
                   .Join(dbcontext.OrderDetails, p => p.ord.OrderId, t => t.OrderId, (p, t) => new { cusOr = p, orde = t })
                   .GroupBy(or => new { or.cusOr.cus.CustomerId, or.cusOr.cus.CompanyName })
                   .Select(y => new {
                       chiffre = y
                   .Sum(t => t.orde.Quantity * (t.orde.UnitPrice * (t.orde.Discount == 0 ? (decimal)1 : (decimal)(1 - t.orde.Discount))))
                   }).Max(t => t.chiffre);

                var query = dbcontext.Customers
                   .Join(dbcontext.Orders, c => c.CustomerId, o => o.CustomerId, (c, o) => new { cus = c, ord = o })
                   .Join(dbcontext.OrderDetails, p => p.ord.OrderId, t => t.OrderId, (p, t) => new { cusOr = p, orde = t })
                   .GroupBy(or => new { or.cusOr.cus.CustomerId, or.cusOr.cus.CompanyName })
                   .Select(y => new {
                       name = y.Key.CompanyName,
                       id = y.Key.CustomerId,
                       chiffre = y
                   .Sum(t => t.orde.Quantity * (t.orde.UnitPrice * (t.orde.Discount == 0 ? (decimal)1 : (decimal)(1 - t.orde.Discount))))
                   }).Where(p=>p.chiffre==query2) ;

                foreach (var item in query)
                {
                    Console.WriteLine(item.name+" " +item.id+" "+item.chiffre);
                }
            }
        }

        internal static void ListeMaxPriceCategorie()
        {
            using (ComptoirAnglais_V1Context dbcontext = new ComptoirAnglais_V1Context())
            {
                var query = dbcontext.Categories.Include(x => x.Products)
                    .Where(t => t.Products.Select(h => h.UnitPrice)==t.Products.Max(u=>u.UnitPrice));
            }
        }
    }
}
