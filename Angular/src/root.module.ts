import { NgModule, APP_INITIALIZER, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ServiceClientModule } from 'src/shared/service-clients/service-client.module';
import { AuthClient } from 'src/shared/service-clients/service-clients';
import { AppSharedModule } from 'src/shared/app-shared.module';
import { RootComponent } from './root.component';
import { RootRoutingModule } from './root-routing.module';
import { AppModule } from '@app/app.module';
import { AccountModule } from './account/account.module';
import { CommonModule } from '@angular/common';

@NgModule({
    imports: [
        CommonModule,
        BrowserModule,
        ReactiveFormsModule,
        HttpClientModule,
        RootRoutingModule,
        ServiceClientModule,
        AppSharedModule,
        FormsModule,
        AccountModule,
        AppModule
    ],
    declarations: [
        RootComponent
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    // providers: [
    //     { provide: APP_INITIALIZER, useFactory: appInitializer, deps: [AuthClient], multi: true },
    //     { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    //     { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
    // ],
    bootstrap: [RootComponent]
})
export class RootModule { }
