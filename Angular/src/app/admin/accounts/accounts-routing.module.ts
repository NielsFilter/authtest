import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ListComponent } from './admin-account-list.component';
import { AdminAccountEditComponent } from './admin-account-edit.component';

const routes: Routes = [
  { path: '', component: ListComponent },
  { path: 'add', component: AdminAccountEditComponent },
  { path: 'edit/:id', component: AdminAccountEditComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AccountsRoutingModule {}
