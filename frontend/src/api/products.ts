import apiClient from './client';
import { Product, Category } from '../types';

export const productsApi = {
  getAll: async () => {
    const response = await apiClient.get<Product[]>('/products');
    return response.data;
  },

  getById: async (id: string) => {
    const response = await apiClient.get<Product>(`/products/${id}`);
    return response.data;
  },

  search: async (query: string) => {
    const response = await apiClient.get<Product[]>('/products/search', { params: { q: query } });
    return response.data;
  },

  create: async (data: { name: string; description: string; price: number; imageUrl: string; categoryId: string; sku: string }) => {
    const response = await apiClient.post<Product>('/products', data);
    return response.data;
  },

  update: async (id: string, data: { name: string; description: string; price: number; imageUrl: string; categoryId: string }) => {
    const response = await apiClient.put<Product>(`/products/${id}`, data);
    return response.data;
  },

  delete: async (id: string) => {
    await apiClient.delete(`/products/${id}`);
  },

  getCategories: async () => {
    const response = await apiClient.get<Category[]>('/categories');
    return response.data;
  },
};
