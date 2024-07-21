import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';

import {AdminSubNavComponent} from './admin-subnav.component';
import {AdminLayoutComponent} from './admin-layout.component';
import {AdminOverviewComponent} from './admin-overview.component';

const accountsModule = () => import('./accounts/accounts.module').then(x => x.AccountsModule);

const routes: Routes = [
    { path: '', component: AdminSubNavComponent, outlet: 'subnav' },
    {
        path: '', component: AdminLayoutComponent,
        children: [
            { path: '', component: AdminOverviewComponent },
            { path: 'accounts', loadChildren: accountsModule }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AdminRoutingModule { }
