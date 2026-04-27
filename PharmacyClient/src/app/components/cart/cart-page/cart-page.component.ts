import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CartService } from '../../../services/cart.service';
import { OrderService } from '../../../services/order.service';
import { PrescriptionService } from '../../../services/prescription.service';
import { AuthService } from '../../../services/auth.service';
import { Prescription } from '../../../models/prescription.model';
import { CreateOrder } from '../../../models/order.model';
import { LoaderComponent } from '../../shared/loader/loader.component';

@Component({
  selector: 'app-cart-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule, LoaderComponent],
  templateUrl: './cart-page.component.html',
  styleUrls: ['./cart-page.component.css'],})
export class CartPageComponent implements OnInit {
  cartService = inject(CartService);
  orderService = inject(OrderService);
  prescriptionService = inject(PrescriptionService);
  authService = inject(AuthService);
  fb = inject(FormBuilder);
  router = inject(Router);

  categoryImageMap: Record<string, string> = {
    'Pain Relief': '/assets/medicines/pain-relief.png',
    'Antibiotics': '/assets/medicines/antibiotics.png',
    'Vitamins & Supplements': '/assets/medicines/vitamins.png',
    'Digestive Health': '/assets/medicines/digestive-health.png',
    'Allergy & Cold': '/assets/medicines/allergy-cold.png',
    'Skin Care': '/assets/medicines/skin-care.png',
    'Diabetes': '/assets/medicines/diabetes.png',
    'Heart & BP': '/assets/medicines/heart-bp.png'
  };

  getImageSrc(medicine: any): string {
    if (medicine.imageUrl && medicine.imageUrl.startsWith('http')) {
      return medicine.imageUrl;
    }
    return this.categoryImageMap[medicine.category] || '/assets/medicines/pain-relief.png';
  }

  handleImageError(event: any, medicine: any) {
    const fallback = this.categoryImageMap[medicine.category] || '/assets/medicines/pain-relief.png';
    if (event.target.src !== window.location.origin + fallback) {
      event.target.src = fallback;
    }
  }

  approvedPrescriptions: Prescription[] = [];
  placingOrder = false;
  errorMessage = '';

  checkoutForm = this.fb.group({
    shippingAddress: ['', Validators.required],
    prescriptionId: ['']
  });

  ngOnInit() {
    // Basic user info for defaults
    this.authService.currentUser$.subscribe(user => {
      if (user && user.address) {
        this.checkoutForm.patchValue({ shippingAddress: user.address });
      }
    });

    // Check if we need a prescription and update validation
    if (this.cartService.requiresPrescription()) {
      this.checkoutForm.get('prescriptionId')?.setValidators(Validators.required);
      this.loadApprovedPrescriptions();
    }
  }

  loadApprovedPrescriptions() {
    this.prescriptionService.getMyPrescriptions().subscribe({
      next: (data) => {
        this.approvedPrescriptions = data.filter(p => p.status === 'Approved');
      }
    });
  }

  updateQuantity(id: number, qty: number) {
    this.cartService.updateQuantity(id, qty);
    
    // Re-evaluate prescription requirement
    if (this.cartService.requiresPrescription()) {
      this.checkoutForm.get('prescriptionId')?.setValidators(Validators.required);
      if (this.approvedPrescriptions.length === 0) {
        this.loadApprovedPrescriptions();
      }
    } else {
      this.checkoutForm.get('prescriptionId')?.clearValidators();
    }
    this.checkoutForm.get('prescriptionId')?.updateValueAndValidity();
  }

  removeItem(id: number) {
    this.cartService.removeFromCart(id);
    // Update validators after removal
    this.updateQuantity(id, 0); // Hack to trigger validator update
  }

  placeOrder() {
    if (this.checkoutForm.invalid || this.cartItemsCount() === 0) return;

    this.placingOrder = true;
    this.errorMessage = '';

    const items = this.cartService.getCartItems().map(item => ({
      medicineId: item.medicine.id,
      quantity: item.quantity
    }));

    const formVal = this.checkoutForm.value;
    const orderReq: CreateOrder = {
      items: items,
      shippingAddress: formVal.shippingAddress || undefined,
      prescriptionId: formVal.prescriptionId ? parseInt(formVal.prescriptionId, 10) : undefined
    };

    this.orderService.placeOrder(orderReq).subscribe({
      next: (order) => {
        this.cartService.clearCart();
        this.placingOrder = false;
        this.router.navigate(['/order-confirmation', order.id]);
      },
      error: (err) => {
        this.placingOrder = false;
        this.errorMessage = err.error?.message || 'Failed to place order. Try again.';
      }
    });
  }

  cartItemsCount(): number {
    return this.cartService.getItemCount();
  }
}
