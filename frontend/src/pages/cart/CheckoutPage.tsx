import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useCartStore } from '../../stores/cartStore';
import { useOrders } from '../../hooks/useOrders';
import { formatPrice } from '../../lib/utils';

export default function CheckoutPage() {
  const { items, getTotal, clearCart } = useCartStore();
  const { createOrder } = useOrders();
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [form, setForm] = useState({
    street: '', city: '', state: '', zipCode: '', country: 'Türkiye',
    bStreet: '', bCity: '', bState: '', bZipCode: '', bCountry: 'Türkiye',
    notes: '',
  });

  const update = (f: string) => (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) =>
    setForm((p) => ({ ...p, [f]: e.target.value }));

  const sameAddress = form.street === form.bStreet && form.city === form.bCity;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (items.length === 0) return;
    setIsLoading(true);

    const orderData = {
      items: items.map((i) => ({ productId: i.productId, productName: i.productName, unitPrice: i.unitPrice, quantity: i.quantity })),
      shippingAddress: { street: form.street, city: form.city, state: form.state, zipCode: form.zipCode, country: form.country },
      billingAddress: { street: form.bStreet || form.street, city: form.bCity || form.city, state: form.bState || form.state, zipCode: form.bZipCode || form.zipCode, country: form.bCountry || form.country },
      notes: form.notes || undefined,
    };

    const order = await createOrder(orderData);
    setIsLoading(false);
    if (order) {
      clearCart();
      navigate(`/orders/${order.id}`);
    }
  };

  if (items.length === 0) {
    navigate('/cart');
    return null;
  }

  return (
    <div className="max-w-4xl mx-auto">
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Siparişi Tamamla</h1>
      <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="card space-y-4">
          <h2 className="font-semibold text-lg">Teslimat Adresi</h2>
          <input required placeholder="Cadde/Sokak" value={form.street} onChange={update('street')} className="input-field" />
          <div className="grid grid-cols-2 gap-4">
            <input required placeholder="Şehir" value={form.city} onChange={update('city')} className="input-field" />
            <input required placeholder="İlçe" value={form.state} onChange={update('state')} className="input-field" />
          </div>
          <div className="grid grid-cols-2 gap-4">
            <input required placeholder="Posta Kodu" value={form.zipCode} onChange={update('zipCode')} className="input-field" />
            <input required placeholder="Ülke" value={form.country} onChange={update('country')} className="input-field" />
          </div>
        </div>
        <div className="card space-y-4">
          <h2 className="font-semibold text-lg">Fatura Adresi</h2>
          <input required placeholder="Cadde/Sokak" value={form.bStreet} onChange={update('bStreet')} className="input-field" />
          <div className="grid grid-cols-2 gap-4">
            <input required placeholder="Şehir" value={form.bCity} onChange={update('bCity')} className="input-field" />
            <input required placeholder="İlçe" value={form.bState} onChange={update('bState')} className="input-field" />
          </div>
          <div className="grid grid-cols-2 gap-4">
            <input required placeholder="Posta Kodu" value={form.bZipCode} onChange={update('bZipCode')} className="input-field" />
            <input required placeholder="Ülke" value={form.bCountry} onChange={update('bCountry')} className="input-field" />
          </div>
          {!sameAddress && <p className="text-sm text-yellow-600">Fatura adresiniz teslimat adresinizden farklı.</p>}
        </div>
        <div className="md:col-span-2 card space-y-4">
          <h2 className="font-semibold text-lg">Sipariş Özeti</h2>
          <div className="space-y-2">
            {items.map((i) => (
              <div key={i.productId} className="flex justify-between text-sm">
                <span>{i.productName} x {i.quantity}</span>
                <span>{formatPrice(i.unitPrice * i.quantity)}</span>
              </div>
            ))}
          </div>
          <div className="border-t pt-4 flex justify-between items-center">
            <span className="font-bold text-lg">Toplam</span>
            <span className="font-bold text-2xl text-primary-600">{formatPrice(getTotal())}</span>
          </div>
          <textarea placeholder="Sipariş notu (isteğe bağlı)" value={form.notes} onChange={update('notes')} className="input-field" rows={3} />
          <button type="submit" disabled={isLoading} className="btn-primary w-full py-3 text-lg">
            {isLoading ? 'Sipariş oluşturuluyor...' : `Siparişi Tamamla (${formatPrice(getTotal())})`}
          </button>
        </div>
      </form>
    </div>
  );
}
