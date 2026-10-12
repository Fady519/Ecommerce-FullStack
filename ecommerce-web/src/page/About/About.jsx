import React, { useEffect, useState } from "react";
import "./About.css";
import PageTransition from '../../components/PageTransition';
import { api } from '../../services/api';

function About() {
  const [stats, setStats] = useState({
    products: 111,
    categories: 10,
    brands: 14,
    loading: true
  });

  useEffect(() => {
    const loadDynamicStats = async () => {
      try {
        const [prodRes, typesRes, brandsRes] = await Promise.all([
          api.products.getAll({ pageSize: 1 }).catch(() => null),
          api.products.getTypes().catch(() => []),
          api.products.getBrands().catch(() => [])
        ]);

        const prodCount = prodRes?.count || 111;
        const typesCount = Array.isArray(typesRes) ? typesRes.length : (typesRes?.data?.length || 10);
        const brandsCount = Array.isArray(brandsRes) ? brandsRes.length : (brandsRes?.data?.length || 14);

        setStats({
          products: prodCount,
          categories: typesCount,
          brands: brandsCount,
          loading: false
        });
      } catch (err) {
        console.error("Error loading stats:", err);
      }
    };

    loadDynamicStats();
    window.addEventListener('catalog-updated', loadDynamicStats);
    return () => window.removeEventListener('catalog-updated', loadDynamicStats);
  }, []);

  return (
    <PageTransition>
      <div className="about-page">

        <div className="about-hero">
          <div className="container">
            <h1>About LuxeStore</h1>
            <p>
              Engineered for seamless digital commerce. Backed by ASP.NET Core Web API and modern React, providing secure, lightning-fast shopping with guaranteed quality.
            </p>
          </div>
        </div>

        <div className="about-content container">

          <div className="about-text">
            <h2>Our Story & Mission</h2>
            <p>
              LuxeStore was built to demonstrate full-stack excellence in modern e-commerce. From robust entity-relational database design in SQL Server to responsive UI components, we focus on delivering real-time inventory synchronization, transparent order tracking, and uncompromising performance.
            </p>
          </div>

          <div className="about-features">

            <div className="feature">
              <h3>Fast Delivery</h3>
              <p>Reliable tracked delivery with transparent shipping methods.</p>
            </div>

            <div className="feature">
              <h3>Secure Payment</h3>
              <p>Protected checkout with Stripe Card Processing & Cash on Delivery.</p>
            </div>

            <div className="feature">
              <h3>Verified Quality</h3>
              <p>100% authentic curated lifestyle essentials and apparel.</p>
            </div>

          </div>

          <div className="about-stats">

            <div className="stat">
              <h3>+{stats.products}</h3>
              <p>Database Products</p>
            </div>

            <div className="stat">
              <h3>{stats.categories}</h3>
              <p>Active Categories</p>
            </div>

            <div className="stat">
              <h3>{stats.brands}</h3>
              <p>Official Brands</p>
            </div>

            <div className="stat">
              <h3>100%</h3>
              <p>Uptime & Reliability</p>
            </div>

          </div>

        </div>

      </div>
    </PageTransition>
  );
}

export default About;

