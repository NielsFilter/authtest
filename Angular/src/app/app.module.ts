import { NgModule, APP_INITIALIZER, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { JwtInterceptor, ErrorInterceptor, appInitializer } from './_helpers';
import { AppComponent } from './app.component';
import { HomeComponent } from './home';
import { ServiceClientModule } from 'src/shared/service-clients/service-client.module';
import { AuthClient } from 'src/shared/service-clients/service-clients';
import { AppSharedModule } from 'src/shared/app-shared.module';

@NgModule({
    imports: [
        BrowserModule,
        ReactiveFormsModule,
        HttpClientModule,
        AppRoutingModule,
        ServiceClientModule,
        AppSharedModule,
        FormsModule
    ],
    declarations: [
        AppComponent,
        HomeComponent
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    providers: [
        { provide: APP_INITIALIZER, useFactory: appInitializer, deps: [AuthClient], multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
