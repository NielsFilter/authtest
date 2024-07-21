import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { AccountsRoutingModule } from './accounts-routing.module';
import { ListComponent } from './admin-account-list.component';
import { AdminAccountEditComponent } from './admin-account-edit.component';
import { AppSharedModule } from 'src/shared/app-shared.module';
import { ButtonModule } from 'primeng/button';

@NgModule({
  imports: [
    CommonModule,
    AppSharedModule,
    ButtonModule,
    ReactiveFormsModule,
    AccountsRoutingModule,
  ],
  declarations: [ListComponent, AdminAccountEditComponent],
})
export class AccountsModule {}
