import { Injectable } from '@angular/core';
import { AuthenticateDto, AuthenticationResult } from 'src/shared/service-clients/service-clients';

const AUTH_KEY = 'auth';

@Injectable({
  providedIn: 'root'
})
export class StorageService {

  saveItem<T>(key: string, value: T){
    window.localStorage.removeItem(key);
    window.localStorage.setItem(key, JSON.stringify(value));
  }

  getItem<T>(key: string) : T | null {
     let value = window.localStorage.getItem(key);
    if (value) {
      return JSON.parse(value);
    }
    return null;
  }

  clean(): void {
    window.localStorage.clear();
  }
}