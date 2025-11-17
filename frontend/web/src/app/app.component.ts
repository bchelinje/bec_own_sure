import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink } from '@angular/router';
import { AuthService } from '@core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink],
  template: `
    <div class="app-container">
      <header class="app-header" *ngIf="authService.isAuthenticated()">
        <nav class="navbar">
          <div class="nav-brand">
            <a routerLink="/dashboard">Device Ownership Platform</a>
          </div>
          <ul class="nav-menu">
            <li><a routerLink="/dashboard">Dashboard</a></li>
            <li><a routerLink="/devices">My Devices</a></li>
            <li><a routerLink="/marketplace">Marketplace</a></li>
            <li><a routerLink="/reports">Reports</a></li>
            <li><a (click)="logout()" class="logout-btn">Logout</a></li>
          </ul>
        </nav>
      </header>

      <main class="app-content">
        <router-outlet></router-outlet>
      </main>

      <footer class="app-footer" *ngIf="authService.isAuthenticated()">
        <p>&copy; 2024 Device Ownership Platform. All rights reserved.</p>
      </footer>
    </div>
  `,
  styles: [`
    .app-container {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }

    .app-header {
      background-color: #1976d2;
      color: white;
      padding: 0;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .navbar {
      max-width: 1200px;
      margin: 0 auto;
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1rem 2rem;
    }

    .nav-brand a {
      color: white;
      text-decoration: none;
      font-size: 1.5rem;
      font-weight: bold;
    }

    .nav-menu {
      display: flex;
      list-style: none;
      margin: 0;
      padding: 0;
      gap: 2rem;
    }

    .nav-menu a,
    .logout-btn {
      color: white;
      text-decoration: none;
      cursor: pointer;
      transition: opacity 0.2s;
    }

    .nav-menu a:hover,
    .logout-btn:hover {
      opacity: 0.8;
    }

    .app-content {
      flex: 1;
      background-color: #f5f5f5;
    }

    .app-footer {
      background-color: #333;
      color: white;
      text-align: center;
      padding: 1rem;
    }
  `]
})
export class AppComponent {
  constructor(public authService: AuthService) {}

  logout(): void {
    this.authService.logout();
  }
}
