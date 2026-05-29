import { useState } from 'react';
import { useProducts } from '../../hooks/useProducts';
import { formatPrice } from '../../lib/utils';
import { Plus, Edit2, Trash2, X } from 'lucide-react';
import toast from 'react-hot-toast';

interface ProductForm {
  name: string; description: string; price: string; imageUrl: string; categoryId: string; sku: string;
}

const emptyForm: ProductForm = { name: '', description: '', price: '', imageUrl: '', categoryId: '', sku: '' };

export default function AdminPage() {
  const { products, categories, createProduct, updateProduct, deleteProduct } = useProducts();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState<ProductForm>(emptyForm);

  const openCreate = () => { setEditingId(null); setForm(emptyForm); setIsModalOpen(true); };

  const openEdit = (id: string) => {
    const product = products.find((p) => p.id === id);
    if (!product) return;
    setEditingId(id);
    setForm({ name: product.name, description: product.description, price: product.price.toString(), imageUrl: product.imageUrl, categoryId: product.categoryId, sku: product.sku });
    setIsModalOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const data = { ...form, price: parseFloat(form.price) };
    if (isNaN(data.price)) { toast.error('Geçerli bir fiyat girin.'); return; }

    let success: boolean;
    if (editingId) {
      success = await updateProduct(editingId, data);
    } else {
      success = await createProduct(data);
    }
    if (success) { setIsModalOpen(false); setForm(emptyForm); setEditingId(null); }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm('Bu ürünü silmek istediğinize emin misiniz?')) return;
    await deleteProduct(id);
  };

  const updateField = (f: keyof ProductForm) => (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) =>
    setForm((p) => ({ ...p, [f]: e.target.value }));

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900">Ürün Yönetimi</h1>
        <button onClick={openCreate} className="btn-primary flex items-center gap-2"><Plus size={20} /> Yeni Ürün</button>
      </div>

      <div className="card overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead><tr className="border-b text-left bg-gray-50"><th className="p-3 font-medium text-gray-500">Ürün</th><th className="p-3 font-medium text-gray-500">SKU</th><th className="p-3 font-medium text-gray-500">Kategori</th><th className="p-3 font-medium text-gray-500">Fiyat</th><th className="p-3 font-medium text-gray-500">Durum</th><th className="p-3 font-medium text-gray-500">İşlemler</th></tr></thead>
            <tbody>
              {products.map((p) => (
                <tr key={p.id} className="border-b last:border-0 hover:bg-gray-50">
                  <td className="p-3"><p className="font-medium">{p.name}</p></td>
                  <td className="p-3 text-gray-500">{p.sku}</td>
                  <td className="p-3">{p.categoryName}</td>
                  <td className="p-3 font-medium">{formatPrice(p.price)}</td>
                  <td className="p-3"><span className={`px-2 py-1 rounded-full text-xs ${p.isPublished ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}`}>{p.isPublished ? 'Yayında' : 'Taslak'}</span></td>
                  <td className="p-3">
                    <div className="flex gap-2">
                      <button onClick={() => openEdit(p.id)} className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg"><Edit2 size={16} /></button>
                      <button onClick={() => handleDelete(p.id)} className="p-2 text-red-600 hover:bg-red-50 rounded-lg"><Trash2 size={16} /></button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        {products.length === 0 && <p className="text-center py-8 text-gray-500">Henüz ürün eklenmemiş.</p>}
      </div>

      {isModalOpen && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4" onClick={() => setIsModalOpen(false)}>
          <div className="bg-white rounded-xl max-w-lg w-full max-h-[90vh] overflow-y-auto p-6" onClick={(e) => e.stopPropagation()}>
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-xl font-bold">{editingId ? 'Ürün Düzenle' : 'Yeni Ürün'}</h2>
              <button onClick={() => setIsModalOpen(false)} className="p-1 hover:bg-gray-100 rounded"><X size={24} /></button>
            </div>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div><label className="block text-sm font-medium text-gray-700 mb-1">Ürün Adı</label><input required value={form.name} onChange={updateField('name')} className="input-field" /></div>
              <div><label className="block text-sm font-medium text-gray-700 mb-1">Açıklama</label><textarea required value={form.description} onChange={updateField('description')} className="input-field" rows={3} /></div>
              <div className="grid grid-cols-2 gap-4">
                <div><label className="block text-sm font-medium text-gray-700 mb-1">Fiyat (₺)</label><input required type="number" step="0.01" value={form.price} onChange={updateField('price')} className="input-field" /></div>
                <div><label className="block text-sm font-medium text-gray-700 mb-1">Kategori</label><select required value={form.categoryId} onChange={updateField('categoryId')} className="input-field"><option value="">Seçiniz</option>{categories.map((c) => (<option key={c.id} value={c.id}>{c.name}</option>))}</select></div>
              </div>
              <div><label className="block text-sm font-medium text-gray-700 mb-1">Görsel URL</label><input value={form.imageUrl} onChange={updateField('imageUrl')} className="input-field" placeholder="https://..." /></div>
              {!editingId && (<div><label className="block text-sm font-medium text-gray-700 mb-1">SKU</label><input required value={form.sku} onChange={updateField('sku')} className="input-field" /></div>)}
              <div className="flex gap-3 pt-2">
                <button type="submit" className="btn-primary flex-1">{editingId ? 'Güncelle' : 'Oluştur'}</button>
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn-secondary flex-1">İptal</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
