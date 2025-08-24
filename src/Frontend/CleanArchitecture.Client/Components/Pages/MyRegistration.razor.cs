using CleanArchitecture.Application.DTOs.UserDtos;
using CleanArchitecture.Presentation.FrontendService;
using Microsoft.AspNetCore.Components;

namespace CleanArchitecture.Client.Components.Pages;

public partial class MyRegistration : ComponentBase
{
    
    [Inject] private MultiService MultiService { get; set; }
    private bool ShowReg { get; set; }
    private bool IsLoading { get; set; }
    private bool ShowMessageModal { get; set; }
    private string ModalMessage { get; set; } = string.Empty;
    private List<UserRegistrationInfoDto> RegistrationInfo { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await GetRegistrationInfo();
    }

    public async Task GetRegistrationInfo()
    {
        IsLoading = true;
        ShowReg = false;

        var regInfo = new List<UserRegistrationInfoDto>();

        
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        
        var user = authState.User;

        if (user is null)
            throw new ApplicationException();

        var userEntity = await MultiService.ApplicationUserService.GetUserByEmailAsync(user.Identity.Name);

        try
        {
            var registrationInfoList =
                await MultiService.UserMeetingRegistrationFormService.GetRegistrationInfoByUserIdAsync(userEntity.Value.Id);

            if (registrationInfoList.IsFailed)
            {
                OpenMessageModal(registrationInfoList.Errors.FirstOrDefault()?.Message ?? "Unknown error");
            }

            regInfo = registrationInfoList.Value;
          
        }
        catch (Exception)
        {
            OpenMessageModal("Internt serverfel.");
            IsLoading = false;
            return;
        }

        if (regInfo.Count == 0)
        {
            return;
        }

        RegistrationInfo = regInfo;
        IsLoading = false;
        ShowReg = true;
    }

    #region MessageModal

    private void OpenMessageModal(string message)
    {
        ModalMessage = message;
        ShowMessageModal = true;
    }

    private void CloseMessageModal()
    {
        ShowMessageModal = false;
        ModalMessage = string.Empty;
    }

    #endregion
}