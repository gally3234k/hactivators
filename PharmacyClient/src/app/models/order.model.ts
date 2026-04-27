export interface Order {
  id: number;
  userId: number;
  userName: string;
  userEmail: string;
  prescriptionId?: number;
  totalAmount: number;
  status: string;
  shippingAddress?: string;
  createdAt: string;
  updatedAt: string;
  items: OrderItem[];
  payment?: Payment;
}

export interface OrderItem {
  id: number;
  medicineId: number;
  medicineName: string;
  medicineImageUrl?: string;
  quantity: number;
  unitPrice: number;
}

export interface Payment {
  id: number;
  orderId: number;
  amount: number;
  status: string;
  method?: string;
  transactionId?: string;
  paidAt?: string;
}

export interface CreateOrder {
  items: { medicineId: number; quantity: number }[];
  prescriptionId?: number;
  shippingAddress?: string;
}

export interface Dashboard {
  totalOrdersToday: number;
  totalRevenue: number;
  pendingPrescriptions: number;
  lowStockMedicines: number;
  activeCustomers: number;
}

export interface InventoryAlert {
  medicineId: number;
  medicineName: string;
  category: string;
  stockQuantity: number;
}
