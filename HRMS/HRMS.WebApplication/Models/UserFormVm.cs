using HRMS.SharedKernel.Models.Response;

namespace HRMS.WebApplication.Models
{
    public sealed class UserFormVm
    {
        public FormMode Mode { get; init; }

        public UserDetailsResponseDto UserDetails { get; init; } = new();

        public bool IsReadOnly => Mode == FormMode.View;
        public bool ShowPassword => Mode != FormMode.View;
        public bool CanSubmit => Mode != FormMode.View;
        public string SubmitUrl { get; init; } = string.Empty;
    }

    public enum FormMode
    {
        Add,
        Edit,
        View
    }
}
