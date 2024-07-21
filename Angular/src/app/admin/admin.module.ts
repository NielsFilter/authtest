import {NgModule} from '@angular/core';
import {ReactiveFormsModule} from '@angular/forms';
import {CommonModule} from '@angular/common';

import {AdminRoutingModule} from './admin-routing.module';
import {AdminSubNavComponent} from './admin-subnav.component';
import {AdminLayoutComponent} from './admin-layout.component';
import {AdminOverviewComponent} from './admin-overview.component';
import { AppSharedModule } from 'src/shared/app-shared.module';

@NgModule({
    imports: [
        CommonModule,
        AppSharedModule,
        ReactiveFormsModule,
        AdminRoutingModule
    ],
    declarations: [
        AdminSubNavComponent,
        AdminLayoutComponent,
        AdminOverviewComponent
    ]
})
export class AdminModule { }
