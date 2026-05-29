import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { ArrowLeft } from 'lucide-react';
import { ordersApi } from '../../api/orders';
import { Order } from '../../types';
import { formatPrice, formatDate, getStatusColor, getStatusText } from '../../lib/utils';
import { useOrders } from '../../hooks/useOrders';
import toast from 'react-hot-toast';

export default function OrderDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [order, setOrder] = useState<Order | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const { cancelOrder } = useOrders();

  useEffect(() => {
    if (!id) return;
    ordersApi.getById(id).then(setOrder).catch(() => toast.error('Sipariş bulunamadı.')).finally(() => setIsLoading(false));
  }, [id]);

  const handleCancel = async () => {
    if (!id) return;
    const success = await cancelOrder(id);
    if (success) setOrder((prev) => prev ? { ...prev, status: 'Cancelled' } : null);
  };

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  if (!order) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500 mb-4">Sipariş bulunamadı.</p>
        <Link to="/orders" className="btn-primary">Siparişlerime Dön</Link>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto">
      <Link to="/orders" className="inline-flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-6">
        <ArrowLeft size={20} /> Siparişlerime Dön
      </Link>
      <div className="card mb-6">
        <div className="flex justify-between items-start mb-4">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">{order.orderNumber}</h1>
            <p className="text-gray-500">{formatDate(order.createdAt)}</p>
          </div>
          <span className={`px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(order.status)}`}>
            {getStatusText(order.status)}
          </span>
        </div>
        {order.status === 'Pending' && (
          <button onClick={handleCancel} className="btn-danger">Siparişi İptal Et</button>
        )}
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
        <div className="card">
          <h2 className="font-semibold mb-2">Teslimat Adresi</h2>
          <p className="text-gray-600">{order.shippingAddress.street}</p>
          <p className="text-gray-600">{order.shippingAddress.city}, {order.shippingAddress.state} {order.shippingAddress.zipCode}</p>
          <p className="text-gray-600">{order.shippingAddress.country}</p>
        </div>
        <div className="card">
          <h2 className="font-semibold mb-2">Fatura Adresi</h2>
          <p className="text-gray-600">{order.billingAddress.street}</p>
          <p className="text-gray-600">{order.billingAddress.city}, {order.billingAddress.state} {order.billingAddress.zipCode}</p>
          <p className="text-gray-600">{order.billingAddress.country}</p>
        </div>
      </div>
      <div className="card">
        <h2 className="font-semibold mb-4">Sipariş Kalemleri</h2>
        <div className="space-y-3">
          {order.items.map((item) => (
            <div key={item.id} className="flex justify-between items-center py-2 border-b last:border-0">
              <div>
                <p className="font-medium text-gray-900">{item.productName}</p>
                <p className="text-sm text-gray-500">{item.quantity} x {formatPrice(item.unitPrice)}</p>
              </div>
              <p className="font-bold">{formatPrice(item.totalPrice)}</p>
            </div>
          ))}
        </div>
        <div className="border-t mt-4 pt-4 space-y-1">
          <div className="flex justify-between text-gray-600"><span>Ara Toplam</span><span>{formatPrice(order.subTotal)}</span></div>
          <div className="flex justify-between text-gray-600"><span>KDV (%18)</span><span>{formatPrice(order.taxAmount)}</span></div>
          <div className="flex justify-between text-gray-600"><span>Kargo</span><span>{formatPrice(order.shippingCost)}</span></div>
          <div className="flex justify-between text-lg font-bold text-primary-600 border-t pt-2">
            <span>Toplam</span><span>{formatPrice(order.totalAmount)}</span>
          </div>
        </div>
      </div>
    </div>
  );
}
