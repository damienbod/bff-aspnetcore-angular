import { provideZoneChangeDetection } from "@angular/core";
/// <reference types="@angular/localize" />

import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

// Import Bootstrap JavaScript for navbar collapse and other components
import 'bootstrap';

bootstrapApplication(AppComponent, {...appConfig, providers: [provideZoneChangeDetection(), ...appConfig.providers]}).catch((err) =>
  console.error(err)
);
