import { CommonModule } from '@angular/common';
import { Component, input, output, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth';


@Component({
  selector: 'app-menu',
  imports: [RouterModule, CommonModule],
  templateUrl: './menu.html',
  styleUrl: './menu.css',
})
export class Menu {
  isMenuCollapsed = input.required<boolean>();
  changeIsMenuCollapsed = output<boolean>();

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    if (!this.authService.isAdmin())
      this.items[2].label = 'Profile';
  }

  toggleCollapse(): void {
    this.changeIsMenuCollapsed.emit(!this.isMenuCollapsed());
  }

  closeSidenav(): void {
    this.changeIsMenuCollapsed.emit(true);
  }

  items = [
    {
      routeLink: '',
      icon: 'home',
      label: 'Home',
    },
    {
      routeLink: 'products',
      icon: 'pallet',
      label: 'Products',
    },
    {
      routeLink: 'users',
      icon: 'groups',
      label: 'Users'
    },
    {
      routeLink: 'login',
      icon: 'logout',
      label: 'Logout',
    },
  ];

}
