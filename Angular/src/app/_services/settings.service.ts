import { Injectable } from '@angular/core';
import {
  BehaviorSubject,
  Observable,
  first,
  tap,
} from 'rxjs';
import {
  ProfileClient,
  ProfileSettingResult,
} from 'src/shared/service-clients/service-clients';
import { StorageService } from './storage.service';

const SETTING_KEY = 'settings';

@Injectable({ providedIn: 'root' })
export class SettingsService {
  setting$: BehaviorSubject<ProfileSettingResult | null> = new BehaviorSubject<ProfileSettingResult | null>(null);

  constructor(
    private profileClient: ProfileClient,
    private storageService: StorageService
  ) {}

  loadStoredSettings() {
    const settings = this.storageService.getItem<ProfileSettingResult>(SETTING_KEY);
    if (settings == null) {
      this.fetchSettings().pipe(first()).subscribe();
    }
    this.setting$.next(settings);
  }

  reloadSettings() {
    this.fetchSettings().pipe(first()).subscribe();
  }

  private fetchSettings(): Observable<ProfileSettingResult> {
    return this.profileClient.profileGetAccountSettings().pipe(
      tap({
        next: (result) => {
            console.log('setting result', result);
            this.storageService.saveItem(SETTING_KEY, result);
            this.setting$.next(result ?? null);
            return result;
        },
      })
    );
  }

  clearData() {
    this.storageService.clean();
    this.setting$.next(null);
  }
}
