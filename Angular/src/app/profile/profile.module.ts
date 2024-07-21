import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ProfileRoutingModule } from './profile-routing.module';
import { ProfileLayoutComponent } from './profile-layout.component';
import { ProfileViewComponent } from './view/profile-view.component';
import { AppSharedModule } from 'src/shared/app-shared.module';
import { ProfilePersonalComponent } from './personal/profile-personal.component';
import { ProfileSettingsComponent } from './settings/profile-settings.component';
import { ProfileSecurityComponent } from './security/profile-security.component';

@NgModule({
    imports: [
        CommonModule,
        AppSharedModule,
        ReactiveFormsModule,
        ProfileRoutingModule
    ],
    declarations: [
        ProfileLayoutComponent,
        ProfileViewComponent,
        ProfilePersonalComponent,
        ProfileSecurityComponent,
        ProfileSettingsComponent
    ]
})
export class ProfileModule { }
