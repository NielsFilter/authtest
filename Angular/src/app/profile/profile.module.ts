import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { ProfileRoutingModule } from './profile-routing.module';
import { LayoutComponent } from './layout.component';
import { DetailsComponent } from './details.component';
import { UpdateComponent } from './update.component';
import { AppSharedModule } from 'src/shared/app-shared.module';

@NgModule({
    imports: [
        CommonModule,
        AppSharedModule,
        ReactiveFormsModule,
        ProfileRoutingModule
    ],
    declarations: [
        LayoutComponent,
        DetailsComponent,
        UpdateComponent
    ]
})
export class ProfileModule { }