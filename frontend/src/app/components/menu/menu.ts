import { CommonModule } from '@angular/common';
import { Component, input, output, signal } from '@angular/core';
import { RouterModule } from '@angular/router';


@Component({
  selector: 'app-menu',
  imports: [RouterModule, CommonModule],
  templateUrl: './menu.html',
  styleUrl: './menu.css',
})
export class Menu {
  isMenuCollapsed = input.required<boolean>();
  changeIsMenuCollapsed = output<boolean>();

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
      label: 'Users',
    },
    {
      routeLink: 'login',
      icon: 'logout',
      label: 'Logout',
    },
  ];

}
