import { useOrders } from '../../hooks/useOrders';
import { formatPrice, getStatusText } from '../../lib/utils';
import { Package, Clock, CheckCircle, XCircle, TrendingUp } from 'lucide-react';

export default function DashboardPage() {
  const { orders, isLoading } = useOrders();

  const stats = {
    total: orders.length,
    pending: orders.filter((o) => o.status === 'Pending').length,
    delivered: orders.filter((o) => o.status === 'Delivered').length,
    cancelled: orders.filter((o) => o.status === 'Cancelled').length,
    totalSpent: orders.reduce((sum, o) => sum + o.totalAmount, 0),
  };

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Panelim</h1>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div className="card flex items-center gap-4">
          <div className="p-3 bg-blue-100 rounded-lg"><Package className="text-blue-600" size={24} /></div>
          <div><p className="text-sm text-gray-500">Toplam Sipariş</p><p className="text-2xl font-bold">{stats.total}</p></div>
        </div>
        <div className="card flex items-center gap-4">
          <div className="p-3 bg-yellow-100 rounded-lg"><Clock className="text-yellow-600" size={24} /></div>
          <div><p className="text-sm text-gray-500">Bekleyen</p><p className="text-2xl font-bold">{stats.pending}</p></div>
        </div>
        <div className="card flex items-center gap-4">
          <div className="p-3 bg-green-100 rounded-lg"><CheckCircle className="text-green-600" size={24} /></div>
          <div><p className="text-sm text-gray-500">Teslim Edilen</p><p className="text-2xl font-bold">{stats.delivered}</p></div>
        </div>
        <div className="card flex items-center gap-4">
          <div className="p-3 bg-purple-100 rounded-lg"><TrendingUp className="text-purple-600" size={24} /></div>
          <div><p className="text-sm text-gray-500">Toplam Harcama</p><p className="text-2xl font-bold">{formatPrice(stats.totalSpent)}</p></div>
        </div>
      </div>
      <div className="card">
        <h2 className="font-semibold text-lg mb-4">Son Siparişler</h2>
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead><tr className="border-b text-left"><th className="pb-3 font-medium text-gray-500">Sipariş No</th><th className="pb-3 font-medium text-gray-500">Durum</th><th className="pb-3 font-medium text-gray-500">Tutar</th><th className="pb-3 font-medium text-gray-500">Tarih</th></tr></thead>
            <tbody>
              {orders.slice(0, 10).map((o) => (
                <tr key={o.id} className="border-b last:border-0">
                  <td className="py-3 font-medium">{o.orderNumber}</td>
                  <td className="py-3">{getStatusText(o.status)}</td>
                  <td className="py-3">{formatPrice(o.totalAmount)}</td>
                  <td className="py-3 text-gray-500">{new Date(o.createdAt).toLocaleDateString('tr-TR')}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        {orders.length === 0 && <p className="text-gray-500 text-center py-4">Henüz siparişiniz bulunmuyor.</p>}
      </div>
    </div>
  );
}
