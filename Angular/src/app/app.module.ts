import { NgModule, APP_INITIALIZER, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { JwtInterceptor, ErrorInterceptor, appInitializer } from './_helpers';
import { AppComponent } from './app.component';
import { AlertComponent } from './_components';
import { HomeComponent } from './home';
import { ServiceClientModule } from 'src/shared/service-clients/service-client.module';
import { environment } from '@environments/environment';
import { CommonModule } from '@angular/common';
import { AccountsClient } from 'src/shared/service-clients/service-clients';
import { AppCommonModule } from 'src/shared/common/app-common.module';

@NgModule({
    imports: [
        BrowserModule,
        ReactiveFormsModule,
        HttpClientModule,
        AppRoutingModule,
        ServiceClientModule,
        AppCommonModule,
        FormsModule
    ],
    declarations: [
        AppComponent,
        AlertComponent,
        HomeComponent
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    providers: [
        { provide: APP_INITIALIZER, useFactory: appInitializer, deps: [AccountsClient], multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
        //TODO: { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
