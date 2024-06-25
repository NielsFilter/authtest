import { NgModule } from '@angular/core';
import { AppSessionService } from './session/app-session.service';
import { AppComponentBase } from './app-component-base';

@NgModule({
    providers: [
        AppSessionService
    ],
})
export class AppCommonModule {}