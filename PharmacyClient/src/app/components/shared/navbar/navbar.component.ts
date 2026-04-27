import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { CartService } from '../../../services/cart.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],})
export class NavbarComponent {
  authService = inject(AuthService);
  cartService = inject(CartService);
  showMobile = false;

  toggleMobile() {
    this.showMobile = !this.showMobile;
  }

  closeMobile() {
    this.showMobile = false;
  }

  logout() {
    this.authService.logout();
    this.cartService.clearCart();
    this.showMobile = false;
    window.location.href = '/login';
  }
}
