import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import './ProductDetails.css';
import ProductDetailsLoading from "./ProductDetailsLoading";
import ProductImages from "./ProductImages";
import ProductInfo from "./ProductInfo";
import PageTransition from "../../../components/PageTransition";
import Product from "../../../components/slideProducts/Product";
import { api } from "../../../services/api";

function ProductDeatails() {
    const { id } = useParams();

    const [product, setProduct] = useState(null);
    const [loading, setLoading] = useState(true);
    const [relatedProducts, setRelatedProducts] = useState([]);
    const [loadingRelated, setLoadingRelated] = useState(true);

    useEffect(() => {
        const fetchProduct = async () => {
            setLoading(true);
            try {
                // First try real backend API if id is numeric
                if (!isNaN(Number(id))) {
                    const data = await api.products.getById(Number(id));
                    setProduct(data);
                } else {
                    const res = await fetch(`https://dummyjson.com/products/${id}`);
                    const data = await res.json();
                    setProduct(data);
                }
            } catch (error) {
                console.error("Failed to fetch product details:", error);
            } finally {
                setLoading(false);
            }
        };

        if (id) fetchProduct();
    }, [id]);

    useEffect(() => {
        if (!product) return;
        document.title = `${product.name || product.title} | LuxeStore`;

        const fetchRelated = async () => {
            setLoadingRelated(true);
            try {
                if (product.productTypeId) {
                    const res = await api.products.getAll({ typeId: product.productTypeId, pageSize: 4 });
                    setRelatedProducts(res.data.filter(p => p.id !== product.id));
                } else {
                    const res = await api.products.getAll({ pageSize: 4 });
                    setRelatedProducts(res.data.filter(p => p.id !== product.id));
                }
            } catch {
                setRelatedProducts([]);
            } finally {
                setLoadingRelated(false);
            }
        };

        fetchRelated();
    }, [product]);

    if (loading) {
        return (
            <div className="container" style={{ padding: "80px 0" }}>
                <ProductDetailsLoading />
            </div>
        );
    }

    if (!product) {
        return (
            <div className="container" style={{ padding: "80px 0", textAlign: "center" }}>
                <h2>Product Not Found</h2>
                <p>The product you are looking for does not exist or has been removed.</p>
            </div>
        );
    }

    return (
        <PageTransition key={id}>
            <div className="product_details">
                <div className="container">
                    <div className="item_hero">
                        <ProductImages product={product} />
                        <ProductInfo product={product} />
                    </div>

                    {relatedProducts.length > 0 && (
                        <div className="related_products_section" style={{ marginTop: "60px" }}>
                            <div className="top_slide">
                                <h2>Related Products</h2>
                                <p>You may also like these items from the same category.</p>
                            </div>
                            <div className="products_grid" style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(240px, 1fr))', gap: '20px', marginTop: '20px' }}>
                                {relatedProducts.map(p => (
                                    <Product key={p.id} item={p} />
                                ))}
                            </div>
                        </div>
                    )}
                </div>
            </div>
        </PageTransition>
    );
}

export default ProductDeatails;
