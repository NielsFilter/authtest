import {NgModule} from '@angular/core';
import {ReactiveFormsModule} from '@angular/forms';
import {CommonModule} from '@angular/common';

import {AccountsRoutingModule} from './accounts-routing.module';
import {ListComponent} from './list.component';
import {AddEditComponent} from './add-edit.component';
import { AppSharedModule } from 'src/shared/app-shared.module';
import { ButtonModule } from 'primeng/button';

@NgModule({
    imports: [
        CommonModule,
        AppSharedModule,
        ButtonModule, 
        ReactiveFormsModule,
        AccountsRoutingModule
    ],
    declarations: [
        ListComponent,
        AddEditComponent
    ]
})
export class AccountsModule { }
