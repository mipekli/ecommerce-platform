import { Link } from 'react-router-dom';
import { Package } from 'lucide-react';
import { useOrders } from '../../hooks/useOrders';
import { formatPrice, formatDate, getStatusColor, getStatusText } from '../../lib/utils';

export default function OrdersPage() {
  const { orders, isLoading } = useOrders();

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  if (orders.length === 0) {
    return (
      <div className="text-center py-16">
        <Package size={64} className="mx-auto text-gray-300 mb-4" />
        <h2 className="text-xl font-semibold text-gray-900 mb-2">Henüz Siparişiniz Yok</h2>
        <p className="text-gray-500 mb-6">Alışverişe başlayın ve ilk siparişinizi oluşturun.</p>
        <Link to="/products" className="btn-primary">Alışverişe Başla</Link>
      </div>
    );
  }

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Siparişlerim</h1>
      <div className="space-y-4">
        {orders.map((order) => (
          <Link key={order.id} to={`/orders/${order.id}`} className="card block hover:shadow-md transition-shadow">
            <div className="flex justify-between items-start mb-2">
              <div>
                <p className="font-semibold text-gray-900">{order.orderNumber}</p>
                <p className="text-sm text-gray-500">{formatDate(order.createdAt)}</p>
              </div>
              <span className={`px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(order.status)}`}>
                {getStatusText(order.status)}
              </span>
            </div>
            <div className="flex justify-between items-center mt-4 pt-4 border-t">
              <p className="text-sm text-gray-600">{order.items.length} ürün</p>
              <p className="font-bold text-primary-600">{formatPrice(order.totalAmount)}</p>
            </div>
          </Link>
        ))}
      </div>
    </div>
  );
}
