using System.ComponentModel.Design;
using Microsoft.VisualBasic;

namespace ordination_test;

using Microsoft.EntityFrameworkCore;

using Service;
using Data;
using shared.Model;

[TestClass]
public class ServiceTest
{
    private DataService service;

    [TestInitialize]
    public void SetupBeforeEachTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();
        optionsBuilder.UseInMemoryDatabase(databaseName: "test-database");
        var context = new OrdinationContext(optionsBuilder.Options);
        service = new DataService(context);
        service.SeedData();
    }

    [TestMethod]
    public void PatientsExist()
    {
        Assert.IsNotNull(service.GetPatienter());
    }

    [TestMethod]
    public void OpretDagligFast()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(1, service.GetDagligFaste().Count());

        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligFaste().Count());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestAtKodenSmiderEnException()
    {
        // Herunder skal man så kalde noget kode,
        // der smider en exception.

        // Hvis koden _ikke_ smider en exception,
        // så fejler testen.
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId, -1, 2, -2, 2, DateTime.Now,
            DateTime.Now.AddDays(3));

        Console.WriteLine("Her kommer der ikke en exception. Testen fejler.");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestAfForkertDato()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DateTime startDate = DateTime.Now;
        DateTime endDate = startDate.AddDays(-2);

        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId, 1, 2, 1, 2, startDate, endDate);
    }

    [TestMethod]
    public void TestAfKorrektSkaev()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        
        var doser = new Dosis[]
        {
            new Dosis { tid = DateTime.Now.AddHours(8), antal = 2.5 },
            new Dosis { tid = DateTime.Now.AddHours(12), antal = 1.5 },
            new Dosis { tid = DateTime.Now.AddHours(20), antal = 3.0 }
        };

        service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, DateTime.Now,
            DateTime.Now.AddDays(3));
    }
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestAfIngenDosisSkaev()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        
        var doser = new Dosis[]
        {
        };

        service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, DateTime.Now,
            DateTime.Now.AddDays(3));
    }
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestAfNegativDosisSkaev()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        
        var doser = new Dosis[]
        {
            new Dosis { tid = DateTime.Now.AddHours(8), antal = -2.5 },
            new Dosis { tid = DateTime.Now.AddHours(12), antal = -1.5 },
            new Dosis { tid = DateTime.Now.AddHours(20), antal = 3.0 }
        };

        service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, DateTime.Now,
            DateTime.Now.AddDays(3));
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestAfIngenDosis()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        
        var doser = new Dosis[]
        {
            new Dosis { tid = DateTime.Now.AddHours(8), antal = 0 },
            new Dosis { tid = DateTime.Now.AddHours(12), antal = 1.5 },
            new Dosis { tid = DateTime.Now.AddHours(20), antal = 3.0 }
        };

        service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, DateTime.Now,
            DateTime.Now.AddDays(3));
    }
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void StartEfterSlutSkaev()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        
        var doser = new Dosis[]
        {
            new Dosis { tid = DateTime.Now.AddHours(8), antal = 2.5 },
            new Dosis { tid = DateTime.Now.AddHours(10), antal = 1.5 },
        };
        DateTime startDate = DateTime.Now;
        DateTime endDate = startDate.AddDays(-5);

        service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, startDate,
            endDate);
    }

    [TestMethod]
    public void PNTest()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        service.OpretPN(patient.PatientId, lm.LaegemiddelId, 2, DateTime.Now, DateTime.Now.AddDays(4));
    }
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PNNegativDosis()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        service.OpretPN(patient.PatientId, lm.LaegemiddelId, -2, DateTime.Now, DateTime.Now.AddDays(4));
    }
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PNIngenDosis()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        service.OpretPN(patient.PatientId, lm.LaegemiddelId, 0, DateTime.Now, DateTime.Now.AddDays(4));
    }
    
    //glemte push
}