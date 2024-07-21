import { Component, Injector, OnInit } from '@angular/core';
import { finalize, first } from 'rxjs/operators';
import { AlertService } from '@app/_services';
import { ProfileClient } from 'src/shared/service-clients/service-clients';
import { AppComponentBase } from 'src/shared/common/app-component-base';
import { SettingsService } from '@app/_services/settings.service';

@Component({ templateUrl: 'profile-settings.component.html' })
export class ProfileSettingsComponent extends AppComponentBase implements OnInit {
    submitting = false;
    isDarkMode: boolean = false;

    constructor(injector: Injector,
        private profileClient: ProfileClient,
        private alertService: AlertService,
        private settingService: SettingsService
    ) {
        super(injector);
     }

    ngOnInit() {
        this.profileClient.profileGetAccountSettings().subscribe((res) => {
            this.isDarkMode = res.isDarkMode ?? false;
        });
    }

    toggleDarkMode(event: any) {
        this.isDarkMode = event.target.checked ?? false;
        this.submitting = true;
        this.profileClient.profileUpdateDarkMode(this.isDarkMode)
            .pipe(first())
            .pipe(finalize(() => setTimeout(() => this.submitting = false, 1000)))
            .subscribe(() => {
                this.settingService.reloadSettings();
                this.alertService.success('Dark mode updated successfully');
            });
    }
}
