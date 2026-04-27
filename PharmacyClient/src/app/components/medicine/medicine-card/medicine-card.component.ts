import { Component, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Medicine } from '../../../models/medicine.model';
import { CartService } from '../../../services/cart.service';
import { ToastService } from '../../../services/toast.service';

@Component({
  selector: 'app-medicine-card',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './medicine-card.component.html',
  styleUrls: ['./medicine-card.component.css'],})
export class MedicineCardComponent {
  @Input() medicine!: Medicine;
  cartService = inject(CartService);
  toastService = inject(ToastService);

  private categoryImageMap: Record<string, string> = {
    'Pain Relief': '/assets/medicines/pain-relief.png',
    'Antibiotics': '/assets/medicines/antibiotics.png',
    'Vitamins & Supplements': '/assets/medicines/vitamins.png',
    'Digestive Health': '/assets/medicines/digestive-health.png',
    'Allergy & Cold': '/assets/medicines/allergy-cold.png',
    'Skin Care': '/assets/medicines/skin-care.png',
    'Diabetes': '/assets/medicines/diabetes.png',
    'Heart & BP': '/assets/medicines/heart-bp.png'
  };

  getImageSrc(): string {
    if (this.medicine.imageUrl && this.medicine.imageUrl.startsWith('http')) {
      return this.medicine.imageUrl;
    }
    return this.categoryImageMap[this.medicine.category] || '/assets/medicines/pain-relief.png';
  }

  handleImageError(event: any) {
    const fallback = this.categoryImageMap[this.medicine.category] || '/assets/medicines/pain-relief.png';
    if (event.target.src !== window.location.origin + fallback) {
      event.target.src = fallback;
    }
  }

  addToCart(event: Event) {
    event.stopPropagation();
    if (this.medicine.isAvailable && this.medicine.stockQuantity > 0) {
      this.cartService.addToCart(this.medicine, 1);
      this.toastService.success(`${this.medicine.name} added to cart`);
    }
  }
}
