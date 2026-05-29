import { useState, useEffect } from 'react';
import { Product, Category } from '../types';
import { productsApi } from '../api/products';
import toast from 'react-hot-toast';

export function useProducts() {
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  const fetchProducts = async () => {
    try {
      setIsLoading(true);
      const data = await productsApi.getAll();
      setProducts(data);
    } catch (error) {
      toast.error('Ürünler yüklenirken bir hata oluştu.');
    } finally {
      setIsLoading(false);
    }
  };

  const fetchCategories = async () => {
    try {
      const data = await productsApi.getCategories();
      setCategories(data);
    } catch (error) {
      toast.error('Kategoriler yüklenirken bir hata oluştu.');
    }
  };

  useEffect(() => {
    fetchProducts();
    fetchCategories();
  }, []);

  const createProduct = async (data: { name: string; description: string; price: number; imageUrl: string; categoryId: string; sku: string }) => {
    try {
      const product = await productsApi.create(data);
      setProducts((prev) => [product, ...prev]);
      toast.success('Ürün başarıyla oluşturuldu!');
      return true;
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Ürün oluşturulurken hata oluştu.');
      return false;
    }
  };

  const updateProduct = async (id: string, data: { name: string; description: string; price: number; imageUrl: string; categoryId: string }) => {
    try {
      const updated = await productsApi.update(id, data);
      setProducts((prev) => prev.map((p) => (p.id === id ? updated : p)));
      toast.success('Ürün başarıyla güncellendi!');
      return true;
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Ürün güncellenirken hata oluştu.');
      return false;
    }
  };

  const deleteProduct = async (id: string) => {
    try {
      await productsApi.delete(id);
      setProducts((prev) => prev.filter((p) => p.id !== id));
      toast.success('Ürün başarıyla silindi!');
      return true;
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Ürün silinirken hata oluştu.');
      return false;
    }
  };

  return { products, categories, isLoading, createProduct, updateProduct, deleteProduct, refresh: fetchProducts };
}
