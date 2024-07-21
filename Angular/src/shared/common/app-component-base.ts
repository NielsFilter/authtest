import { Component, Injector, OnDestroy, OnInit } from "@angular/core";
import { AccountDto, AccountSessionInfo, AuthenticateDto, ProfileSettingResult } from "../service-clients/service-clients";
import { AuthService } from "@app/_services/auth.service";
import { BehaviorSubject, Observable, Subscription } from "rxjs";
import { Role } from "@app/_models";
import { SettingsService } from "@app/_services/settings.service";

@Component({
    template: '',
})
export abstract class AppComponentBase implements OnDestroy {

    defaultPageSize: number = 10;
    account: AccountDto | null;
    profileImage: string = 'https://mdbootstrap.com/img/Photos/Others/placeholder-avatar.jpg';
    profileSettings: ProfileSettingResult | null;
    darkTheme: string = "light";
    isAdmin: boolean = false;

    private authSub: Subscription;
    private settingsSub: Subscription;

    constructor(injector: Injector) { 
        const authService = injector.get(AuthService);
        const settingsService = injector.get(SettingsService);

        // Subscribe the currentQuote property of quote service to get real time value
        this.authSub = authService.account$.subscribe(
            // update the component's property
            res => { 
                this.account = res ?? null;
                this.profileImage = this.account?.image ?? 'https://mdbootstrap.com/img/Photos/Others/placeholder-avatar.jpg';
                this.isAdmin = this.account?.roles != null && this.account.roles.indexOf(Role.Admin) > -1;
            }
        );

        this.settingsSub = settingsService.setting$.subscribe(
            // update the component's property
            res => { 
                console.log('settings loaded', this.profileSettings);
                this.profileSettings = res ?? null;
                this.darkTheme = this.profileSettings?.isDarkMode ? "dark" : "light";
            }
        );
    }

    ngOnDestroy(): void {
        this.authSub.unsubscribe();
        this.settingsSub.unsubscribe();
    }
}
