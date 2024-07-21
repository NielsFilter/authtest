import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ProfileLayoutComponent } from './profile-layout.component';
import { ProfileViewComponent } from './view/profile-view.component';
import { ProfilePersonalComponent } from './personal/profile-personal.component';
import { ProfileSecurityComponent } from './security/profile-security.component';
import { ProfileSettingsComponent } from './settings/profile-settings.component';

const routes: Routes = [
    {
        path: '', component: ProfileLayoutComponent,
        children: [
            { path: '', component: ProfileViewComponent },
            { path: 'personal', component: ProfilePersonalComponent },
            { path: 'security', component: ProfileSecurityComponent },
            { path: 'settings', component: ProfileSettingsComponent }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ProfileRoutingModule { }
