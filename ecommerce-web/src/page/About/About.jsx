import React, { useEffect, useState } from "react";
import "./about.css";
import PageTransition from '../../components/PageTransition';
import { api } from '../../services/api';

function About() {
  const [stats, setStats] = useState({
    products: '–',
    categories: '–',
    brands: '–',
    loading: true
  });

  useEffect(() => {
    const loadDynamicStats = async () => {
      try {
        const [prodRes, typesRes, brandsRes] = await Promise.all([
          api.products.getAll({ pageSize: 1 }),
          api.products.getTypes(),
          api.products.getBrands()
        ]);

        const typesCount = Array.isArray(typesRes) ? typesRes.length : (typesRes?.data?.length ?? 0);
        const brandsCount = Array.isArray(brandsRes) ? brandsRes.length : (brandsRes?.data?.length ?? 0);

        setStats({
          products: prodRes?.count ?? 0,
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
              A demo shop built with an ASP.NET Core Web API and React.
            </p>
          </div>
        </div>

        <div className="about-content container">

          <div className="about-text">
            <h2>Our Story & Mission</h2>
            <p>
              LuxeStore is a portfolio project that shows a complete online shop end to end: an ASP.NET Core 10 Web API with SQL Server behind it, and a React 19 storefront and admin console in front. Prices and stock are always checked on the server, and every order keeps its own copy of the item prices and shipping address.
            </p>
          </div>

          <div className="about-features">

            <div className="feature">
              <h3>Fast Delivery</h3>
              <p>Reliable tracked delivery with transparent shipping methods.</p>
            </div>

            <div className="feature">
              <h3>Secure Payment</h3>
              <p>Card payment through Stripe (test mode) or Cash on Delivery.</p>
            </div>

            <div className="feature">
              <h3>Verified Quality</h3>
              <p>100% authentic curated lifestyle essentials and apparel.</p>
            </div>

          </div>

          <div className="about-stats">

            <div className="stat">
              <h3>{stats.products}</h3>
              <p>Products</p>
            </div>

            <div className="stat">
              <h3>{stats.categories}</h3>
              <p>Categories</p>
            </div>

            <div className="stat">
              <h3>{stats.brands}</h3>
              <p>Brands</p>
            </div>

            <div className="stat">
              <h3>2</h3>
              <p>Payment Options</p>
            </div>

          </div>

        </div>

      </div>
    </PageTransition>
  );
}

export default About;

