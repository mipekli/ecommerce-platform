import { Link } from 'react-router-dom';
import { Trash2, Minus, Plus, ShoppingBag } from 'lucide-react';
import { useCartStore } from '../../stores/cartStore';
import { formatPrice } from '../../lib/utils';

export default function CartPage() {
  const { items, removeItem, updateQuantity, getTotal } = useCartStore();

  if (items.length === 0) {
    return (
      <div className="text-center py-16">
        <ShoppingBag size={64} className="mx-auto text-gray-300 mb-4" />
        <h2 className="text-xl font-semibold text-gray-900 mb-2">Sepetiniz Boş</h2>
        <p className="text-gray-500 mb-6">Alışverişe başlamak için ürünleri keşfedin.</p>
        <Link to="/products" className="btn-primary">Alışverişe Başla</Link>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto">
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Sepetim</h1>
      <div className="space-y-4">
        {items.map((item) => (
          <div key={item.productId} className="card flex items-center gap-4">
            <div className="w-20 h-20 bg-gray-100 rounded-lg overflow-hidden flex-shrink-0">
              {item.imageUrl ? <img src={item.imageUrl} alt={item.productName} className="w-full h-full object-cover" /> : <div className="w-full h-full flex items-center justify-center text-gray-400">Img</div>}
            </div>
            <div className="flex-1 min-w-0">
              <h3 className="font-semibold text-gray-900 truncate">{item.productName}</h3>
              <p className="text-primary-600 font-medium">{formatPrice(item.unitPrice)}</p>
            </div>
            <div className="flex items-center gap-2">
              <button onClick={() => item.quantity > 1 && updateQuantity(item.productId, item.quantity - 1)} className="p-1 rounded hover:bg-gray-100"><Minus size={18} /></button>
              <span className="w-8 text-center font-medium">{item.quantity}</span>
              <button onClick={() => updateQuantity(item.productId, item.quantity + 1)} className="p-1 rounded hover:bg-gray-100"><Plus size={18} /></button>
            </div>
            <p className="font-bold text-gray-900 w-24 text-right">{formatPrice(item.unitPrice * item.quantity)}</p>
            <button onClick={() => removeItem(item.productId)} className="p-2 text-red-500 hover:bg-red-50 rounded-lg transition-colors"><Trash2 size={20} /></button>
          </div>
        ))}
      </div>
      <div className="card mt-6">
        <div className="flex justify-between items-center mb-4">
          <span className="text-lg font-semibold">Toplam</span>
          <span className="text-2xl font-bold text-primary-600">{formatPrice(getTotal())}</span>
        </div>
        <Link to="/checkout" className="btn-primary w-full block text-center">Siparişi Tamamla</Link>
      </div>
    </div>
  );
}
