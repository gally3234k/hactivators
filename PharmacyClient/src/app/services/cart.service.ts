import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { CartItem } from '../models/cart-item.model';
import { Medicine } from '../models/medicine.model';

@Injectable({ providedIn: 'root' })
export class CartService {
  private cartItems: CartItem[] = [];
  private cartSubject = new BehaviorSubject<CartItem[]>([]);
  public cart$ = this.cartSubject.asObservable();

  private totalSubject = new BehaviorSubject<number>(0);
  public total$ = this.totalSubject.asObservable();

  constructor() {
    const stored = localStorage.getItem('cart');
    if (stored) {
      this.cartItems = JSON.parse(stored);
      this.cartSubject.next(this.cartItems);
      this.calculateTotal();
    }
  }

  addToCart(medicine: Medicine, quantity: number = 1): void {
    const existing = this.cartItems.find(item => item.medicine.id === medicine.id);
    if (existing) {
      existing.quantity += quantity;
    } else {
      this.cartItems.push({ medicine, quantity });
    }
    this.updateCart();
  }

  removeFromCart(medicineId: number): void {
    this.cartItems = this.cartItems.filter(item => item.medicine.id !== medicineId);
    this.updateCart();
  }

  updateQuantity(medicineId: number, quantity: number): void {
    const item = this.cartItems.find(i => i.medicine.id === medicineId);
    if (item) {
      item.quantity = quantity;
      if (item.quantity <= 0) {
        this.removeFromCart(medicineId);
        return;
      }
    }
    this.updateCart();
  }

  clearCart(): void {
    this.cartItems = [];
    this.updateCart();
  }

  getCartItems(): CartItem[] {
    return this.cartItems;
  }

  getItemCount(): number {
    return this.cartItems.reduce((sum, item) => sum + item.quantity, 0);
  }

  requiresPrescription(): boolean {
    return this.cartItems.some(item => item.medicine.requiresPrescription);
  }

  private calculateTotal(): void {
    const total = this.cartItems.reduce(
      (sum, item) => sum + item.medicine.price * item.quantity, 0
    );
    this.totalSubject.next(total);
  }

  private updateCart(): void {
    localStorage.setItem('cart', JSON.stringify(this.cartItems));
    this.cartSubject.next([...this.cartItems]);
    this.calculateTotal();
  }
}
