import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { OrderService } from '../../../services/order.service';
import { Order } from '../../../models/order.model';
import { LoaderComponent } from '../../shared/loader/loader.component';
import { CartService } from '../../../services/cart.service';
import { MedicineService } from '../../../services/medicine.service';
import { forkJoin } from 'rxjs';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-order-history',
  standalone: true,
  imports: [CommonModule, RouterModule, LoaderComponent],
  templateUrl: './order-history.component.html',
  styleUrls: ['./order-history.component.css'],})
export class OrderHistoryComponent implements OnInit {
  orderService = inject(OrderService);
  cartService = inject(CartService);
  medicineService = inject(MedicineService);
  router = inject(Router);

  orders: Order[] = [];
  loading = true;
  reorderingOrderId: number | null = null;
  cancellingOrderId: number | null = null;

  ngOnInit() {
    this.loadOrders();
  }

  loadOrders() {
    this.orderService.getMyOrders().subscribe({
      next: (data) => {
        // Sort newest first
        this.orders = data.sort((a, b) => 
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  cancelOrder(id: number) {
    if (!confirm('Are you sure you want to cancel this order?')) return;
    
    this.cancellingOrderId = id;
    this.orderService.cancelOrder(id).subscribe({
      next: (updatedOrder) => {
        // Update the order in the list
        const index = this.orders.findIndex(o => o.id === id);
        if (index !== -1) {
          this.orders[index] = updatedOrder;
        }
        this.cancellingOrderId = null;
      },
      error: (err) => {
        console.error('Failed to cancel order', err);
        alert(err.error?.message || 'Could not cancel order. Please try again.');
        this.cancellingOrderId = null;
      }
    });
  }

  reorder(order: Order) {
    if (!order.items || order.items.length === 0) return;
    
    this.reorderingOrderId = order.id;
    
    // Fetch full medicine details for all items
    const observables = order.items.map(item => 
      this.medicineService.getById(item.medicineId)
    );
    
    forkJoin(observables).pipe(
      finalize(() => this.reorderingOrderId = null)
    ).subscribe({
      next: (medicines) => {
        order.items.forEach((item, index) => {
          const medicine = medicines[index];
          // Add to cart if medicine is still available
          if (medicine && medicine.isAvailable) {
            this.cartService.addToCart(medicine, item.quantity);
          }
        });
        
        // Navigate to cart for checkout
        this.router.navigate(['/cart']);
      },
      error: (err) => {
        console.error('Failed to fetch medicine details for reorder', err);
        alert('Could not process reorder. Some items may no longer be available.');
      }
    });
  }
}
