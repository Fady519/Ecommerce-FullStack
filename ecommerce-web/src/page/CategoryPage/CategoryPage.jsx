import React, { useEffect, useState, useMemo } from 'react';
import { useParams, useNavigate, useSearchParams } from 'react-router-dom';
import Product from './../../components/slideProducts/Product';
import './CategoryPage.css';
import SlideProductLoading from './../../components/slideProducts/SlideProductLoading';
import PageTransition from '../../components/PageTransition';
import { api } from '../../services/api';
import { HiOutlineFunnel, HiArrowPath } from "react-icons/hi2";

function CategoryPage() {
    const { category } = useParams();
    const navigate = useNavigate();
    const [searchParams, setSearchParams] = useSearchParams();

    const [loading, setLoading] = useState(true);
    const [products, setProducts] = useState([]);
    const [types, setTypes] = useState([]);
    const [brands, setBrands] = useState([]);

    // Read initial filters from URL query parameters (preserves on refresh)
    const urlTypeId = searchParams.get('typeId') ? Number(searchParams.get('typeId')) : null;
    const urlBrandId = searchParams.get('brandId') ? Number(searchParams.get('brandId')) : null;
    const urlSort = searchParams.get('sort') || 'NameAsc';

    const [selectedType, setSelectedType] = useState(urlTypeId);
    const [selectedBrand, setSelectedBrand] = useState(urlBrandId);
    const [sortBy, setSortBy] = useState(urlSort);

    // 1. Fetch categories and brands metadata
    useEffect(() => {
        Promise.all([
            api.products.getTypes().catch(() => []),
            api.products.getBrands().catch(() => [])
        ]).then(([typesRes, brandsRes]) => {
            const typesList = Array.isArray(typesRes) ? typesRes : (typesRes?.value || typesRes?.data || []);
            const brandsList = Array.isArray(brandsRes) ? brandsRes : (brandsRes?.value || brandsRes?.data || []);
            setTypes(typesList);
            setBrands(brandsList);

            // Sync from route param if query params are not explicitly set
            const queryHasType = searchParams.has('typeId');
            const queryHasBrand = searchParams.has('brandId');

            if (!queryHasType && !queryHasBrand) {
                const decodedCat = decodeURIComponent(category || "");
                if (decodedCat && decodedCat !== "all") {
                    const matchedType = typesList.find(t => t.name.toLowerCase() === decodedCat.toLowerCase());
                    const matchedBrand = brandsList.find(b => b.name.toLowerCase() === decodedCat.toLowerCase());

                    if (matchedType) {
                        setSelectedType(matchedType.id);
                        setSelectedBrand(null);
                    } else if (matchedBrand) {
                        setSelectedBrand(matchedBrand.id);
                        setSelectedType(null);
                    }
                } else {
                    setSelectedType(null);
                    setSelectedBrand(null);
                }
            } else {
                if (queryHasType) setSelectedType(urlTypeId);
                if (queryHasBrand) setSelectedBrand(urlBrandId);
            }

            if (searchParams.has('sort')) {
                setSortBy(urlSort);
            }
        });
    }, [category]);

    // 2. Helper to synchronize state with URL search params
    const syncFiltersToUrl = (newTypeId, newBrandId, newSort) => {
        const params = new URLSearchParams();
        if (newTypeId) params.set('typeId', newTypeId.toString());
        if (newBrandId) params.set('brandId', newBrandId.toString());
        if (newSort && newSort !== 'NameAsc') params.set('sort', newSort);
        setSearchParams(params, { replace: true });
    };

    // 3. Fetch products whenever selected filters change
    useEffect(() => {
        const fetchFilteredProducts = async () => {
            setLoading(true);
            try {
                const params = {
                    pageSize: 50,
                    typeId: selectedType || undefined,
                    brandId: selectedBrand || undefined,
                    sort: sortBy || "NameAsc"
                };
                const res = await api.products.getAll(params);
                setProducts(res.data || []);
            } catch (err) {
                console.error("Failed to load catalog products:", err);
                setProducts([]);
            } finally {
                setLoading(false);
            }
        };

        fetchFilteredProducts();
        window.addEventListener('catalog-updated', fetchFilteredProducts);
        return () => window.removeEventListener('catalog-updated', fetchFilteredProducts);
    }, [selectedType, selectedBrand, sortBy]);

    const handleTypeChange = (e) => {
        const val = e.target.value ? Number(e.target.value) : null;
        setSelectedType(val);
        syncFiltersToUrl(val, selectedBrand, sortBy);
    };

    const handleBrandChange = (e) => {
        const val = e.target.value ? Number(e.target.value) : null;
        setSelectedBrand(val);
        syncFiltersToUrl(selectedType, val, sortBy);
    };

    const handleSortChange = (e) => {
        const val = e.target.value;
        setSortBy(val);
        syncFiltersToUrl(selectedType, selectedBrand, val);
    };

    const resetFilters = () => {
        setSelectedType(null);
        setSelectedBrand(null);
        setSortBy("NameAsc");
        setSearchParams({}, { replace: true });
        navigate('/category/all', { replace: true });
    };

    const pageTitle = selectedType 
        ? types.find(t => t.id === selectedType)?.name 
        : selectedBrand 
        ? brands.find(b => b.id === selectedBrand)?.name 
        : (category && category !== "all" ? decodeURIComponent(category) : "All Products");

    return (
        <PageTransition key={category}>
            <div className='category_products'>
                <div className='container'>
                    
                    {/* Catalog Header Banner */}
                    <div className='catalog_header_banner'>
                        <div className="banner_text">
                            <span className="catalog_tag">Store Collection</span>
                            <h2>{pageTitle}</h2>
                            <p>Explore our premium collection of verified store products.</p>
                        </div>
                        <div className="catalog_stats">
                            <span className="stats_count">{products.length}</span>
                            <span className="stats_label">Products Available</span>
                        </div>
                    </div>

                    {/* Filter and Sort Toolbar */}
                    <div className='catalog_toolbar'>
                        <div className='filters_group'>
                            <div className="filter_item">
                                <label><HiOutlineFunnel /> Category:</label>
                                <select 
                                    value={selectedType || ""} 
                                    onChange={handleTypeChange}
                                >
                                    <option value="">All Categories</option>
                                    {types.map(t => (
                                        <option key={t.id} value={t.id}>{t.name}</option>
                                    ))}
                                </select>
                            </div>

                            <div className="filter_item">
                                <label>Brand:</label>
                                <select 
                                    value={selectedBrand || ""} 
                                    onChange={handleBrandChange}
                                >
                                    <option value="">All Brands</option>
                                    {brands.map(b => (
                                        <option key={b.id} value={b.id}>{b.name}</option>
                                    ))}
                                </select>
                            </div>

                            {(selectedType || selectedBrand) && (
                                <button type="button" className="reset_filter_btn" onClick={resetFilters}>
                                    <HiArrowPath /> Clear Filters
                                </button>
                            )}
                        </div>

                        <div className='sort_group'>
                            <label>Sort by:</label>
                            <select value={sortBy} onChange={handleSortChange}>
                                <option value="NameAsc">Name (A to Z)</option>
                                <option value="NameDesc">Name (Z to A)</option>
                                <option value="PriceAsc">Price: Low to High</option>
                                <option value="PriceDesc">Price: High to Low</option>
                            </select>
                        </div>
                    </div>

                    {/* Products Grid */}
                    {loading ? (
                        <div className="catalog_loading_grid">
                            <SlideProductLoading />
                        </div>
                    ) : products.length > 0 ? (
                        <div className='products_grid'>
                            {products.map((item) => (
                                <Product item={item} key={item.id} />
                            ))}
                        </div>
                    ) : (
                        <div className='no_products_found'>
                            <div className="empty_icon">🛍️</div>
                            <h3>No products found</h3>
                            <p>No products match the selected category or brand filters.</p>
                            <button type="button" className="btn_clear" onClick={resetFilters}>
                                View All Products
                            </button>
                        </div>
                    )}

                </div>
            </div>
        </PageTransition>
    );
}

export default CategoryPage;
