import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { OrderService } from '../../../services/order.service';
import { Order, Payment } from '../../../models/order.model';
import { LoaderComponent } from '../../shared/loader/loader.component';

@Component({
  selector: 'app-order-confirmation',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, LoaderComponent],
  templateUrl: './order-confirmation.component.html',
  styleUrls: ['./order-confirmation.component.css'],})
export class OrderConfirmationComponent implements OnInit {
  route = inject(ActivatedRoute);
  orderService = inject(OrderService);
  fb = inject(FormBuilder);
  router = inject(Router);

  order: Order | null = null;
  loading = true;
  processingPayment = false;
  paymentError = '';

  paymentForm = this.fb.group({
    method: ['Card', Validators.required]
  });

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));
      if (id) {
        this.loadOrder(id);
      } else {
        this.loading = false;
        this.router.navigate(['/']);
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
        this.router.navigate(['/orders']);
      }
    });
  }

  processPayment() {
    if (this.paymentForm.invalid || !this.order) return;

    this.processingPayment = true;
    this.paymentError = '';
    const method = this.paymentForm.value.method || 'Card';

    this.orderService.processPayment(this.order.id, method).subscribe({
      next: (payment) => {
        if (this.order) {
          this.order.payment = payment;
          this.order.status = 'Confirmed'; // Local status update
        }
        this.processingPayment = false;
      },
      error: (err) => {
        this.paymentError = err.error?.message || 'Payment simulation failed.';
        this.processingPayment = false;
      }
    });
  }
}
