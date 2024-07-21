import { Component, OnInit } from '@angular/core';
import { AuthService } from '@app/_services/auth.service';
import { SettingsService } from '@app/_services/settings.service';

@Component({
    selector: 'app-root',
    templateUrl: './root.component.html',
})
export class RootComponent implements OnInit {
    // ngxSpinnerText: NgxSpinnerTextService;

    constructor(
        private authService: AuthService,
        private settingService: SettingsService){
    }
    // constructor(_ngxSpinnerText: NgxSpinnerTextService) {
    //     this.ngxSpinnerText = _ngxSpinnerText;
    // }

    // getSpinnerText(): string {
    //     return this.ngxSpinnerText.getText();
    // }
    ngOnInit() {
        console.log('auto login root');
        const isLoggedIn = this.authService.autoLogin();
        if(isLoggedIn) {
            this.settingService.loadStoredSettings();
        }
      }
}
