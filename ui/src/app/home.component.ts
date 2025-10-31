import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { Observable } from 'rxjs';

interface Claim {
  type: string;
  value: string;
}

interface UserProfile {
  isAuthenticated: boolean;
  nameClaimType: string;
  roleClaimType: string;
  claims: Claim[];
}

@Component({
    selector: 'app-home',
    templateUrl: 'home.component.html',
    imports: [CommonModule]
})
export class HomeComponent implements OnInit {
  private readonly httpClient = inject(HttpClient);
  dataFromAzureProtectedApi$?: Observable<string[]>;
  dataGraphApiCalls$?: Observable<string[]>;
  userProfileClaims$?: Observable<UserProfile>;

  ngOnInit() {
    console.info('home component');
    this.getUserProfile();
  }

  getUserProfile() {
    this.userProfileClaims$ = this.httpClient.get<UserProfile>(
      `${this.getCurrentHost()}/api/User`
    );
  }

  getDirectApiData() {
    this.dataFromAzureProtectedApi$ = this.httpClient.get<string[]>(
      `${this.getCurrentHost()}/api/DirectApi`
    );
  }

  getGraphApiDataUsingApi() {
    this.dataGraphApiCalls$ = this.httpClient.get<string[]>(
      `${this.getCurrentHost()}/api/GraphApiData`
    );
  }

  private getCurrentHost() {
    const host = window.location.host;
    const url = `${window.location.protocol}//${host}`;

    return url;
  }
}
