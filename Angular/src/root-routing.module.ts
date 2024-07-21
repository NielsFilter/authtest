import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '@app/_helpers';

const accountModule = () => import('./account/account.module').then(x => x.AccountModule);
const appModule = () => import('./app/app.module').then(x => x.AppModule);

const routes: Routes = [
    { path: '', redirectTo: 'app/home', pathMatch: 'full' },
    { path: 'account', loadChildren: accountModule },
    { path: 'app', loadChildren: appModule, canActivate: [AuthGuard] },

    //todo;
    // // otherwise redirect to home
    { path: '**', redirectTo: 'app/home' }    
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class RootRoutingModule {}
