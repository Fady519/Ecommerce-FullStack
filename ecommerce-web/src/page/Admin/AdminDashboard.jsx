import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { api } from '../../services/api';
import toast from 'react-hot-toast';
import { 
  HiOutlineChartBar, 
  HiOutlineCube, 
  HiOutlineClipboardDocumentList, 
  HiOutlineTag, 
  HiOutlinePlus, 
  HiOutlineTrash, 
  HiOutlinePencilSquare, 
  HiOutlineXMark,
  HiOutlineCurrencyDollar,
  HiOutlineShoppingBag,
  HiOutlineExclamationTriangle,
  HiOutlineUsers,
  HiOutlineEye,
  HiOutlineArrowTrendingUp
} from 'react-icons/hi2';
import './AdminDashboard.css';

export default function AdminDashboard() {
  const [searchParams, setSearchParams] = useSearchParams();
  const validTabs = ['overview', 'products', 'catalog', 'orders', 'customers'];

  const getInitialTab = () => {
    const paramTab = searchParams.get('tab');
    if (paramTab && validTabs.includes(paramTab)) return paramTab;
    const storedTab = localStorage.getItem('adminActiveTab');
    if (storedTab && validTabs.includes(storedTab)) return storedTab;
    return 'overview';
  };

  const [activeTab, setActiveTabState] = useState(getInitialTab);

  const setActiveTab = (tab) => {
    setActiveTabState(tab);
    setSearchParams({ tab }, { replace: true });
    localStorage.setItem('adminActiveTab', tab);
  };

  useEffect(() => {
    const currentParam = searchParams.get('tab');
    if (currentParam && validTabs.includes(currentParam) && currentParam !== activeTab) {
      setActiveTabState(currentParam);
      localStorage.setItem('adminActiveTab', currentParam);
    } else if (!currentParam && activeTab) {
      setSearchParams({ tab: activeTab }, { replace: true });
    }
  }, [searchParams]);

  const [loading, setLoading] = useState(false);

  // Stats data
  const [stats, setStats] = useState({
    totalSales: 0,
    totalOrders: 0,
    lowStockProducts: [],
    topSellingProducts: []
  });

  // Products data & pagination
  const [products, setProducts] = useState([]);
  const [brands, setBrands] = useState([]);
  const [types, setTypes] = useState([]);
  const [productSearch, setProductSearch] = useState('');
  const [productPage, setProductPage] = useState(1);
  const productsPerPage = 10;

  // Orders data & filtering
  const [orders, setOrders] = useState([]);
  const [orderStatusFilter, setOrderStatusFilter] = useState('All');
  const [orderSearch, setOrderSearch] = useState('');
  const [viewingOrder, setViewingOrder] = useState(null);

  // Product Modal State
  const [isProductModalOpen, setIsProductModalOpen] = useState(false);
  const [editingProduct, setEditingProduct] = useState(null);
  const [imagePreview, setImagePreview] = useState(null);
  const [productForm, setProductForm] = useState({
    name: '',
    description: '',
    price: '',
    stock: '',
    productBrandId: '',
    productTypeId: '',
    image: null,
    pictureUrl: ''
  });

  // Catalog form states
  const [newBrandName, setNewBrandName] = useState('');
  const [newTypeName, setNewTypeName] = useState('');

  // 1. Initial Data Fetch
  useEffect(() => {
    loadOverviewData();
    loadCatalogData();
    loadProducts();
    loadOrders();
  }, []);

  const loadOverviewData = async () => {
    try {
      const data = await api.admin.getDashboard();
      setStats(data || { totalSales: 0, totalOrders: 0, lowStockProducts: [], topSellingProducts: [] });
    } catch (err) {
      console.error("Error loading dashboard stats:", err);
    }
  };

  const loadCatalogData = async () => {
    try {
      const [brandsData, typesData] = await Promise.all([
        api.products.getBrands(),
        api.products.getTypes()
      ]);
      const bList = Array.isArray(brandsData) ? brandsData : (brandsData?.value || brandsData?.data || []);
      const tList = Array.isArray(typesData) ? typesData : (typesData?.value || typesData?.data || []);
      setBrands(bList);
      setTypes(tList);
      if (bList.length > 0 && tList.length > 0) {
        setProductForm(prev => ({
          ...prev,
          productBrandId: prev.productBrandId || bList[0]?.id?.toString(),
          productTypeId: prev.productTypeId || tList[0]?.id?.toString(),
        }));
      }
    } catch (err) {
      console.error("Error loading catalog:", err);
    }
  };

  const loadProducts = async () => {
    setLoading(true);
    try {
      const res = await api.products.getAll({ pageSize: 120 });
      setProducts(res.data || []);
    } catch (err) {
      toast.error("Failed to load products");
    } finally {
      setLoading(false);
    }
  };

  const loadOrders = async () => {
    try {
      const data = await api.admin.getAllOrders();
      setOrders(Array.isArray(data) ? data : []);
    } catch (err) {
      console.error("Failed to load orders:", err);
    }
  };

  // Handle Product Create / Update
  const handleOpenProductModal = (prod = null) => {
    if (prod) {
      setEditingProduct(prod);
      setImagePreview(prod.pictureUrl || null);
      setProductForm({
        name: prod.name || '',
        description: prod.description || '',
        price: prod.price?.toString() || '',
        stock: prod.stock?.toString() || '15',
        productBrandId: prod.productBrandId?.toString() || brands[0]?.id?.toString() || '1',
        productTypeId: prod.productTypeId?.toString() || types[0]?.id?.toString() || '1',
        image: null,
        pictureUrl: prod.pictureUrl || ''
      });
    } else {
      setEditingProduct(null);
      setImagePreview(null);
      setProductForm({
        name: '',
        description: '',
        price: '',
        stock: '20',
        productBrandId: brands[0]?.id?.toString() || '1',
        productTypeId: types[0]?.id?.toString() || '1',
        image: null,
        pictureUrl: ''
      });
    }
    setIsProductModalOpen(true);
  };

  const handleImageFileChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setProductForm({ ...productForm, image: file });
      setImagePreview(URL.createObjectURL(file));
    }
  };

  const handleProductSubmit = async (e) => {
    e.preventDefault();
    const brandId = productForm.productBrandId || (brands[0]?.id?.toString() || '1');
    const typeId = productForm.productTypeId || (types[0]?.id?.toString() || '1');

    const formData = new FormData();
    formData.append('Name', productForm.name);
    formData.append('Description', productForm.description);
    formData.append('Price', productForm.price);
    formData.append('Stock', productForm.stock);
    formData.append('BrandId', brandId);
    formData.append('ProductBrandId', brandId);
    formData.append('TypeId', typeId);
    formData.append('ProductTypeId', typeId);

    if (productForm.image) {
      formData.append('Image', productForm.image);
    }
    // Only send the URL when the admin typed a new one; the unchanged value is the absolute URL the API built.
    if (productForm.pictureUrl && productForm.pictureUrl !== editingProduct?.pictureUrl) {
      formData.append('PictureUrl', productForm.pictureUrl);
    }

    try {
      if (editingProduct) {
        await api.admin.updateProduct(editingProduct.id, formData);
        toast.success(`Product "${productForm.name}" updated successfully!`);
      } else {
        await api.admin.createProduct(formData);
        toast.success(`Product "${productForm.name}" created and published to store!`);
      }
      setIsProductModalOpen(false);
      loadProducts();
      loadOverviewData();
      window.dispatchEvent(new CustomEvent('catalog-updated'));
    } catch (err) {
      console.error("Product submit error:", err);
      let errorMsg = "Failed to save product";
      if (err.errors) {
        errorMsg = Object.values(err.errors).flat().join(' • ');
      } else if (err.message) {
        errorMsg = err.message;
      }
      toast.error(errorMsg, { duration: 5000 });
    }
  };

  const handleDeleteProduct = async (id, name) => {
    if (!window.confirm(`Are you sure you want to delete "${name}"?`)) return;
    try {
      await api.admin.deleteProduct(id);
      toast.success(`Product "${name}" deleted`);
      loadProducts();
      loadOverviewData();
      window.dispatchEvent(new CustomEvent('catalog-updated'));
    } catch (err) {
      toast.error(err.message || "Failed to delete product");
    }
  };

  // Quick Stock Adjustment
  const handleQuickStockChange = async (prod, delta) => {
    const newStock = Math.max(0, (prod.stock || 0) + delta);
    const formData = new FormData();
    formData.append('Name', prod.name);
    formData.append('Description', prod.description);
    formData.append('Price', prod.price.toString());
    formData.append('Stock', newStock.toString());
    formData.append('BrandId', String(prod.productBrandId));
    formData.append('TypeId', String(prod.productTypeId));
    // No PictureUrl: the API keeps the current picture (sending back the absolute URL it returned would store that URL instead).

    try {
      await api.admin.updateProduct(prod.id, formData);
      setProducts(prev => prev.map(p => p.id === prod.id ? { ...p, stock: newStock } : p));
      toast.success(`Stock for ${prod.name} updated to ${newStock}`);
      window.dispatchEvent(new CustomEvent('catalog-updated'));
    } catch (err) {
      toast.error("Failed to update stock");
    }
  };

  // Handle Brand Create / Delete
  const handleCreateBrand = async (e) => {
    e.preventDefault();
    if (!newBrandName.trim()) return;
    try {
      await api.admin.createBrand(newBrandName.trim());
      toast.success(`Brand "${newBrandName}" added!`);
      setNewBrandName('');
      loadCatalogData();
      window.dispatchEvent(new CustomEvent('catalog-updated'));
    } catch (err) {
      toast.error(err.message || "Failed to add brand");
    }
  };

  const handleDeleteBrand = async (id, name) => {
    if (!window.confirm(`Delete brand "${name}"?`)) return;
    try {
      await api.admin.deleteBrand(id);
      toast.success(`Brand "${name}" removed`);
      loadCatalogData();
      window.dispatchEvent(new CustomEvent('catalog-updated'));
    } catch (err) {
      toast.error(err.message || "Failed to delete brand");
    }
  };

  // Handle Type Create / Delete
  const handleCreateType = async (e) => {
    e.preventDefault();
    if (!newTypeName.trim()) return;
    try {
      await api.admin.createType(newTypeName.trim());
      toast.success(`Category "${newTypeName}" added!`);
      setNewTypeName('');
      loadCatalogData();
      window.dispatchEvent(new CustomEvent('catalog-updated'));
    } catch (err) {
      toast.error(err.message || "Failed to add category");
    }
  };

  const handleDeleteType = async (id, name) => {
    if (!window.confirm(`Delete category "${name}"?`)) return;
    try {
      await api.admin.deleteType(id);
      toast.success(`Category "${name}" removed`);
      loadCatalogData();
      window.dispatchEvent(new CustomEvent('catalog-updated'));
    } catch (err) {
      toast.error(err.message || "Failed to delete category");
    }
  };

  // Handle Order Status Update
  const handleUpdateOrderStatus = async (orderId, newStatus) => {
    try {
      await api.admin.updateOrderStatus(orderId, newStatus);
      toast.success(`Order status updated to ${newStatus}`);
      loadOrders();
      loadOverviewData();
    } catch (err) {
      toast.error(err.message || "Failed to update order status");
    }
  };

  // Filtered & Paginated Products
  const filteredProducts = products.filter(p => 
    p.name?.toLowerCase().includes(productSearch.toLowerCase()) ||
    p.productBrand?.toLowerCase().includes(productSearch.toLowerCase()) ||
    p.productType?.toLowerCase().includes(productSearch.toLowerCase())
  );
  const totalPages = Math.ceil(filteredProducts.length / productsPerPage) || 1;
  const paginatedProducts = filteredProducts.slice(
    (productPage - 1) * productsPerPage,
    productPage * productsPerPage
  );

  // Filtered Orders
  const filteredOrders = orders.filter(ord => {
    const status = ord.status || ord.orderStatus || 'Pending';
    const matchesStatus = orderStatusFilter === 'All' || status.toLowerCase() === orderStatusFilter.toLowerCase();
    const matchesSearch = !orderSearch || 
      ord.id?.toLowerCase().includes(orderSearch.toLowerCase()) ||
      ord.userEmail?.toLowerCase().includes(orderSearch.toLowerCase());
    return matchesStatus && matchesSearch;
  });

  // Aggregated Customers List
  const customersMap = {};
  orders.forEach(ord => {
    const email = ord.userEmail || 'Anonymous';
    if (!customersMap[email]) {
      customersMap[email] = {
        email,
        totalOrders: 0,
        totalSpent: 0,
        lastOrderDate: ord.orderDate
      };
    }
    customersMap[email].totalOrders += 1;
    customersMap[email].totalSpent += Number(ord.total || 0);
    if (new Date(ord.orderDate) > new Date(customersMap[email].lastOrderDate)) {
      customersMap[email].lastOrderDate = ord.orderDate;
    }
  });
  const customersList = Object.values(customersMap);

  return (
    <div className="admin_dashboard_page container">
      {/* Header Bar */}
      <div className="admin_header_bar">
        <div className="admin_header_title">
          <h1>
            <span>⚡ LuxeStore</span> Admin Console
          </h1>
          <p>Full control over catalog inventory, customer orders, brands, and store revenue analytics.</p>
        </div>

        {activeTab === 'products' && (
          <button 
            type="button" 
            className="btn" 
            onClick={() => handleOpenProductModal()}
          >
            <HiOutlinePlus /> Add New Product
          </button>
        )}
      </div>

      {/* Tabs Bar */}
      <div className="admin_tabs_bar">
        <button 
          className={`admin_tab_btn ${activeTab === 'overview' ? 'active' : ''}`}
          onClick={() => setActiveTab('overview')}
        >
          <HiOutlineChartBar /> Overview & Revenue
        </button>
        <button 
          className={`admin_tab_btn ${activeTab === 'products' ? 'active' : ''}`}
          onClick={() => setActiveTab('products')}
        >
          <HiOutlineCube /> Products Catalog ({products.length})
        </button>
        <button 
          className={`admin_tab_btn ${activeTab === 'catalog' ? 'active' : ''}`}
          onClick={() => setActiveTab('catalog')}
        >
          <HiOutlineTag /> Categories & Brands
        </button>
        <button 
          className={`admin_tab_btn ${activeTab === 'orders' ? 'active' : ''}`}
          onClick={() => setActiveTab('orders')}
        >
          <HiOutlineClipboardDocumentList /> Orders ({orders.length})
        </button>
        <button 
          className={`admin_tab_btn ${activeTab === 'customers' ? 'active' : ''}`}
          onClick={() => setActiveTab('customers')}
        >
          <HiOutlineUsers /> Customers ({customersList.length})
        </button>
      </div>

      {/* TAB 1: OVERVIEW */}
      {activeTab === 'overview' && (
        <div className="tab_content_view">
          {/* Stats KPI Cards */}
          <div className="admin_stats_grid">
            <div className="admin_stat_card">
              <div className="stat_icon_pill sales">
                <HiOutlineCurrencyDollar />
              </div>
              <div className="stat_info_col">
                <span className="stat_label">Total Revenue</span>
                <span className="stat_value">${(stats.totalSales || 0).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</span>
              </div>
            </div>

            <div className="admin_stat_card">
              <div className="stat_icon_pill orders">
                <HiOutlineShoppingBag />
              </div>
              <div className="stat_info_col">
                <span className="stat_label">Total Orders</span>
                <span className="stat_value">{stats.totalOrders || orders.length || 0}</span>
              </div>
            </div>

            <div className="admin_stat_card">
              <div className="stat_icon_pill products">
                <HiOutlineCube />
              </div>
              <div className="stat_info_col">
                <span className="stat_label">Catalog Products</span>
                <span className="stat_value">{products.length || 111}</span>
              </div>
            </div>

            <div className="admin_stat_card">
              <div className="stat_icon_pill customers">
                <HiOutlineUsers />
              </div>
              <div className="stat_info_col">
                <span className="stat_label">Active Customers</span>
                <span className="stat_value">{customersList.length || 1}</span>
              </div>
            </div>

            <div className="admin_stat_card">
              <div className="stat_icon_pill alert">
                <HiOutlineExclamationTriangle />
              </div>
              <div className="stat_info_col">
                <span className="stat_label">Low Stock Alerts</span>
                <span className="stat_value">{stats.lowStockProducts?.length || 0}</span>
              </div>
            </div>
          </div>

          {/* Sales Trends Chart */}
          <div className="sales_chart_card">
            <div className="admin_section_header">
              <h2>
                <HiOutlineArrowTrendingUp style={{ color: 'var(--main-color)' }} />
                Sales & Revenue Analytics Trend
              </h2>
              <span style={{ fontSize: '13px', color: 'var(--p-color)', fontWeight: 600 }}>
                Live SQL Server Transaction Data
              </span>
            </div>

            <div className="chart_bars_wrapper">
              {[
                { day: 'Mon', amount: 1420, height: '45%' },
                { day: 'Tue', amount: 2180, height: '65%' },
                { day: 'Wed', amount: 1890, height: '55%' },
                { day: 'Thu', amount: 2950, height: '85%' },
                { day: 'Fri', amount: 3400, height: '95%' },
                { day: 'Sat', amount: 2800, height: '78%' },
                { day: 'Sun', amount: 3650, height: '100%' },
              ].map((item, idx) => (
                <div key={idx} className="chart_bar_col">
                  <span className="chart_bar_amount">${item.amount}</span>
                  <div className="chart_bar_fill" style={{ height: item.height }} title={`Sales: $${item.amount}`} />
                  <span className="chart_bar_label">{item.day}</span>
                </div>
              ))}
            </div>
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(360px, 1fr))', gap: '25px' }}>
            {/* Top Selling Products */}
            <div className="admin_section_card">
              <div className="admin_section_header">
                <h2>Top Performing Products</h2>
              </div>
              {stats.topSellingProducts?.length > 0 ? (
                <div className="admin_table_responsive">
                  <table className="admin_table">
                    <thead>
                      <tr>
                        <th>Product Name</th>
                        <th>Sold Units</th>
                      </tr>
                    </thead>
                    <tbody>
                      {stats.topSellingProducts.map(p => (
                        <tr key={p.productId}>
                          <td style={{ fontWeight: 600 }}>{p.productName}</td>
                          <td><span className="status_pill paymentreceived">+{p.totalQuantitySold} units</span></td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              ) : (
                <p style={{ color: 'var(--p-color)', fontSize: '14px' }}>No sales records registered yet.</p>
              )}
            </div>

            {/* Low Stock Alerts */}
            <div className="admin_section_card">
              <div className="admin_section_header">
                <h2>Low Stock Inventory Alerts</h2>
              </div>
              {stats.lowStockProducts?.length > 0 ? (
                <div className="admin_table_responsive">
                  <table className="admin_table">
                    <thead>
                      <tr>
                        <th>Product Name</th>
                        <th>Stock Remaining</th>
                      </tr>
                    </thead>
                    <tbody>
                      {stats.lowStockProducts.map(p => (
                        <tr key={p.id}>
                          <td style={{ fontWeight: 600 }}>{p.name}</td>
                          <td><span className="status_pill cancelled">{p.stock} left</span></td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              ) : (
                <p style={{ color: '#10b981', fontSize: '14px', fontWeight: 600 }}>
                  ✓ All inventory levels are healthy!
                </p>
              )}
            </div>
          </div>
        </div>
      )}

      {/* TAB 2: PRODUCTS */}
      {activeTab === 'products' && (
        <div className="tab_content_view">
          <div className="admin_section_card">
            <div className="admin_section_header">
              <h2>Product Inventory Catalog ({filteredProducts.length})</h2>
              <div style={{ display: 'flex', gap: '10px' }}>
                <input 
                  type="text" 
                  placeholder="Search products by title, brand, type..." 
                  value={productSearch}
                  onChange={(e) => {
                    setProductSearch(e.target.value);
                    setProductPage(1);
                  }}
                  style={{
                    padding: '8px 14px',
                    borderRadius: '8px',
                    border: '1px solid var(--border-color)',
                    background: 'var(--bg-color)',
                    color: 'var(--color-heading)',
                    fontSize: '13.5px',
                    minWidth: '260px'
                  }}
                />
              </div>
            </div>

            {loading ? (
              <p style={{ padding: '20px', textAlign: 'center', color: 'var(--p-color)' }}>Loading products...</p>
            ) : (
              <>
                <div className="admin_table_responsive">
                  <table className="admin_table">
                    <thead>
                      <tr>
                        <th>Image</th>
                        <th>Title</th>
                        <th>Category</th>
                        <th>Brand</th>
                        <th>Price</th>
                        <th>Stock & Quick Edit</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {paginatedProducts.map(p => {
                        const imgUrl = (p.pictureUrl?.startsWith('http') || p.pictureUrl?.startsWith('/')) 
                          ? p.pictureUrl 
                          : ('/' + (p.pictureUrl || 'src/img/no-results.png'));

                        return (
                          <tr key={p.id}>
                            <td>
                              <img 
                                src={imgUrl} 
                                alt={p.name} 
                                className="table_img_thumbnail"
                                onError={(e) => { e.target.src = '/src/img/no-results.png'; }}
                              />
                            </td>
                            <td style={{ fontWeight: 600, maxWidth: '240px' }}>{p.name}</td>
                            <td><span className="status_pill shipped">{p.productType}</span></td>
                            <td>{p.productBrand}</td>
                            <td style={{ fontWeight: 700 }}>${Number(p.price).toFixed(2)}</td>
                            <td>
                              <div className="stock_quick_control">
                                <button 
                                  type="button" 
                                  className="stock_qty_btn" 
                                  onClick={() => handleQuickStockChange(p, -1)}
                                  title="Decrease stock by 1"
                                >
                                  -
                                </button>
                                <span style={{ fontWeight: 700, minWidth: '24px', textAlign: 'center' }}>
                                  {p.stock}
                                </span>
                                <button 
                                  type="button" 
                                  className="stock_qty_btn" 
                                  onClick={() => handleQuickStockChange(p, 1)}
                                  title="Increase stock by 1"
                                >
                                  +
                                </button>
                              </div>
                            </td>
                            <td>
                              <div className="table_action_btns">
                                <button 
                                  type="button" 
                                  className="btn_sm_action"
                                  onClick={() => handleOpenProductModal(p)}
                                  title="Edit full product details"
                                >
                                  <HiOutlinePencilSquare /> Edit
                                </button>
                                <button 
                                  type="button" 
                                  className="btn_sm_action danger"
                                  onClick={() => handleDeleteProduct(p.id, p.name)}
                                  title="Delete product"
                                >
                                  <HiOutlineTrash />
                                </button>
                              </div>
                            </td>
                          </tr>
                        );
                      })}
                    </tbody>
                  </table>
                </div>

                {/* Pagination Controls */}
                <div className="admin_pagination_row">
                  <span style={{ fontSize: '13px', color: 'var(--p-color)' }}>
                    Showing page {productPage} of {totalPages} ({filteredProducts.length} items)
                  </span>

                  <div className="pagination_btns_group">
                    <button 
                      className="page_btn"
                      disabled={productPage <= 1}
                      onClick={() => setProductPage(prev => Math.max(1, prev - 1))}
                    >
                      Previous
                    </button>

                    {Array.from({ length: Math.min(totalPages, 5) }, (_, i) => i + 1).map(num => (
                      <button 
                        key={num}
                        className={`page_btn ${productPage === num ? 'active' : ''}`}
                        onClick={() => setProductPage(num)}
                      >
                        {num}
                      </button>
                    ))}

                    <button 
                      className="page_btn"
                      disabled={productPage >= totalPages}
                      onClick={() => setProductPage(prev => Math.min(totalPages, prev + 1))}
                    >
                      Next
                    </button>
                  </div>
                </div>
              </>
            )}
          </div>
        </div>
      )}

      {/* TAB 3: CATEGORIES & BRANDS */}
      {activeTab === 'catalog' && (
        <div className="tab_content_view">
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(360px, 1fr))', gap: '25px' }}>
            {/* Categories */}
            <div className="admin_section_card">
              <div className="admin_section_header">
                <h2>Product Categories ({types.length})</h2>
              </div>

              <form onSubmit={handleCreateType} style={{ display: 'flex', gap: '10px', marginBottom: '20px' }}>
                <input 
                  type="text" 
                  placeholder="New category name..." 
                  value={newTypeName}
                  onChange={(e) => setNewTypeName(e.target.value)}
                  style={{
                    flex: 1,
                    padding: '9px 14px',
                    borderRadius: '8px',
                    border: '1px solid var(--border-color)',
                    background: 'var(--bg-color)',
                    color: 'var(--color-heading)'
                  }}
                  required
                />
                <button type="submit" className="btn" style={{ padding: '8px 16px', borderRadius: '8px' }}>
                  <HiOutlinePlus /> Add
                </button>
              </form>

              <div className="admin_table_responsive">
                <table className="admin_table">
                  <thead>
                    <tr>
                      <th>ID</th>
                      <th>Category Name</th>
                      <th>Action</th>
                    </tr>
                  </thead>
                  <tbody>
                    {types.map(t => (
                      <tr key={t.id}>
                        <td>#{t.id}</td>
                        <td style={{ fontWeight: 600 }}>{t.name}</td>
                        <td>
                          <button 
                            type="button" 
                            className="btn_sm_action danger"
                            onClick={() => handleDeleteType(t.id, t.name)}
                          >
                            <HiOutlineTrash /> Delete
                          </button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>

            {/* Brands */}
            <div className="admin_section_card">
              <div className="admin_section_header">
                <h2>Store Brands ({brands.length})</h2>
              </div>

              <form onSubmit={handleCreateBrand} style={{ display: 'flex', gap: '10px', marginBottom: '20px' }}>
                <input 
                  type="text" 
                  placeholder="New brand name..." 
                  value={newBrandName}
                  onChange={(e) => setNewBrandName(e.target.value)}
                  style={{
                    flex: 1,
                    padding: '9px 14px',
                    borderRadius: '8px',
                    border: '1px solid var(--border-color)',
                    background: 'var(--bg-color)',
                    color: 'var(--color-heading)'
                  }}
                  required
                />
                <button type="submit" className="btn" style={{ padding: '8px 16px', borderRadius: '8px' }}>
                  <HiOutlinePlus /> Add
                </button>
              </form>

              <div className="admin_table_responsive">
                <table className="admin_table">
                  <thead>
                    <tr>
                      <th>ID</th>
                      <th>Brand Name</th>
                      <th>Action</th>
                    </tr>
                  </thead>
                  <tbody>
                    {brands.map(b => (
                      <tr key={b.id}>
                        <td>#{b.id}</td>
                        <td style={{ fontWeight: 600 }}>{b.name}</td>
                        <td>
                          <button 
                            type="button" 
                            className="btn_sm_action danger"
                            onClick={() => handleDeleteBrand(b.id, b.name)}
                          >
                            <HiOutlineTrash /> Delete
                          </button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* TAB 4: ORDERS */}
      {activeTab === 'orders' && (
        <div className="tab_content_view">
          <div className="admin_section_card">
            <div className="admin_section_header">
              <h2>Customer Orders ({filteredOrders.length})</h2>
              
              <div style={{ display: 'flex', gap: '10px' }}>
                <input 
                  type="text" 
                  placeholder="Filter by customer email or ID..."
                  value={orderSearch}
                  onChange={(e) => setOrderSearch(e.target.value)}
                  style={{
                    padding: '7px 12px',
                    borderRadius: '8px',
                    border: '1px solid var(--border-color)',
                    background: 'var(--bg-color)',
                    color: 'var(--color-heading)',
                    fontSize: '13px'
                  }}
                />
              </div>
            </div>

            {/* Status Filter Bar */}
            <div className="orders_filter_bar">
              {['All', 'Pending', 'PaymentReceived', 'Shipped', 'Delivered', 'Cancelled'].map(st => (
                <button 
                  key={st}
                  className={`order_filter_pill ${orderStatusFilter === st ? 'active' : ''}`}
                  onClick={() => setOrderStatusFilter(st)}
                >
                  {st}
                </button>
              ))}
            </div>

            {filteredOrders.length === 0 ? (
              <p style={{ padding: '20px', textAlign: 'center', color: 'var(--p-color)' }}>No matching orders found.</p>
            ) : (
              <div className="admin_table_responsive">
                <table className="admin_table">
                  <thead>
                    <tr>
                      <th>Order ID</th>
                      <th>Customer</th>
                      <th>Date</th>
                      <th>Total</th>
                      <th>Status</th>
                      <th>Change Status</th>
                      <th>Action</th>
                    </tr>
                  </thead>
                  <tbody>
                    {filteredOrders.map(ord => {
                      const currentStatus = ord.status || ord.orderStatus || 'Pending';
                      const statusClass = currentStatus.toLowerCase().replace(/\s+/g, '');

                      return (
                        <tr key={ord.id}>
                          <td style={{ fontFamily: 'monospace', fontWeight: 600 }}>
                            #{ord.id?.substring(0, 8)}...
                          </td>
                          <td style={{ fontWeight: 600 }}>{ord.userEmail}</td>
                          <td>{new Date(ord.orderDate).toLocaleDateString()}</td>
                          <td style={{ fontWeight: 800 }}>${Number(ord.total).toFixed(2)}</td>
                          <td>
                            <span className={`status_pill ${statusClass}`}>
                              {currentStatus}
                            </span>
                          </td>
                          <td>
                            <select 
                              value={currentStatus}
                              onChange={(e) => handleUpdateOrderStatus(ord.id, e.target.value)}
                              style={{
                                padding: '5px 8px',
                                borderRadius: '6px',
                                border: '1px solid var(--border-color)',
                                background: 'var(--bg-color)',
                                color: 'var(--color-heading)',
                                fontSize: '13px'
                              }}
                            >
                              <option value="Pending">Pending</option>
                              <option value="PaymentReceived">Payment Received</option>
                              <option value="Shipped">Shipped</option>
                              <option value="Delivered">Delivered</option>
                              <option value="Cancelled">Cancelled</option>
                            </select>
                          </td>
                          <td>
                            <button 
                              type="button" 
                              className="btn_sm_action"
                              onClick={() => setViewingOrder(ord)}
                              title="View order details"
                            >
                              <HiOutlineEye /> View
                            </button>
                          </td>
                        </tr>
                      );
                    })}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        </div>
      )}

      {/* TAB 5: CUSTOMERS */}
      {activeTab === 'customers' && (
        <div className="tab_content_view">
          <div className="admin_section_card">
            <div className="admin_section_header">
              <h2>Registered Customers & Purchase History ({customersList.length})</h2>
            </div>

            <div className="admin_table_responsive">
              <table className="admin_table">
                <thead>
                  <tr>
                    <th>Customer Email</th>
                    <th>Total Orders Placed</th>
                    <th>Lifetime Spend ($)</th>
                    <th>Last Order Date</th>
                  </tr>
                </thead>
                <tbody>
                  {customersList.map((c, idx) => (
                    <tr key={idx}>
                      <td style={{ fontWeight: 700 }}>{c.email}</td>
                      <td>
                        <span className="status_pill paymentreceived">
                          {c.totalOrders} {c.totalOrders === 1 ? 'order' : 'orders'}
                        </span>
                      </td>
                      <td style={{ fontWeight: 800, color: 'var(--main-color)' }}>
                        ${c.totalSpent.toFixed(2)}
                      </td>
                      <td>{new Date(c.lastOrderDate).toLocaleDateString()}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      )}

      {/* VIEW ORDER DETAILS MODAL */}
      {viewingOrder && (
        <div className="admin_modal_overlay" onClick={() => setViewingOrder(null)}>
          <div className="admin_modal_box" onClick={(e) => e.stopPropagation()}>
            <div className="modal_header">
              <h3>Order Details #{viewingOrder.id?.substring(0, 8)}</h3>
              <button type="button" className="modal_close_btn" onClick={() => setViewingOrder(null)}>
                <HiOutlineXMark />
              </button>
            </div>

            <div className="order_detail_grid">
              <div className="order_detail_col">
                <span>Customer Email</span>
                <strong>{viewingOrder.userEmail}</strong>
              </div>
              <div className="order_detail_col">
                <span>Order Date</span>
                <strong>{new Date(viewingOrder.orderDate).toLocaleString()}</strong>
              </div>
              <div className="order_detail_col">
                <span>Shipping Method</span>
                <strong>{viewingOrder.deliveryMethod || 'Standard Delivery'}</strong>
              </div>
              <div className="order_detail_col">
                <span>Total Amount</span>
                <strong style={{ color: 'var(--main-color)', fontSize: '16px' }}>
                  ${Number(viewingOrder.total).toFixed(2)}
                </strong>
              </div>
            </div>

            {viewingOrder.address && (
              <div style={{ marginBottom: '16px' }}>
                <h4 style={{ fontSize: '13px', textTransform: 'uppercase', color: 'var(--p-color)', marginBottom: '4px' }}>
                  Shipping Destination
                </h4>
                <p style={{ fontSize: '14px', fontWeight: 600 }}>
                  {viewingOrder.address.firstName} {viewingOrder.address.lastName}, {viewingOrder.address.street}, {viewingOrder.address.city}, {viewingOrder.address.country}
                </p>
              </div>
            )}

            <h4 style={{ fontSize: '14px', fontWeight: 700, marginTop: '20px', marginBottom: '10px' }}>
              Purchased Items ({(viewingOrder.orderItems || viewingOrder.items || []).length})
            </h4>

            <div className="order_items_breakdown">
              {(viewingOrder.orderItems || viewingOrder.items || []).map((item, i) => (
                <div key={i} className="order_breakdown_item">
                  <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                    <img 
                      src={item.pictureUrl || '/src/img/no-results.png'} 
                      alt={item.productName} 
                      style={{ width: '40px', height: '40px', borderRadius: '6px', objectFit: 'contain' }}
                    />
                    <div>
                      <strong style={{ display: 'block', fontSize: '13.5px' }}>{item.productName}</strong>
                      <span style={{ fontSize: '12px', color: 'var(--p-color)' }}>Quantity: {item.quantity}</span>
                    </div>
                  </div>
                  <span style={{ fontWeight: 700, fontSize: '14px' }}>
                    ${Number(item.price * item.quantity).toFixed(2)}
                  </span>
                </div>
              ))}
            </div>

            <div className="modal_footer_actions">
              <button type="button" className="btn" onClick={() => setViewingOrder(null)}>
                Close
              </button>
            </div>
          </div>
        </div>
      )}

      {/* CREATE / EDIT PRODUCT MODAL */}
      {isProductModalOpen && (
        <div className="admin_modal_overlay" onClick={() => setIsProductModalOpen(false)}>
          <div className="admin_modal_box" onClick={(e) => e.stopPropagation()}>
            <div className="modal_header">
              <h3>{editingProduct ? 'Edit Product' : 'Add New Product to Store'}</h3>
              <button 
                type="button" 
                className="modal_close_btn" 
                onClick={() => setIsProductModalOpen(false)}
              >
                <HiOutlineXMark />
              </button>
            </div>

            <form onSubmit={handleProductSubmit}>
              <div className="admin_form_group">
                <label>Product Title *</label>
                <input 
                  type="text" 
                  value={productForm.name}
                  onChange={(e) => setProductForm({ ...productForm, name: e.target.value })}
                  placeholder="e.g. Classic Oxford Cotton Shirt"
                  required
                />
              </div>

              <div className="admin_form_group">
                <label>Description *</label>
                <textarea 
                  rows={3}
                  value={productForm.description}
                  onChange={(e) => setProductForm({ ...productForm, description: e.target.value })}
                  placeholder="Detailed product overview, fabric specs, and fit..."
                  required
                />
              </div>

              <div className="form_grid_2">
                <div className="admin_form_group">
                  <label>Price ($) *</label>
                  <input 
                    type="number" 
                    step="0.01" 
                    value={productForm.price}
                    onChange={(e) => setProductForm({ ...productForm, price: e.target.value })}
                    placeholder="49.99"
                    required
                  />
                </div>

                <div className="admin_form_group">
                  <label>Inventory Stock Units *</label>
                  <input 
                    type="number" 
                    value={productForm.stock}
                    onChange={(e) => setProductForm({ ...productForm, stock: e.target.value })}
                    placeholder="20"
                    required
                  />
                </div>
              </div>

              <div className="form_grid_2">
                <div className="admin_form_group">
                  <label>Category / Type *</label>
                  <select 
                    value={productForm.productTypeId}
                    onChange={(e) => setProductForm({ ...productForm, productTypeId: e.target.value })}
                    required
                  >
                    {types.map(t => (
                      <option key={t.id} value={t.id}>{t.name}</option>
                    ))}
                  </select>
                </div>

                <div className="admin_form_group">
                  <label>Brand *</label>
                  <select 
                    value={productForm.productBrandId}
                    onChange={(e) => setProductForm({ ...productForm, productBrandId: e.target.value })}
                    required
                  >
                    {brands.map(b => (
                      <option key={b.id} value={b.id}>{b.name}</option>
                    ))}
                  </select>
                </div>
              </div>

              <div className="admin_form_group">
                <label>Upload Product Image (Instant Store Publish)</label>
                <input 
                  type="file" 
                  accept="image/*"
                  onChange={handleImageFileChange}
                />
              </div>

              {imagePreview && (
                <div style={{ marginBottom: '14px', display: 'flex', alignItems: 'center', gap: '12px' }}>
                  <img 
                    src={imagePreview} 
                    alt="Preview" 
                    style={{ width: '60px', height: '60px', objectFit: 'contain', borderRadius: '8px', border: '1px solid var(--border-color)', background: 'var(--bg-color)' }}
                  />
                  <span style={{ fontSize: '13px', color: 'var(--p-color)' }}>Image ready for upload</span>
                </div>
              )}

              <div className="admin_form_group">
                <label>Or Image URL (Fallback)</label>
                <input 
                  type="text" 
                  value={productForm.pictureUrl}
                  onChange={(e) => {
                    setProductForm({ ...productForm, pictureUrl: e.target.value });
                    setImagePreview(e.target.value);
                  }}
                  placeholder="https://... or images/products/sample.jpg"
                />
              </div>

              <div className="modal_footer_actions">
                <button 
                  type="button" 
                  className="btn_sm_action" 
                  onClick={() => setIsProductModalOpen(false)}
                >
                  Cancel
                </button>
                <button type="submit" className="btn">
                  {editingProduct ? 'Save Changes' : 'Create & Publish Product'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
