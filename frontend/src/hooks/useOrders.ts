import { useState, useEffect } from 'react';
import { Order } from '../types';
import { ordersApi } from '../api/orders';
import toast from 'react-hot-toast';

export function useOrders() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  const fetchOrders = async () => {
    try {
      setIsLoading(true);
      const data = await ordersApi.getMyOrders();
      setOrders(data);
    } catch (error) {
      toast.error('Siparişler yüklenirken bir hata oluştu.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchOrders();
  }, []);

  const createOrder = async (data: {
    items: { productId: string; productName: string; unitPrice: number; quantity: number }[];
    shippingAddress: { street: string; city: string; state: string; zipCode: string; country: string };
    billingAddress: { street: string; city: string; state: string; zipCode: string; country: string };
    notes?: string;
  }) => {
    try {
      const order = await ordersApi.create(data);
      setOrders((prev) => [order, ...prev]);
      toast.success('Sipariş başarıyla oluşturuldu!');
      return order;
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Sipariş oluşturulurken hata oluştu.');
      return null;
    }
  };

  const cancelOrder = async (id: string) => {
    try {
      await ordersApi.cancel(id);
      setOrders((prev) => prev.map((o) => (o.id === id ? { ...o, status: 'Cancelled' } : o)));
      toast.success('Sipariş iptal edildi.');
      return true;
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Sipariş iptal edilirken hata oluştu.');
      return false;
    }
  };

  return { orders, isLoading, createOrder, cancelOrder, refresh: fetchOrders };
}
