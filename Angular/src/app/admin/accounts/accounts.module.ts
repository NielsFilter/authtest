import {NgModule} from '@angular/core';
import {ReactiveFormsModule} from '@angular/forms';
import {CommonModule} from '@angular/common';

import {AccountsRoutingModule} from './accounts-routing.module';
import {ListComponent} from './list.component';
import {AddEditComponent} from './add-edit.component';
import { AppCommonModule } from 'src/shared/common/app-common.module';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';

@NgModule({
    imports: [
        CommonModule,
        TableModule,
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
