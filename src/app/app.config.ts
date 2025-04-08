
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes)]
};

import { ApplicationConfig } from '@angular/core';
import { provideRouter, Routes } from '@angular/router';


import { MicrowaveComponent } from './components/microwave/microwave.component';

const routes: Routes = [
  { path: '', component: MicrowaveComponent }
];

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
  ],
};
