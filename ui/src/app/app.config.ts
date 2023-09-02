import { ApplicationConfig } from '@angular/core';
import { provideHttpClient, HTTP_INTERCEPTORS } from "@angular/common/http";
import { SecureApiInterceptor } from './secureapi.interceptor';

import {
  provideRouter,
  withEnabledBlockingInitialNavigation,
} from '@angular/router';
import { appRoutes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(appRoutes, withEnabledBlockingInitialNavigation()), 
    provideHttpClient(),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: SecureApiInterceptor,
      multi: true,
    }
  ]
};
