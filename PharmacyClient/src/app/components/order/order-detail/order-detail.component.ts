import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { OrderService } from '../../../services/order.service';
import { Order } from '../../../models/order.model';
import { LoaderComponent } from '../../shared/loader/loader.component';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, LoaderComponent],
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.css'],})
export class OrderDetailComponent implements OnInit {
  route = inject(ActivatedRoute);
  orderService = inject(OrderService);

  order: Order | null = null;
  loading = true;
  cancelling = false;

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));
      if (id) {
        this.loadOrder(id);
      }
    });
  }

  loadOrder(id: number) {
    this.loading = true;
    this.orderService.getOrderById(id).subscribe({
      next: (data) => {
        this.order = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  cancelOrder() {
    if (!this.order) return;
    if (!confirm('Are you sure you want to cancel this order?')) return;
    
    this.cancelling = true;
    this.orderService.cancelOrder(this.order.id).subscribe({
      next: (updatedOrder) => {
        this.order = updatedOrder;
        this.cancelling = false;
      },
      error: (err) => {
        console.error('Failed to cancel order', err);
        alert(err.error?.message || 'Could not cancel order. Please try again.');
        this.cancelling = false;
      }
    });
  }

  isStatusReached(statuses: string[]): boolean {
    if (!this.order) return false;
    
    // Ordered progression maps
    const progression = ['Pending', 'Processing', 'Shipped', 'Delivered'];
    const currentIdx = progression.indexOf(this.order.status);
    
    // If current status is in progression, check if target status is at or before current
    if (currentIdx !== -1) {
       return statuses.some(s => progression.indexOf(s) <= currentIdx);
    }
    
    // For Cancelled/Confirmed/Refunded, check direct match mostly
    // We treat 'Confirmed' as 'Pending' in UI flow mostly, but it means placed
    if (this.order.status === 'Confirmed') {
        const pIdx = progression.indexOf('Pending');
        return statuses.some(s => progression.indexOf(s) <= pIdx);
    }
    
    return false;
  }
}
