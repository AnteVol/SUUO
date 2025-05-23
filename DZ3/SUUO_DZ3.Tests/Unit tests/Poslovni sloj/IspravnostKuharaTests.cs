using FluentValidation.TestHelper;
using SUUO_DZ3.Models;
using SUUO_DZ3.Models.Validation;

namespace SUUO_DZ3.Tests.Unit_tests;

public class IspravnostKuharaTests
{
    private readonly KuharValidator _validator;

    public IspravnostKuharaTests()
    {
        _validator = new KuharValidator();
    }

    [Fact]
    public void Should_Not_Have_Validation_Error_For_Valid_Telefon()
    {
        var kuhar = new Kuhar { Telefon = "+385-912345678" };
        var result = _validator.TestValidate(kuhar);
        result.ShouldNotHaveValidationErrorFor(x => x.Telefon);
    }

    [Fact]
    public void Should_Have_Validation_Error_For_Invalid_Telefon()
    {
        var kuhar = new Kuhar { Telefon = "0912345678" };
        var result = _validator.TestValidate(kuhar);
        result.ShouldHaveValidationErrorFor(x => x.Telefon);
    }

    [Fact]
    public void Should_Not_Have_Validation_Error_For_Valid_Email()
    {
        var kuhar = new Kuhar { Email = "kuhar@example.com" };
        var result = _validator.TestValidate(kuhar);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Validation_Error_For_Invalid_Email()
    {
        var kuhar = new Kuhar { Email = "neispravanemail" };
        var result = _validator.TestValidate(kuhar);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Not_Have_Validation_Error_For_Valid_Specijaliteti()
    {
        const string specijaliteti = "Riba;Mesno";
        var kuhar = new Kuhar { Specijaliteti = specijaliteti.Split(";").ToList() };
        var result = _validator.TestValidate(kuhar);
        result.ShouldNotHaveValidationErrorFor(x => x.Specijaliteti);
    }

    [Fact]
    public void Should_Have_Validation_Error_For_Empty_Specijaliteti()
    {
        var kuhar = new Kuhar { Specijaliteti = new List<string>() };
        var result = _validator.TestValidate(kuhar);
        result.ShouldHaveValidationErrorFor(x => x.Specijaliteti);
    }
}