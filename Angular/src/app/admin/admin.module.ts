import {NgModule} from '@angular/core';
import {ReactiveFormsModule} from '@angular/forms';
import {CommonModule} from '@angular/common';

import {AdminRoutingModule} from './admin-routing.module';
import {SubNavComponent} from './subnav.component';
import {LayoutComponent} from './layout.component';
import {OverviewComponent} from './overview.component';
import { AppCommonModule } from 'src/shared/common/app-common.module';

@NgModule({
    imports: [
        CommonModule,
        AppCommonModule,
        ReactiveFormsModule,
        AdminRoutingModule
    ],
    declarations: [
        SubNavComponent,
        LayoutComponent,
        OverviewComponent
    ]
})
export class AdminModule { }
