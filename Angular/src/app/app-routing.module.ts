import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { Role } from './_models';
import { AuthGuard } from './_helpers/auth.guard';
import { AppComponent } from './app.component';
import { HomeComponent } from './dashboard';
import { SettingsComponent } from './settings/settings.component';

const adminModule = () => import('./admin/admin.module').then(x => x.AdminModule);
const profileModule = () => import('./profile/profile.module').then(x => x.ProfileModule);

const routes: Routes = [
    {
        path: '', component: AppComponent, canActivate: [AuthGuard], canActivateChild: [AuthGuard],
        children: [
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
            { path: 'admin', loadChildren: adminModule, canActivate: [AuthGuard], data: { roles: [Role.Admin] } },
            { path: 'settings', component: SettingsComponent, canActivate: [AuthGuard] },
            { path: 'profile', loadChildren: profileModule, canActivate: [AuthGuard] }, //todo: don't need a whole module for this?
        
            // otherwise redirect to home
            { path: '**', redirectTo: 'home' }
        ],
    }
    // otherwise redirect to home
  //todo:  { path: '**', redirectTo: '/app/main/home' }
];


@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule {
    constructor() { }
}
