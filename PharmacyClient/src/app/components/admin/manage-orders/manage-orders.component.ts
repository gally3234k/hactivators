import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { OrderService } from '../../../services/order.service';
import { Order } from '../../../models/order.model';
import { LoaderComponent } from '../../shared/loader/loader.component';

@Component({
  selector: 'app-manage-orders',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, LoaderComponent],
  templateUrl: './manage-orders.component.html',
  styleUrls: ['./manage-orders.component.css'],})
export class ManageOrdersComponent implements OnInit {
  orderService = inject(OrderService);
  
  orders: Order[] = [];
  loading = true;
  processingId: number | null = null;

  ngOnInit() {
    this.loadOrders();
  }

  loadOrders() {
    this.loading = true;
    this.orderService.getAllOrders().subscribe({
      next: (data) => {
        this.orders = data.sort((a,b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  updateStatus(order: Order, newStatus: string) {
    if (order.status === newStatus) return;
    
    this.processingId = order.id;
    this.orderService.updateOrderStatus(order.id, newStatus).subscribe({
      next: (updated) => {
        order.status = updated.status;
        this.processingId = null;
      },
      error: () => {
        alert('Failed to update status');
        this.processingId = null;
        // Revert UI selection implicitly on failure if handled carefully, 
        // but here we just let the user see it failed and they can trace back.
        this.loadOrders();
      }
    });
  }
}
