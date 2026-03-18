using FluentValidation;
using AppCore.Dto;
using AppCore.Interfaces;

public class UpdatePersonDtoValidator : AbstractValidator<UpdatePersonDto>
{
    private readonly ICompanyRepository _companyRepository;

    public UpdatePersonDtoValidator(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Imię jest wymagane.")
            .MaximumLength(100).WithMessage("Imię nie może przekraczać 100 znaków.")
            .Matches(@"^[\p{L}\s\-]+$").WithMessage("Imię zawiera niedozwolone znaki.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Nazwisko jest wymagane.")
            .MaximumLength(200).WithMessage("Nazwisko nie może przekraczać 200 znaków.")
            .Matches(@"^[\p{L}\s\-]+$").WithMessage("Nazwisko zawiera niedozwolone znaki.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email jest wymagany.")
            .EmailAddress().WithMessage("Nieprawidłowy format adresu email.")
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[0-9\s\-]{7,15}$")
            .WithMessage("Nieprawidłowy format numeru telefonu.")
            .When(x => x.Phone is not null);

        RuleFor(x => x.EmployerId)
            .MustAsync(EmployerExistsAsync)
            .WithMessage("Wskazana firma nie istnieje.")
            .When(x => x.EmployerId.HasValue);
    }

    private async Task<bool> EmployerExistsAsync(Guid? employerId, CancellationToken ct) =>
        await _companyRepository.FindByIdAsync(employerId ?? Guid.Empty) is not null;
}