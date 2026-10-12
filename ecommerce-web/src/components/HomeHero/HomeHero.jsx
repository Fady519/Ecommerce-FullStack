import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { FaBagShopping, FaArrowRight, FaShieldHalved, FaTruckFast, FaLayerGroup } from 'react-icons/fa6';
import { api } from '../../services/api';
import './HomeHero.css';

export default function HomeHero() {
  const [metrics, setMetrics] = useState({
    products: 110,
    categories: 10,
    brands: 14
  });

  const loadMetrics = async () => {
    try {
      const [prodRes, typesRes, brandsRes] = await Promise.all([
        api.products.getAll({ pageSize: 1 }).catch(() => null),
        api.products.getTypes().catch(() => []),
        api.products.getBrands().catch(() => [])
      ]);

      const prodCount = prodRes?.count ?? 110;
      const typesList = Array.isArray(typesRes) ? typesRes : (typesRes?.value || typesRes?.data || []);
      const brandsList = Array.isArray(brandsRes) ? brandsRes : (brandsRes?.value || brandsRes?.data || []);

      setMetrics({
        products: prodCount,
        categories: typesList.length || 10,
        brands: brandsList.length || 14
      });
    } catch (e) {
      console.error("Failed to load hero metrics:", e);
    }
  };

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
          <span>Full-Stack Verified E-Commerce Experience</span>
        </div>

        <h1 className="home_hero_title">
          Elevate Your Lifestyle with <span className="gradient_text">LuxeStore</span>
        </h1>

        <p className="home_hero_desc">
          A comprehensive full-stack e-commerce platform built with ASP.NET Core Web API and React 19. It delivers real-time catalog browsing across {metrics.products}+ products in {metrics.categories} curated categories, advanced multi-filter search, isolated customer carts, secure Stripe card processing alongside Cash on Delivery, complete order lifecycle tracking, and an integrated Admin Management Portal.
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
            <span className="hero_metric_num">+{metrics.products}</span>
            <span className="hero_metric_lbl">Active Catalog Items</span>
          </div>
          <div className="hero_metric_box">
            <span className="hero_metric_num">{metrics.categories}</span>
            <span className="hero_metric_lbl">Curated Categories</span>
          </div>
          <div className="hero_metric_box">
            <span className="hero_metric_num">{metrics.brands}</span>
            <span className="hero_metric_lbl">Global Brands</span>
          </div>
          <div className="hero_metric_box">
            <span className="hero_metric_num">100%</span>
            <span className="hero_metric_lbl">Safe & Tracked Delivery</span>
          </div>
        </div>
      </div>
    </section>
  );
}
