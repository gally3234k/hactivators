export interface Medicine {
  id: number;
  name: string;
  description?: string;
  category: string;
  price: number;
  stockQuantity: number;
  imageUrl?: string;
  requiresPrescription: boolean;
  isAvailable: boolean;
  dosage?: string;
  packaging?: string;
  manufacturer?: string;
}

export interface CreateMedicine {
  name: string;
  description?: string;
  category: string;
  price: number;
  stockQuantity: number;
  imageUrl?: string;
  requiresPrescription: boolean;
  dosage?: string;
  packaging?: string;
  manufacturer?: string;
}
