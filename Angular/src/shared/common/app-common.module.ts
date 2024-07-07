import { NgModule } from '@angular/core';
import { AppComponentBase } from './app-component-base';
import { TableModule } from 'primeng/table';

@NgModule({
    imports: [
        TableModule
    ],
    providers: [
        TableModule
    ],
})
export class AppCommonModule {}