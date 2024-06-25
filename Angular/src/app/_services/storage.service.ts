import { Injectable } from '@angular/core';
import { AuthenticateDto, AuthenticationResult } from 'src/shared/service-clients/service-clients';

const AUTH_KEY = 'auth';

@Injectable({
  providedIn: 'root'
})
export class StorageService {



  constructor() { }


  saveAuth(user : AuthenticationResult){
    window.localStorage.removeItem(AUTH_KEY);
    window.localStorage.setItem(AUTH_KEY, JSON.stringify(user));
  }
  getAuth() : AuthenticationResult | null {
    const user = window.localStorage.getItem(AUTH_KEY);
    if (user) {
      return JSON.parse(user);
    }
    return null;
  }

  clean(): void {
    window.localStorage.clear();
  }
}