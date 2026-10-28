import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { FaBagShopping, FaArrowRight, FaShieldHalved, FaTruckFast, FaLayerGroup } from 'react-icons/fa6';
import { api } from '../../services/api';
import './HomeHero.css';

export default function HomeHero() {
  // null until the API answers, so we never flash made-up numbers
  const [metrics, setMetrics] = useState(null);

  const loadMetrics = async () => {
    try {
      const [prodRes, typesRes, brandsRes] = await Promise.all([
        api.products.getAll({ pageSize: 1 }),
        api.products.getTypes(),
        api.products.getBrands()
      ]);

      const typesList = Array.isArray(typesRes) ? typesRes : (typesRes?.value || typesRes?.data || []);
      const brandsList = Array.isArray(brandsRes) ? brandsRes : (brandsRes?.value || brandsRes?.data || []);

      setMetrics({
        products: prodRes?.count ?? 0,
        categories: typesList.length,
        brands: brandsList.length
      });
    } catch (e) {
      console.error("Failed to load hero metrics:", e);
    }
  };

  const show = (value) => (metrics ? value : '–');

  useEffect(() => {
    loadMetrics();
    window.addEventListener('catalog-updated', loadMetrics);
    return () => window.removeEventListener('catalog-updated', loadMetrics);
  }, []);

  return (
    <section className="home_hero_section">
      <div className="home_hero_ambient_glow" aria-hidden="true" />
      
      <div className="home_hero_content">
        <div className="hero_badge_pill">
          <FaShieldHalved />
          <span>ASP.NET Core 10 · React 19 · SQL Server</span>
        </div>

        <h1 className="home_hero_title">
          Elevate Your Lifestyle with <span className="gradient_text">LuxeStore</span>
        </h1>

        <p className="home_hero_desc">
          A full-stack e-commerce demo built with an ASP.NET Core 10 Web API and React 19. Filter and search the catalog, keep your cart on the server, and check out with Cash on Delivery or a Stripe test-mode payment. Admins manage products, orders and stock from the admin console.
        </p>

        <div className="home_hero_actions">
          <Link to="/category/all" className="hero_btn_primary">
            <span>Explore Collection</span>
            <FaArrowRight />
          </Link>
          <Link to="/about" className="hero_btn_secondary">
            <FaLayerGroup />
            <span>Architecture & Story</span>
          </Link>
        </div>

        <div className="hero_metrics_grid">
          <div className="hero_metric_box">
            <span className="hero_metric_num">{show(metrics?.products)}</span>
            <span className="hero_metric_lbl">Products</span>
          </div>
          <div className="hero_metric_box">
            <span className="hero_metric_num">{show(metrics?.categories)}</span>
            <span className="hero_metric_lbl">Categories</span>
          </div>
          <div className="hero_metric_box">
            <span className="hero_metric_num">{show(metrics?.brands)}</span>
            <span className="hero_metric_lbl">Brands</span>
          </div>
          <div className="hero_metric_box">
            <span className="hero_metric_num">2</span>
            <span className="hero_metric_lbl">Payment Options</span>
          </div>
        </div>
      </div>
    </section>
  );
}
