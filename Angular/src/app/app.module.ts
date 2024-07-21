import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ServiceClientModule } from 'src/shared/service-clients/service-client.module';
import { AppSharedModule } from 'src/shared/app-shared.module';
import { FooterComponent } from './footer/app-footer.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { appInitializer, JwtInterceptor } from './_helpers';
import { AuthClient } from 'src/shared/service-clients/service-clients';

@NgModule({
    imports: [
        CommonModule,
        ReactiveFormsModule,
        AppRoutingModule,
        ServiceClientModule,
        AppSharedModule,
        FormsModule,
        FooterComponent
    ],
    declarations: [
        AppComponent
    ],
    providers: [
        { provide: APP_INITIALIZER, useFactory: appInitializer, deps: [AuthClient], multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
    ],
})
export class AppModule { }
