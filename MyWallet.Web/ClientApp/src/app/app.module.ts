import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { AuthGuard } from './auth.guard';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { WalletEntryListComponent } from './wallet-entry/wallet-entry-list/wallet-entry-list.component';
import { LoginComponent } from './login/login.component';
import {JwtInterceptor} from "./shared/interceptors/jwt.interceptor";
import {ErrorInterceptor} from "./shared/interceptors/error.interceptor";
import { RegisterComponent } from './user/register/register.component';
import { AddWalletEntryComponent } from './wallet-entry/add-wallet-entry/add-wallet-entry.component';
import { UpdateWalletEntryComponent } from './wallet-entry/update-wallet-entry/update-wallet-entry.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    WalletEntryListComponent,
    LoginComponent,
    RegisterComponent,
    AddWalletEntryComponent,
    UpdateWalletEntryComponent
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      {path: '', component: HomeComponent, pathMatch: 'full'},
      {path: 'wallet-entry', component: WalletEntryListComponent, canActivate: [AuthGuard]},
      {path: 'login', component: LoginComponent},
      {path: 'register', component: RegisterComponent},
      {path: 'add-wallet-entry', component: AddWalletEntryComponent, canActivate: [AuthGuard]},
      {path: 'update-wallet-entry/:id', component: UpdateWalletEntryComponent, canActivate: [AuthGuard]}
    ]),
    ReactiveFormsModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
