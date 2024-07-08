import { NgModule } from '@angular/core';
import { TableModule } from 'primeng/table';
import { AlertComponent } from './components/alert/alert.component';
import { CommonModule } from '@angular/common';

@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [
        AlertComponent
    ],
    exports: [
        TableModule,
        AlertComponent
    ]
})
export class AppSharedModule {}