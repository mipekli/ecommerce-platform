import apiClient from './client';
import { Order } from '../types';

export const ordersApi = {
  getMyOrders: async () => {
    const response = await apiClient.get<Order[]>('/orders');
    return response.data;
  },

  getById: async (id: string) => {
    const response = await apiClient.get<Order>(`/orders/${id}`);
    return response.data;
  },

  create: async (data: {
    items: { productId: string; productName: string; unitPrice: number; quantity: number }[];
    shippingAddress: { street: string; city: string; state: string; zipCode: string; country: string };
    billingAddress: { street: string; city: string; state: string; zipCode: string; country: string };
    notes?: string;
  }) => {
    const response = await apiClient.post<Order>('/orders', data);
    return response.data;
  },

  confirm: async (id: string) => {
    const response = await apiClient.put<Order>(`/orders/${id}/confirm`);
    return response.data;
  },

  cancel: async (id: string) => {
    await apiClient.put(`/orders/${id}/cancel`);
  },
};
