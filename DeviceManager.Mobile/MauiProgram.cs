using DeviceManager.Mobile.Data;
using DeviceManager.Mobile.ViewModels;
using DeviceManager.Mobile.Views;
using Microsoft.Extensions.Logging;

namespace DeviceManager.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<RealmDbContext>();
            builder.Services.AddSingleton<DeviceViewModel>();
            builder.Services.AddTransient<DeviceFormViewModel>();
            builder.Services.AddTransient<DeviceFormPage>();
            builder.Services.AddSingleton<MainPage>();


            return builder.Build();
        }
    }
}
