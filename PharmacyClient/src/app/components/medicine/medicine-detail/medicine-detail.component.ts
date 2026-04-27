import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MedicineService } from '../../../services/medicine.service';
import { CartService } from '../../../services/cart.service';
import { ToastService } from '../../../services/toast.service';
import { Medicine } from '../../../models/medicine.model';
import { LoaderComponent } from '../../shared/loader/loader.component';

@Component({
  selector: 'app-medicine-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, LoaderComponent],
  templateUrl: './medicine-detail.component.html',
  styleUrls: ['./medicine-detail.component.css'],})
export class MedicineDetailComponent implements OnInit {
  route = inject(ActivatedRoute);
  router = inject(Router);
  medicineService = inject(MedicineService);
  cartService = inject(CartService);
  toastService = inject(ToastService);

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

  getImageSrc(): string {
    if (!this.medicine) return '/assets/medicines/pain-relief.png';
    if (this.medicine.imageUrl && this.medicine.imageUrl.startsWith('http')) {
      return this.medicine.imageUrl;
    }
    return this.categoryImageMap[this.medicine.category] || '/assets/medicines/pain-relief.png';
  }

  handleImageError(event: any) {
    if (!this.medicine) return;
    const fallback = this.categoryImageMap[this.medicine.category] || '/assets/medicines/pain-relief.png';
    if (event.target.src !== window.location.origin + fallback) {
      event.target.src = fallback;
    }
  }

  medicine: Medicine | null = null;
  loading = true;
  quantity = 1;
  imgFailed = false;
  Math = Math;

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));
      if (id) {
        this.loadMedicine(id);
      } else {
        this.loading = false;
      }
    });
  }

  loadMedicine(id: number) {
    this.loading = true;
    this.imgFailed = false;
    this.medicineService.getById(id).subscribe({
      next: (data) => {
        this.medicine = data;
        this.loading = false;
      },
      error: () => {
        this.medicine = null;
        this.loading = false;
      }
    });
  }

  incrementQty() {
    if (this.medicine && this.quantity < Math.min(this.medicine.stockQuantity, 10)) {
      this.quantity++;
    }
  }

  decrementQty() {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  addToCart() {
    if (this.medicine && this.medicine.stockQuantity >= this.quantity) {
      this.cartService.addToCart(this.medicine, this.quantity);
      this.toastService.success(`${this.medicine.name} (x${this.quantity}) added to cart`);
      this.router.navigate(['/cart']);
    }
  }
}
