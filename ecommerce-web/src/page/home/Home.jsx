import React, { useEffect, useState } from 'react';
import HomeHero from '../../components/HomeHero/HomeHero';
import './home.css';
import SlideProductLoading from '../../components/slideProducts/SlideProductLoading';
import PageTransition from '../../components/PageTransition';
import SlideProducts from './../../components/slideProducts/SlideProducts';
import { api } from '../../services/api';
import { Link } from 'react-router-dom';
import { HiOutlineSparkles, HiOutlineArrowRight } from 'react-icons/hi2';

export default function Home() {
  const [types, setTypes] = useState([]);
  const [productsByType, setProductsByType] = useState({});
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadHomeData = async () => {
      setLoading(true);
      try {
        // 1. Fetch real categories from database
        const typesData = await api.products.getTypes();
        const typesList = Array.isArray(typesData) ? typesData : (typesData?.value || typesData?.data || []);
        setTypes(typesList);

        // 2. Fetch products for each category from the real ASP.NET Core API
        const results = await Promise.all(
          typesList.map(async (t) => {
            try {
              const res = await api.products.getAll({ typeId: t.id, pageSize: 8 });
              return { typeId: t.id, typeName: t.name, products: res.data || [] };
            } catch {
              return { typeId: t.id, typeName: t.name, products: [] };
            }
          })
        );

        const map = {};
        results.forEach((item) => {
          map[item.typeId] = item;
        });
        setProductsByType(map);
      } catch (error) {
        console.error("Error fetching homepage catalog from API:", error);
      } finally {
        setLoading(false);
      }
    };

    loadHomeData();
    window.addEventListener('catalog-updated', loadHomeData);
    return () => window.removeEventListener('catalog-updated', loadHomeData);
  }, []);

  return (
    <PageTransition>
      <div className="homepage_container">
        <div className="container">
          <HomeHero />
        </div>

        {/* Feature Highlights Strip */}
        <div className="container" style={{ margin: '30px auto' }}>
          <div style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))',
            gap: '20px',
            background: 'var(--card-bg, #ffffff)',
            padding: '24px 30px',
            borderRadius: '16px',
            boxShadow: 'var(--card-shadow, 0 4px 15px rgba(0,0,0,0.03))',
            border: '1px solid var(--border-color, #e2e8f0)',
            transition: 'background-color 0.25s, border-color 0.25s'
          }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '14px' }}>
              <span style={{ fontSize: '28px' }}>🚀</span>
              <div>
                <h4 style={{ fontSize: '15px', color: 'var(--color-heading)', fontWeight: '700' }}>Fast Delivery</h4>
                <p style={{ fontSize: '13px', color: 'var(--p-color)' }}>Nationwide tracked shipping</p>
              </div>
            </div>
            <div style={{ display: 'flex', alignItems: 'center', gap: '14px' }}>
              <span style={{ fontSize: '28px' }}>🛡️</span>
              <div>
                <h4 style={{ fontSize: '15px', color: 'var(--color-heading)', fontWeight: '700' }}>Verified Quality</h4>
                <p style={{ fontSize: '13px', color: 'var(--p-color)' }}>100% authentic products</p>
              </div>
            </div>
            <div style={{ display: 'flex', alignItems: 'center', gap: '14px' }}>
              <span style={{ fontSize: '28px' }}>💳</span>
              <div>
                <h4 style={{ fontSize: '15px', color: 'var(--color-heading)', fontWeight: '700' }}>Secure Checkout</h4>
                <p style={{ fontSize: '13px', color: 'var(--p-color)' }}>Stripe (test mode) or Cash on Delivery</p>
              </div>
            </div>
            <div style={{ display: 'flex', alignItems: 'center', gap: '14px' }}>
              <span style={{ fontSize: '28px' }}>💬</span>
              <div>
                <h4 style={{ fontSize: '15px', color: 'var(--color-heading)', fontWeight: '700' }}>24/7 Support</h4>
                <p style={{ fontSize: '13px', color: 'var(--p-color)' }}>Ready to assist anytime</p>
              </div>
            </div>
          </div>
        </div>

        {/* Categories Sections from Database */}
        {loading ? (
          <div className="container">
            <SlideProductLoading />
            <SlideProductLoading />
          </div>
        ) : (
          types.map((type) => {
            const group = productsByType[type.id];
            if (!group || group.products.length === 0) return null;

            return (
              <SlideProducts 
                key={type.id} 
                data={group.products} 
                title={type.name} 
              />
            );
          })
        )}
      </div>
    </PageTransition>
  );
}